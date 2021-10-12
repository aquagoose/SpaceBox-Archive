#version 330 core

in vec2 frag_texCoords;

out vec4 out_color;

uniform sampler2D uTexture;
uniform vec4 uColor;

void main()
{
    out_color = texture(uTexture, frag_texCoords) * uColor;
}
