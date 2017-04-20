#version 330 core
  
layout (location = 0) in vec3 position;

uniform vec3 pointPosition;
void main()
{
	vec3 pos = position + pointPosition;
    gl_Position = vec4(pos.x, pos.y, pos.z, 1.0);
}