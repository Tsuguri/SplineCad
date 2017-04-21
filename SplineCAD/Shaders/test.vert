#version 330 core
  
layout (location = 0) in vec3 position;
uniform mat4 viewMatrix;
uniform mat4 projMatrix;

void main()
{
    gl_Position = vec4(position.x, position.y, position.z, 1.0) * 
		viewMatrix * projMatrix;
}