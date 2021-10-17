using System.Collections.Generic;

namespace Cubic.Engine.Render
{
    public class ForwardRenderer
    {
        private Dictionary<string, Shader> _shaders;
        //private 

        public ForwardRenderer()
        {
            _shaders = new Dictionary<string, Shader>();
        }
    }
}