#version 330 core

in vec4 aPosition;

out vec2 frag_texCoords;

uniform mat4 uProjection;

void main() 
{
    gl_Position = vec4(aPosition.xy, 0.0, 1.0) * uProjection;
    frag_texCoords = aPosition.zw;
}
