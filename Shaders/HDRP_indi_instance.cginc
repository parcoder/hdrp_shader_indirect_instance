#define CUSTOM_PI 3.141592653589793238462

uint CHash(uint s)
{
	s ^= 2747636419u;
	s *= 2654435769u;
	s ^= s >> 16;
	s *= 2654435769u;
	s ^= s >> 16;
	s *= 2654435769u;
	return s;
}

float Random(uint seed)
{
	return float(CHash(seed)) / 4294967295.0;
}

float3x3 Euler3x3(float3 v)
{
	float sx, cx;
	float sy, cy;
	float sz, cz;

	sincos(v.x, sx, cx);
	sincos(v.y, sy, cy);
	sincos(v.z, sz, cz);

	float3 row1 = float3(sx*sy*sz + cy*cz, sx*sy*cz - cy*sz, cx*sy);
	float3 row3 = float3(sx*cy*sz - sy*cz, sx*cy*cz + sy*sz, cx*cy);
	float3 row2 = float3(cx*sz, cx*cz, -sx);

	return float3x3(row1, row2, row3);
}
   
struct Input {
	float vface : VFACE;
};

float _Area;
float _Scale;
float4x4 _LocalToWorld;
float4x4 _WorldToLocal;
int _Shape;

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

void setup()
{
	#define unity_ObjectToWorld unity_ObjectToWorld
	#define unity_WorldToObject unity_WorldToObject

	uint id = unity_InstanceID;
	uint seed = id * 6;

	float2 pos = float2(Random(seed), Random(seed + 1));
	if(_Shape == 1)
	{
		float a = Random(seed) * CUSTOM_PI * 360;
		float distance = Random(seed+2);
		pos = float2(sin(a) * distance * _Area / 2, cos(a) * distance * _Area / 2);
	}
	else 
	{
		pos = (pos - 0.5)  * _Area;		
	}

	float ry = Random(seed + 3) * CUSTOM_PI * 2;
	float rx = (Random(seed + 4) - 0.5) * 0.8;
	float rz = (Random(seed + 5) - 0.5) * 0.8;

	float3x3 rot = Euler3x3(float3(-90 + rx, ry, rz));

	float scale = _Scale * (Random(seed + 6) + 0.5);

	float3x3 R = rot * scale;

	float4x4 o2w = float4x4(
		R._11, R._12, R._13, pos.x,
		R._21, R._22, R._23, 0,
		R._31, R._32, R._33, pos.y,
		0, 0, 0, 1
	);

	R = rot / scale;

	float4x4 w2o = float4x4(
		R._11, R._21, R._31, -pos.x,
		R._12, R._22, R._32, 0,
		R._13, R._23, R._33, -pos.y,
		0, 0, 0, 1
	);

	unity_ObjectToWorld = mul(_LocalToWorld, o2w);
	unity_WorldToObject = mul(w2o, _WorldToLocal);

}

#endif