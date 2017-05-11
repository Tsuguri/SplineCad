#version 330 core
  
layout (location = 0) in vec3 position;

uniform mat4 viewMatrix;
uniform mat4 projMatrix;

uniform vec4 b00;
uniform vec4 b01;
uniform vec4 b02;
uniform vec4 b03;
uniform vec4 b10;
uniform vec4 b11;
uniform vec4 b12;
uniform vec4 b13;
uniform vec4 b20;
uniform vec4 b21;
uniform vec4 b22;
uniform vec4 b23;
uniform vec4 b30;
uniform vec4 b31;
uniform vec4 b32;
uniform vec4 b33;

float BSplineN0(float i, float t)
{
	return (t>=i -1 && t<i) ? 1.0 : 0.0;
}

float BSplineMix(float n, float i, float t, float N1, float N2)
{
	return ((t-i+1)*N1 + (i+n-t)*N2) / n;
}


vec4 EvaluateFunctions(float t)
{
	float v0 = BSplineN0(-2,t);
	float v1 = BSplineN0(-1,t);
	float v2 = BSplineN0(0,t);
	float v3 = BSplineN0(1,t);
	float v4 = BSplineN0(2,t);
	float v5 = BSplineN0(3,t);
	float v6 = BSplineN0(4,t);

	v0 = BSplineMix(1, -2, t, v0, v1);
	v1 = BSplineMix(1, -1, t, v1, v2);
	v2 = BSplineMix(1, 0, t, v2, v3);
	v3 = BSplineMix(1, 1, t, v3, v4);
	v4 = BSplineMix(1, 2, t, v4, v5);
	v5 = BSplineMix(1, 3, t, v5, v6);

	v0 = BSplineMix(2, -2, t, v0, v1);
	v1 = BSplineMix(2, -1, t, v1, v2);
	v2 = BSplineMix(2, 0, t, v2, v3);
	v3 = BSplineMix(2, 1, t, v3, v4);
	v4 = BSplineMix(2, 2, t, v4, v5);

	v0 = BSplineMix(3, -2, t, v0, v1);
	v1 = BSplineMix(3, -1, t, v1, v2);
	v2 = BSplineMix(3, 0, t, v2, v3);
	v3 = BSplineMix(3, 1, t, v3, v4);

	return vec4(v0, v1, v2, v3);
}

vec3 EvaluateBspline(float u, float v)
{
	vec4 uVal = EvaluateFunctions(u);
	vec4 vVal = EvaluateFunctions(v);

	vec4 result = vec4(0,0,0,0);

	result += b00 * uVal.x * vVal.x;
	result += b01 * uVal.y * vVal.x;
	result += b02 * uVal.z * vVal.x;
	result += b03 * uVal.w * vVal.x;

	result += b10 * uVal.x * vVal.y;
	result += b11 * uVal.y * vVal.y;
	result += b12 * uVal.z * vVal.y;
	result += b13 * uVal.w * vVal.y;

	result += b20 * uVal.x * vVal.z;
	result += b21 * uVal.y * vVal.z;
	result += b22 * uVal.z * vVal.z;
	result += b23 * uVal.w * vVal.z;

	result += b30 * uVal.x * vVal.w;
	result += b31 * uVal.y * vVal.w;
	result += b32 * uVal.z * vVal.w;
	result += b33 * uVal.w * vVal.w;

	return result.xyz;
}

void main()
{
	vec3 pos = EvaluateBspline(position.x,position.y);
    gl_Position = vec4(pos.x, pos.y, pos.z, 1.0) * viewMatrix * projMatrix;
}