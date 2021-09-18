#version 330 core

in vec3 aPosition;
in vec2 aTexCoords;

out vec2 frag_texCoords;

uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

void main() 
{
    gl_Position = vec4(aPosition, 1.0) * uModel * uView * uProjection;
    frag_texCoords = aTexCoords;
}
