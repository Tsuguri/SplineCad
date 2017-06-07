#version 330 core
 
layout (location = 0) in vec3 position;

const int MaxPoints = 192;
struct BaseFunction
{
	vec4 controlPoint;
	float uStart;
	vec4 uDistances;
	float vStart;
	vec4 vDistances;
};

uniform BaseFunction functions[MaxPoints];

uniform float usedPoints;
uniform vec2 size;

uniform mat4 viewMatrix;
uniform mat4 projMatrix;

out vec3 vNormal;
out vec3 vWorldPos;

float BSplineN0(float from, float to, float t)
{
	return (t >= from && t < to) ? 1.0 : 0.0;
}

float BSplineMix(float n1from, float n1to, float n2from, float n2to, float t, float N1, float N2, float n)
{
	return (t-n1from)/(n1to-n1from)*N1 + (n2to-t)/(n2to-n2from)*N2;
}

float EvaluateFunction(float t, float start, vec4 divs)
{
	float v0 = BSplineN0(start, divs.x, t);
	float v1 = BSplineN0(divs.x, divs.y, t);
	float v2 = BSplineN0(divs.y, divs.z, t);
	float v3 = BSplineN0(divs.z, divs.w, t);

	v0 = BSplineMix(start, divs.x, divs.x, divs.y, t, v0, v1, 1);
	v1 = BSplineMix(divs.x, divs.y, divs.y, divs.z, t, v1, v2, 1);
	v2 = BSplineMix(divs.y, divs.z, divs.z, divs.w, t, v2, v3, 1);

	v0 = BSplineMix(start, divs.y, divs.x, divs.z, t, v0, v1, 2);
	v1 = BSplineMix(divs.x, divs.z, divs.y, divs.w, t, v1, v2, 2);

	v0 = BSplineMix(start, divs.z, divs.x, divs.w, t, v0, v1, 3);

	return v0;
}

float EvaluateDerivative(float t, float start, vec4 divs)
{
	float v0 = BSplineN0(start, divs.x, t);
	float v1 = BSplineN0(divs.x, divs.y, t);
	float v2 = BSplineN0(divs.y, divs.z, t);

	v0 = BSplineMix(start, divs.x, divs.x, divs.y, t, v0, v1, 1);
	v1 = BSplineMix(divs.x, divs.y, divs.y, divs.z, t, v1, v2, 1);

	v0 = BSplineMix(start, divs.y, divs.x, divs.z, t, v0, v1, 2);

	return v0;
}

vec4 NurbsVal(vec4 value)
{
	return vec4(value.x*value.w, value.y*value.w, value.z*value.w,value.w);
}

vec4 EvaluateTsplinePoint(vec4 point, float uStart, vec4 uDiv, float vStart, vec4 vDiv, vec2 uv)
{
	vec4 pt = NurbsVal(point);
	float uFun = EvaluateFunction(uv.x, uStart, uDiv);
	float vFun = EvaluateFunction(uv.y, vStart, vDiv);
	return pt * vFun * uFun;
}

vec4 EvaluateTsplinePointU(vec4 point, float uStart, vec4 uDiv, float vStart, vec4 vDiv, vec2 uv)
{
	vec4 pt = NurbsVal(point);
	float uFun = EvaluateDerivative(uv.x, uStart, uDiv);
	float vFun = EvaluateFunction(uv.y, vStart, vDiv);
	return pt * vFun * uFun;
}

vec4 EvaluateTsplinePointV(vec4 point, float uStart, vec4 uDiv, float vStart, vec4 vDiv, vec2 uv)
{
	vec4 pt = NurbsVal(point);
	float uFun = EvaluateFunction(uv.x, uStart, uDiv);
	float vFun = EvaluateDerivative(uv.y, vStart, vDiv);
	return pt * vFun * uFun;
}

void main()
{
	vec2 uv = vec2(position.x * size.x, position.y * size.y);

	vec4 tsplineValue = vec4(0.0, 0.0, 0.0, 0.0);
	vec4 tsplineU = vec4(0.0f, 0.0f, 0.0f, 0.0f);
	vec4 tsplineV = vec4(0.0f, 0.0f, 0.0f, 0.0f);

	for(int i = 0; i < usedPoints; i++)
	{
		tsplineValue += EvaluateTsplinePoint(functions[i].controlPoint, functions[i].uStart, functions[i].uDistances, functions[i].vStart, functions[i].vDistances, uv);
		tsplineU += EvaluateTsplinePointU(functions[i].controlPoint, functions[i].uStart, functions[i].uDistances, functions[i].vStart, functions[i].vDistances, uv);
		tsplineV += EvaluateTsplinePointV(functions[i].controlPoint, functions[i].uStart, functions[i].uDistances, functions[i].vStart, functions[i].vDistances, uv);

	}
	vec3 result = tsplineValue.xyz / tsplineValue.w;
	vec3 posU = tsplineU.xyz / tsplineU.w;
	vec3 posV = tsplineV.xyz / tsplineV.w;

	vWorldPos = result;
	vNormal = normalize(cross(posV - result, posU - result));
	//if(tsplineValue.length()<0.01)
	//result = vec3(position.x, tsplineValue.w, position.y);
    gl_Position = vec4(result, 1.0) * viewMatrix * projMatrix;
}