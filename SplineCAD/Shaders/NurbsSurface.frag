#version 330 core

out vec4 color;

uniform vec3 lightPos;
uniform vec3 surfColor;
uniform vec3 camPos;

in vec3 vNormal;
in vec3 vWorldPos;

const vec3 ambientColor = vec3(0.2f, 0.2f, 0.2f);
const vec3 lightColor = vec3(1.0f, 1.0f, 1.0f);
const float kd = 0.6f;
const float ks = 0.5f;
const float m = 100.0f;

void main()
{
	vec3 viewVec = normalize(camPos - vWorldPos);
	vec3 clr = surfColor * ambientColor;
	vec3 lightVec = normalize(lightPos - vWorldPos);
	vec3 halfVec = normalize(camPos + lightVec);

	clr += lightColor * surfColor * kd * clamp(dot(vNormal, lightVec), 0.0f, 1.0f);
	float nh = clamp(dot(vNormal, halfVec), 0.0f, 1.0f);
	nh = pow(nh, m) * ks;
    clr += lightColor * nh;

	color = vec4(clr, 1.0f);
} 