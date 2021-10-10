#version 330 core

struct Material
{
    sampler2D diffuse;
    sampler2D specular;
    float shininess;
};

struct DirLight
{
    vec3 direction;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

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

vec3 CalculateDirectional(DirLight light, vec3 normal, vec3 viewDir);
float CalculateShadow(vec3 lightDir);

void main() 
{
    vec3 norm = normalize(frag_normal);
    vec3 viewDir = normalize(uViewPos - frag_position);
    
    vec3 result = CalculateDirectional(uDirLight, norm, viewDir);
    
    out_color = vec4(result, uOpacity);
}

vec3 CalculateDirectional(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), uMaterial.shininess);
    
    vec3 ambient = light.ambient * vec3(texture(uMaterial.diffuse, frag_texCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(uMaterial.diffuse, frag_texCoords));
    vec3 specular = light.specular * spec * vec3(texture(uMaterial.specular, frag_texCoords));
    
    float shadow = CalculateShadow(lightDir);
    
    return ambient + (1 - shadow) * (diffuse + specular);
    //return ambient + diffuse + specular;
}

float CalculateShadow(vec3 lightDir)
{
    vec3 projCoords = frag_lightSpace.xyz / frag_lightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;
    
    float closestDepth = texture(uShadowMap, projCoords.xy).r;
    float currentDepth = projCoords.z;
    //float bias = max(0.05 * (1.0 - dot(frag_normal, lightDir)), 0.005);
    float bias = 0.0001;
    float shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;
    
    return shadow;
}
