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

uniform float t1;
uniform float t2;
uniform float t3;
uniform float t4;
uniform float t5;
uniform float t6;
uniform float t7;


float BSplineN0(float from, float to, float t)
{
	return (t >= from && t < to) ? 1.0 : 0.0;
}

float BSplineMix(float n1from, float n1to, float n2from, float n2to, float t, float N1, float N2, float n)
{
	return (t-n1from)/(n1to-n1from)*N1 + (n2to-t)/(n2to-n2from)*N2;
}


vec4 EvaluateFunctions(float t)
{
	float v0 = BSplineN0(0, t1, t);
	float v1 = BSplineN0(t1, t2, t);
	float v2 = BSplineN0(t2, t3, t);
	float v3 = BSplineN0(t3, t4, t);
	float v4 = BSplineN0(t4, t5, t);
	float v5 = BSplineN0(t5, t6, t);
	float v6 = BSplineN0(t6, t7, t);

	v0 = BSplineMix(0, t1, t1, t2, t, v0, v1, 1);
	v1 = BSplineMix(t1, t2, t2, t3, t, v1, v2, 1);
	v2 = BSplineMix(t2, t3, t3, t4, t, v2, v3, 1);
	v3 = BSplineMix(t3, t4, t4, t5, t, v3, v4, 1);
	v4 = BSplineMix(t4, t5, t5, t6, t, v4, v5, 1);
	v5 = BSplineMix(t5, t6, t6, t7, t, v5, v6, 1);

	v0 = BSplineMix(0, t2, t1, t3, t, v0, v1, 2);
	v1 = BSplineMix(t1, t3, t2, t4, t, v1, v2, 2);
	v2 = BSplineMix(t2, t4, t3, t5, t, v2, v3, 2);
	v3 = BSplineMix(t3, t5, t4, t6, t, v3, v4, 2);
	v4 = BSplineMix(t4, t6, t5, t7, t, v4, v5, 2);

	v0 = BSplineMix(0, t3, t1, t4, t, v0, v1, 3);
	v1 = BSplineMix(t1, t4, t2, t5, t, v1, v2, 3);
	v2 = BSplineMix(t2, t5, t3, t6, t, v2, v3, 3);
	v3 = BSplineMix(t3, t6, t4, t7, t, v3, v4, 3);
	return vec4(v0, v1, v2, v3);



}

vec4 NurbsVal(vec4 value)
{
	return vec4(value.x*value.w, value.y*value.w, value.z*value.w,value.w);
}

vec3 EvaluateBspline(float u, float v)
{
	vec4 uVal = EvaluateFunctions(u);
	vec4 vVal = EvaluateFunctions(v);

	vec3 result = vec3(0,0,0);

	result += b00.xyz * uVal.x * vVal.x;
	result += b01.xyz * uVal.y * vVal.x;
	result += b02.xyz * uVal.z * vVal.x;
	result += b03.xyz * uVal.w * vVal.x;
				 
	result += b10.xyz * uVal.x * vVal.y;
	result += b11.xyz * uVal.y * vVal.y;
	result += b12.xyz * uVal.z * vVal.y;
	result += b13.xyz * uVal.w * vVal.y;
				 
	result += b20.xyz * uVal.x * vVal.z;
	result += b21.xyz * uVal.y * vVal.z;
	result += b22.xyz * uVal.z * vVal.z;
	result += b23.xyz * uVal.w * vVal.z;
				 
	result += b30.xyz * uVal.x * vVal.w;
	result += b31.xyz * uVal.y * vVal.w;
	result += b32.xyz * uVal.z * vVal.w;
	result += b33.xyz * uVal.w * vVal.w;

	return result;
}

void main()
{
	vec3 pos = EvaluateBspline(position.x,position.y);
    gl_Position = vec4(pos.x, pos.y, pos.z, 1.0) * viewMatrix * projMatrix;
}