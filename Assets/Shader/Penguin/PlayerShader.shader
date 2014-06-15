Shader "Custom/PlayerShader" {
Properties {
	_MainTex ("Base01 (RGB) Trans (A)", 2D) = "white" {}
	_MainTex_02 ("Base02 (RGB) Trans (A)", 2D) = "white" {}
	_Color ("MainColor", Color) = (1,1,1,1)
	_WingSpeed ("WingSpeed", Float) = 1
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass { 
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"


			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _MainTex_02;
			fixed4 _MainTex_ST;
			fixed4 _Color;
			float _WingSpeed;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				fixed4 col2 = tex2D(_MainTex_02, i.texcoord);
				float time = _Time.x * _WingSpeed;
				return lerp(col, col2, floor(frac(time * 100.0f) + 0.5f));
			}
		ENDCG
	}
}
}
