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

uniform float tu1;
uniform float tu2;
uniform float tu3;
uniform float tu4;
uniform float tu5;
uniform float tu6;
uniform float tu7;

uniform float tv1;
uniform float tv2;
uniform float tv3;
uniform float tv4;
uniform float tv5;
uniform float tv6;
uniform float tv7;

float BSplineN0(float from, float to, float t)
{
	return (t >= from && t < to) ? 1.0 : 0.0;
}

float BSplineMix(float n1from, float n1to, float n2from, float n2to, float t, float N1, float N2, float n)
{
	return (t-n1from)/(n1to-n1from)*N1 + (n2to-t)/(n2to-n2from)*N2;
}


vec4 EvaluateUFunctions(float t)
{
	float v0 = BSplineN0(0, tu1, t);
	float v1 = BSplineN0(tu1, tu2, t);
	float v2 = BSplineN0(tu2, tu3, t);
	float v3 = BSplineN0(tu3, tu4, t);
	float v4 = BSplineN0(tu4, tu5, t);
	float v5 = BSplineN0(tu5, tu6, t);
	float v6 = BSplineN0(tu6, tu7, t);

	v0 = BSplineMix(0, tu1, tu1, tu2, t, v0, v1, 1);
	v1 = BSplineMix(tu1, tu2, tu2, tu3, t, v1, v2, 1);
	v2 = BSplineMix(tu2, tu3, tu3, tu4, t, v2, v3, 1);
	v3 = BSplineMix(tu3, tu4, tu4, tu5, t, v3, v4, 1);
	v4 = BSplineMix(tu4, tu5, tu5, tu6, t, v4, v5, 1);
	v5 = BSplineMix(tu5, tu6, tu6, tu7, t, v5, v6, 1);

	v0 = BSplineMix(0, tu2, tu1, tu3, t, v0, v1, 2);
	v1 = BSplineMix(tu1, tu3, tu2, tu4, t, v1, v2, 2);
	v2 = BSplineMix(tu2, tu4, tu3, tu5, t, v2, v3, 2);
	v3 = BSplineMix(tu3, tu5, tu4, tu6, t, v3, v4, 2);
	v4 = BSplineMix(tu4, tu6, tu5, tu7, t, v4, v5, 2);

	v0 = BSplineMix(0, tu3, tu1, tu4, t, v0, v1, 3);
	v1 = BSplineMix(tu1, tu4, tu2, tu5, t, v1, v2, 3);
	v2 = BSplineMix(tu2, tu5, tu3, tu6, t, v2, v3, 3);
	v3 = BSplineMix(tu3, tu6, tu4, tu7, t, v3, v4, 3);
	return vec4(v0, v1, v2, v3);
}

vec4 EvaluateVFunctions(float t)
{
	float v0 = BSplineN0(0, tv1, t);
	float v1 = BSplineN0(tv1, tv2, t);
	float v2 = BSplineN0(tv2, tv3, t);
	float v3 = BSplineN0(tv3, tv4, t);
	float v4 = BSplineN0(tv4, tv5, t);
	float v5 = BSplineN0(tv5, tv6, t);
	float v6 = BSplineN0(tv6, tv7, t);

	v0 = BSplineMix(0, tv1, tv1, tv2, t, v0, v1, 1);
	v1 = BSplineMix(tv1, tv2, tv2, tv3, t, v1, v2, 1);
	v2 = BSplineMix(tv2, tv3, tv3, tv4, t, v2, v3, 1);
	v3 = BSplineMix(tv3, tv4, tv4, tv5, t, v3, v4, 1);
	v4 = BSplineMix(tv4, tv5, tv5, tv6, t, v4, v5, 1);
	v5 = BSplineMix(tv5, tv6, tv6, tv7, t, v5, v6, 1);

	v0 = BSplineMix(0, tv2, tv1, tv3, t, v0, v1, 2);
	v1 = BSplineMix(tv1, tv3, tv2, tv4, t, v1, v2, 2);
	v2 = BSplineMix(tv2, tv4, tv3, tv5, t, v2, v3, 2);
	v3 = BSplineMix(tv3, tv5, tv4, tv6, t, v3, v4, 2);
	v4 = BSplineMix(tv4, tv6, tv5, tv7, t, v4, v5, 2);

	v0 = BSplineMix(0, tv3, tv1, tv4, t, v0, v1, 3);
	v1 = BSplineMix(tv1, tv4, tv2, tv5, t, v1, v2, 3);
	v2 = BSplineMix(tv2, tv5, tv3, tv6, t, v2, v3, 3);
	v3 = BSplineMix(tv3, tv6, tv4, tv7, t, v3, v4, 3);
	return vec4(v0, v1, v2, v3);
}

vec4 NurbsVal(vec4 value)
{
	return vec4(value.x*value.w, value.y*value.w, value.z*value.w,value.w);
}

vec3 EvaluateBspline(float u, float v)
{
	vec4 uVal = EvaluateUFunctions(u*(tu4-tu3)+tu3);
	vec4 vVal = EvaluateVFunctions(v*(tv4-tv3)+tv3);

	vec4 result = vec4(0,0,0,0);

	result += NurbsVal(b00) * uVal.x * vVal.x;
	result += NurbsVal(b01) * uVal.y * vVal.x;
	result += NurbsVal(b02) * uVal.z * vVal.x;
	result += NurbsVal(b03) * uVal.w * vVal.x;
				 
	result += NurbsVal(b10) * uVal.x * vVal.y;
	result += NurbsVal(b11) * uVal.y * vVal.y;
	result += NurbsVal(b12) * uVal.z * vVal.y;
	result += NurbsVal(b13) * uVal.w * vVal.y;
				 		  
	result += NurbsVal(b20) * uVal.x * vVal.z;
	result += NurbsVal(b21) * uVal.y * vVal.z;
	result += NurbsVal(b22) * uVal.z * vVal.z;
	result += NurbsVal(b23) * uVal.w * vVal.z;
				 		  
	result += NurbsVal(b30) * uVal.x * vVal.w;
	result += NurbsVal(b31) * uVal.y * vVal.w;
	result += NurbsVal(b32) * uVal.z * vVal.w;
	result += NurbsVal(b33) * uVal.w * vVal.w;

	return result.xyz/result.w;
}

void main()
{
	vec3 pos = EvaluateBspline(position.x,position.y);
    gl_Position = vec4(pos.x, pos.y, pos.z, 1.0) * viewMatrix * projMatrix;
}