#version 330 core
  
layout (location = 0) in vec3 position;

uniform mat4 viewMatrix;
uniform mat4 projMatrix;

uniform vec3 b00;
uniform vec3 b01;
uniform vec3 b02;
uniform vec3 b03;
uniform vec3 b10;
uniform vec3 b11;
uniform vec3 b12;
uniform vec3 b13;
uniform vec3 b20;
uniform vec3 b21;
uniform vec3 b22;
uniform vec3 b23;
uniform vec3 b30;
uniform vec3 b31;
uniform vec3 b32;
uniform vec3 b33;

out vec3 vNormal;
out vec3 vWorldPos;


float BSplineN0(float i, float t)
{
	return (t>=i -1 && t<i) ? 1.0 : 0.0;
}

float BSplineMix(float n, float i, float t, float N1, float N2)
{
	return ((t-i+1)*N1 + (i+n-t)*N2) / n;
}


vec4 EvaluateBasis3(float t)
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

vec3 EvaluateBasis2(float t)
{
	float v0 = BSplineN0(-1,t);
	float v1 = BSplineN0(0,t);
	float v2 = BSplineN0(1,t);
	float v3 = BSplineN0(2,t);
	float v4 = BSplineN0(3,t);

	v0 = BSplineMix(1, -1, t, v0, v1);
	v1 = BSplineMix(1, 0, t, v1, v2);
	v2 = BSplineMix(1, 1, t, v2, v3);
	v3 = BSplineMix(1, 2, t, v3, v4);

	v0 = BSplineMix(2, -1, t, v0, v1);
	v1 = BSplineMix(2, 0, t, v1, v2);
	v2 = BSplineMix(2, 1, t, v2, v3);

	return vec3(v0, v1, v2);
}

vec3 Evaluate(vec3 p0, vec3 p1, vec3 p2, vec3 p3, float t)
{
	vec4 basis = EvaluateBasis3(t);

	return basis.x * p0 + basis.y * p1 + basis.z * p2 + basis.w * p3;
}

vec3 EvaluateD(vec3 p0, vec3 p1, vec3 p2, vec3 p3, float t)
{
	vec3 d0 = p1 - p0;
	vec3 d1 = p2 - p1;
	vec3 d2 = p3 - p2;

	vec3 basis = EvaluateBasis2(t);

	return basis.x * d0 + basis.y * d1 + basis.z * d2;
}

void main()
{
	vec3 row0 = Evaluate(b00, b01, b02, b03, position.x);
	vec3 row1 = Evaluate(b10, b11, b12, b13, position.x);
	vec3 row2 = Evaluate(b20, b21, b22, b23, position.x);
	vec3 row3 = Evaluate(b30, b31, b32, b33, position.x);

	vec3 col0 = Evaluate(b00, b10, b20, b30, position.y);
	vec3 col1 = Evaluate(b01, b11, b21, b31, position.y);
	vec3 col2 = Evaluate(b02, b12, b22, b32, position.y);
	vec3 col3 = Evaluate(b03, b13, b23, b33, position.y);

	vec3 pos = Evaluate(row0, row1, row2, row3, position.y);

	vec3 posDU = EvaluateD(row0, row1, row2, row3, position.y);
	vec3 posDV = EvaluateD(col0, col1, col2, col3, position.x);

	vNormal = normalize(cross(posDV, posDU));
	vWorldPos = pos;

    gl_Position = vec4(pos.x, pos.y, pos.z, 1.0) * viewMatrix * projMatrix;
}