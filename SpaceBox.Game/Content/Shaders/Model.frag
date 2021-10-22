#include "Lighting.glsl"

in vec2 frag_texCoords;
in vec3 frag_position;
in vec3 frag_normal;
in vec4 frag_lightSpace;

out vec4 out_color;

//uniform sampler2D uTexture;
uniform Material uMaterial;
uniform vec3 uViewPos;
uniform DirLight uDirLight;
uniform float uOpacity;

uniform sampler2D uShadowMap;

void main() 
{
    vec3 norm = normalize(frag_normal);
    vec3 viewDir = normalize(uViewPos - frag_position);
    
    vec3 result = CalculateDirectional(uDirLight, norm, viewDir);
    
    out_color = vec4(result, uOpacity);
}


