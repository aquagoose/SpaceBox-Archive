in vec2 frag_texCoords;

out vec4 out_color;

uniform sampler2D uTexture;
uniform vec4 uColor;

void main() 
{
    out_color = vec4(1.0, 1.0, 1.0, texture(uTexture, frag_texCoords).r) * uColor;
}
