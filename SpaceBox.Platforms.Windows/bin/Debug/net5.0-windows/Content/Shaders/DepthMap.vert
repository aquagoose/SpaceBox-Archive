#version 330 core

in vec3 aPosition;

uniform mat4 uLightSpace;
uniform mat4 uModel;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uModel * uLightSpace;
}
