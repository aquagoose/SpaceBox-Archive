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

out vec4 out_color;

//uniform sampler2D uTexture;
uniform Material uMaterial;
uniform vec3 uViewPos;
uniform DirLight uDirLight;
uniform float uOpacity;

vec3 CalculateDirectional(DirLight light, vec3 normal, vec3 viewDir);

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
    
    return ambient + diffuse + specular;
}
