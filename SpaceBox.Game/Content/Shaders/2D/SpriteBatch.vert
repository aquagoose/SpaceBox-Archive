in vec2 aPosition;
in vec2 aTexCoords;

out vec2 frag_texCoords;

uniform mat4 uModel;
uniform mat4 uTransform;
uniform mat4 uProjection;

void main() 
{
    // Model-view-projection multiplication of sprite.
    gl_Position = vec4(aPosition, 0.0, 1.0) * uModel * uTransform * uProjection;
    frag_texCoords = aTexCoords;
    frag_texCoords.y *= -1;
}
