Shader "Unlit/Transparent_Color" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Flicker_Speed ("Flicker_Speed", Float) = 1
	_Opacity ("Opacity", Float) = 1
}
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	
CGPROGRAM
#pragma surface surf Lambert

float4 _Color;
float _Flicker_Speed;
float _Opacity;

struct Input {
	float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	float fTime = _Time.z;
	o.Albedo = _Color.rgb;
	o.Emission = _Color.rgb;
	o.Alpha = _Color.a * (sin(fTime * _Flicker_Speed) * 0.5f + 0.5f) * _Opacity;
}
ENDCG
} 
FallBack "Self-Illumin/VertexLit"
}
