#version 330 core
  
layout (location = 0) in vec3 position;

uniform vec3 pointPosition = vec3(0, 0, 0);
uniform vec4 rationalPointPosition = vec4(0, 0, 0, 0);
uniform mat4 viewMatrix;
uniform mat4 projMatrix;

void main()
{
	vec4 pos = vec4(position + pointPosition+rationalPointPosition.xyz, 1.0);
    gl_Position = pos * viewMatrix * projMatrix;
}