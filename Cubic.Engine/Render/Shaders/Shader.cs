using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Cubic.Engine.Utilities;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Cubic.Engine.Render.Shaders
{
    /// <summary>
    /// Represents a GLSL shader program.
    /// </summary>
    public class Shader : IDisposable
    {
        private int _program;
        private Dictionary<string, int> _uniformLocations;

        private bool _disposed;

        #region Constructors
        
        /// <summary>
        /// Create a new shader with the given vertex & fragment paths/code.
        /// </summary>
        /// <param name="vertex">The vertex shader path/code.</param>
        /// <param name="fragment">The fragment shader path/code.</param>
        /// <param name="loadType">Whether the shader should be loaded from a file or directly from a string.</param>
        /// <param name="autoDispose">If true, the shader will automatically dispose on application exit.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <see cref="loadType"/> is not a valid <see cref="ShaderLoadType"/>.</exception>
        public Shader(string vertex, string fragment, ShaderLoadType loadType = ShaderLoadType.File, bool autoDispose = true)
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            switch (loadType)
            {
                case ShaderLoadType.File:
                    // If the shader is a file, then load the file text, then set the shader source.
                    GL.ShaderSource(vertexShader, File.ReadAllText(vertex));
                    GL.ShaderSource(fragmentShader, File.ReadAllText(fragment));
                    break;
                case ShaderLoadType.String:
                    // Otherwise, just set the shader source directly.
                    GL.ShaderSource(vertexShader, vertex);
                    GL.ShaderSource(fragmentShader, fragment);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(loadType));
            }
            
            CompileShader(vertexShader);
            CompileShader(fragmentShader);

            _program = GL.CreateProgram();
            // Attach the shaders to the program
            GL.AttachShader(_program, vertexShader);
            GL.AttachShader(_program, fragmentShader);
            LinkProgram(_program);
            // Detach & delete the shaders as we no longer need them.
            GL.DetachShader(_program, vertexShader);
            GL.DetachShader(_program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Get a list of all uniform locations.
            // This is useful as an exception can be thrown if a uniform does not exist.
            _uniformLocations = new Dictionary<string, int>();
            GL.GetProgram(_program, GetProgramParameterName.ActiveUniforms, out int uniforms);
            for (int i = 0; i < uniforms; i++)
            {
                string name = GL.GetActiveUniform(_program, i, out _, out _);
                int location = GL.GetUniformLocation(_program, name);
                _uniformLocations.Add(name, location);
            }

            if (autoDispose)
                DisposeManager.Add(this);
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Set this shader as the active OpenGL shader.
        /// </summary>
        public void Use() => GL.UseProgram(_program);
        
        /// <summary>
        /// Set a uniform value with the given name.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <param name="value">The value that should be set.</param>
        /// <param name="transpose">Whether or not the given value should be transposed. This is <b>ONLY VALID</b> with a matrix value, and will be ignored otherwise.</param>
        /// <typeparam name="T">
        ///     <list type="bullet">
        ///         <listheader>
        ///             <term>Accepts the following types:</term>
        ///         </listheader>
        ///         <item><term><see cref="bool"/></term></item>
        ///         <item><term><see cref="int"/></term></item>
        ///         <item><term><see cref="float"/></term></item>
        ///         <item><term><see cref="double"/></term></item>
        ///         <item><term><see cref="Vector2"/></term></item>
        ///         <item><term><see cref="Vector3"/></term></item>
        ///         <item><term><see cref="Vector4"/></term></item>
        ///         <item><term><see cref="Matrix4"/></term></item>
        ///         <item><term><see cref="Color"/></term></item>
        ///     </list>
        /// </typeparam>
        /// <exception cref="NotSupportedException">Thrown if the given type is not supported by the shader.</exception>
        public void SetUniform<T>(string name, T value, bool transpose = true)
        {
            int location = GetUniformLocation(name);

            switch (value)
            {
                case bool bValue:
                    GL.Uniform1(location, bValue ? 1 : 0);
                    break;
                case int iValue:
                    GL.Uniform1(location, iValue);
                    break;
                case float fValue:
                    GL.Uniform1(location, fValue);
                    break;
                case double dValue:
                    GL.Uniform1(location, dValue);
                    break;
                case Vector2 v2Value:
                    GL.Uniform2(location, v2Value);
                    break;
                case Vector3 v3Value:
                    GL.Uniform3(location, v3Value);
                    break;
                case Vector4 v4Value:
                    GL.Uniform4(location, v4Value);
                    break;
                case Matrix4 m4Value:
                    GL.UniformMatrix4(location, transpose, ref m4Value);
                    break;
                case Color cValue:
                    GL.Uniform4(location, cValue.R / 255f, cValue.G / 255f, cValue.B / 255f, cValue.A / 255f);
                    break;
                default:
                    throw new NotSupportedException("The given value's type is not supported by the shader.");
            }
        }

        public int GetAttribLocation(string name) => GL.GetAttribLocation(_program, name);

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Compile a shader object.
        /// </summary>
        /// <param name="shader">The shader object.</param>
        /// <exception cref="ShaderException">Thrown if the shader failed to compile.</exception>
        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
            if (status != 1)
                throw new ShaderException(ShaderExceptionType.Compiling, shader, GL.GetShaderInfoLog(shader));
        }

        /// <summary>
        /// Link a program object.
        /// </summary>
        /// <param name="program">The program object.</param>
        /// <exception cref="ShaderException">Thrown if the program failed to link.</exception>
        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
            if (status != 1)
                throw new ShaderException(ShaderExceptionType.Linking, program, GL.GetProgramInfoLog(program));
        }

        /// <summary>
        /// Run a pre-processor on the given shader code.
        /// </summary>
        /// <param name="code"></param>
        private static void PreProcess(string code)
        {
            StringBuilder newCode = new StringBuilder(code);
            newCode.Insert(0, "#version 330 core");

            foreach (string line in newCode.ToString().Split('\n'))
            {
                if (line.StartsWith("#include"))
                {
                    
                }
            }
        }

        /// <summary>
        /// Get the uniform location with the given name.
        /// </summary>
        /// <param name="name">The name of the uniform.</param>
        /// <returns>The uniform location.</returns>
        /// <exception cref="ShaderException">Thrown if the uniform name does not exist in the shader.</exception>
        private int GetUniformLocation(string name)
        {
            if (!_uniformLocations.ContainsKey(name))
                throw new ShaderException($"Given uniform '{name}' does not exist in the shader.");
            
            return _uniformLocations[name];
        }
        
        #endregion

        public void Dispose()
        {
            if (_disposed) return;
            GL.DeleteProgram(_program);
            Console.WriteLine($"Shader '{_program}' disposed.");
            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }

    /// <summary>
    /// Represents an exception thrown by a shader object.
    /// </summary>
    public class ShaderException : Exception
    {
        public ShaderException(ShaderExceptionType type, int shaderProgram, string infoLog)
            : base(
                $"Error {type.ToString().ToLower()} {(type == ShaderExceptionType.Compiling ? "shader" : "program")} '{shaderProgram}':\n{infoLog}") {}
        
        public ShaderException(string message) : base(message) {}
    }

    /// <summary>
    /// The type of shader exception.
    /// </summary>
    public enum ShaderExceptionType
    {
        Compiling,
        Linking
    }
    
    /// <summary>
    /// How the shader will be interpreted by the constructor. File: The shader is located externally. String: The shader is directly in code.
    /// </summary>
    public enum ShaderLoadType
    {
        File,
        String
    }
}