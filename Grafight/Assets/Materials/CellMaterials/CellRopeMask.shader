// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Cell/CellRopeMask"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Float0("Float 0", Range( 0 , 1)) = 0.1826724
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float _Float0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord16 = i.uv_texcoord * float2( 0.1,0.1 ) + float2( 0,0 );
			float4 tex2DNode1 = tex2D( _Albedo, uv_TexCoord16 );
			o.Albedo = tex2DNode1.rgb;
			o.Alpha = _Float0;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			# include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float4 texcoords01 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.texcoords01 = float4( v.texcoord.xy, v.texcoord1.xy );
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord.xy = IN.texcoords01.xy;
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13801
131;93;1506;867;1760.512;481.0469;1.364677;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-1198.635,-356.1379;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.1,0.1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DotProductOpNode;21;-918.2484,159.192;Float;False;2;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-770.0863,159.192;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;25;-907.5701,304.6844;Float;False;Constant;_Float1;Float 1;4;0;0.5;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;33;-189.7689,-360.9554;Float;False;Constant;_Color0;Color 0;2;0;0.278,0.5294118,0.5294118,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.ColorNode;29;-719.2633,371.876;Float;False;Constant;_Color1;Color 1;3;0;0.5514706,0.5514706,0.5514706,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-373.3834,289.972;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0,0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-536.4973,169.8703;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-432.6811,-41.62103;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.0;False;1;COLOR
Node;AmplifyShaderEditor.RangedFloatNode;14;-440.3915,440.8341;Float;False;Property;_Float0;Float 0;1;0;0.1826724;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.ScreenColorNode;31;-704.2526,-75.73798;Float;False;Global;_GrabScreen0;Grab Screen 0;2;0;Object;-1;False;1;0;FLOAT2;0,0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RotatorNode;15;-982.7469,-71.70721;Float;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;0.1;False;1;FLOAT2
Node;AmplifyShaderEditor.WorldNormalVector;20;-1207.899,112.4741;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;22;-1193.216,330.0455;Float;False;1;0;FLOAT;0.0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-641.7009,-304.4949;Float;True;Property;_Albedo;Albedo;0;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Cell/CellRopeMask;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;3;False;0;0;Transparent;0.5;True;True;0;True;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0.05;0.7321583,0.7679338,0.8161765,0.475;VertexScale;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;20;0
WireConnection;21;1;22;0
WireConnection;23;0;21;0
WireConnection;23;1;25;0
WireConnection;26;1;24;0
WireConnection;24;0;23;0
WireConnection;24;1;25;0
WireConnection;34;0;1;0
WireConnection;34;1;31;0
WireConnection;15;0;16;0
WireConnection;1;1;16;0
WireConnection;0;0;1;0
WireConnection;0;9;14;0
ASEEND*/
//CHKSM=C5B3040DAC1F92F689156E0E2EFA6317AC4C1AC0