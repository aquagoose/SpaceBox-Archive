#version 330 core

in vec3 aPosition;

out vec3 frag_texCoords;

uniform mat4 uView;
uniform mat4 uProjection;

void main() 
{
    gl_Position = vec4(aPosition, 1.0) * uView * uProjection;
    frag_texCoords = aPosition;
}
