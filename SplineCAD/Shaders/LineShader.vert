#version 330 core
  
layout (location = 0) in vec3 position;

uniform mat4 viewMatrix;
uniform mat4 projMatrix;

void main()
{
	vec3 pos = position;
    gl_Position = vec4(pos.x, pos.y, pos.z, 1.0) * viewMatrix * projMatrix;
}