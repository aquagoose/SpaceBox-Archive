in vec3 frag_texCoords;

out vec4 out_color;

uniform samplerCube uSkybox;

void main() 
{
    out_color = texture(uSkybox, frag_texCoords);
}
