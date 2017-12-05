// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/VertexDisplacement"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
		

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			struct Input {
			float2 uv_MainTex;
			};

//			float4 worldSpaceVertex = mul( unity_ObjectToWorld, objectSpaceVertex ); // object to world
//			float4 objectSpaceVertex = mul( unity_WorldToObject, worldSpaceVertex ); // world to object
			
			v2f vert (appdata v)
			{
				v2f o;
				float _Speed = 1;
				float _Amount = 2.3;
				float _Distance = 0.1f;


				v.vertex.x += cos( _Time.y * _Speed + v.vertex.z * _Amount ) * _Distance;
				UnityObjectToClipPos(v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);


				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}

	
			ENDCG
		}
	}
}
