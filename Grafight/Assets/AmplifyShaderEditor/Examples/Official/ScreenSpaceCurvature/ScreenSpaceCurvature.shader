// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ASESampleShaders/ScreenSpaceCurvature"
{
	Properties
	{
		_ScaleFactor("ScaleFactor", Float) = 4
		_Tint("Tint", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
		};

		uniform float4 _Tint;
		uniform float _ScaleFactor;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = _Tint.rgb;
			float3 temp_output_2_0 = ddx( i.worldNormal );
			float3 temp_output_7_0 = ddy( i.worldNormal );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			o.Emission = ( ( ( ( cross( ( i.worldNormal - temp_output_2_0 ) , ( i.worldNormal + temp_output_2_0 ) ).y - cross( ( i.worldNormal - temp_output_7_0 ) , ( i.worldNormal + temp_output_7_0 ) ).x ) * _ScaleFactor ) / length( ase_vertex3Pos ) ) + float3(0.5,0.5,0.5) );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

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
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD6;
				float3 worldNormal : TEXCOORD1;
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
				o.worldNormal = worldNormal;
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
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
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
1706;104;1839;958;824.7046;400.3994;1.3;True;False
Node;AmplifyShaderEditor.WorldNormalVector;33;-1093.197,86.89935;Float;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.DdyOpNode;7;-820.8997,285.6999;Float;False;1;0;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.DdxOpNode;2;-835.1998,-79.59997;Float;False;1;0;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-573.0988,-142.6001;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-536.899,289.1003;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleSubtractOpNode;19;-531.6992,126.5998;Float;False;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;18;-575.8983,-12.50004;Float;False;2;2;0;FLOAT3;0.0,0,0;False;1;FLOAT3;0.0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.CrossProductOpNode;21;-373.0967,-141.1999;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.CrossProductOpNode;22;-375.2969,147;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3
Node;AmplifyShaderEditor.BreakToComponentsNode;23;-206.0973,-189.2997;Float;False;FLOAT3;1;0;FLOAT3;0.0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.BreakToComponentsNode;24;-212.5965,203.3003;Float;False;FLOAT3;1;0;FLOAT3;0.0,0,0;False;16;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;27;19.00231,144.3006;Float;False;Property;_ScaleFactor;ScaleFactor;0;0;4;0;0;0;1;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;14;-108.1999,367.4005;Float;False;0;0;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleSubtractOpNode;25;8.402822,-21.59954;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;173.5023,10.90066;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.LengthOpNode;16;92.40041,431.3006;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT
Node;AmplifyShaderEditor.Vector3Node;30;288.9025,325.6991;Float;False;Constant;_Vector0;Vector 0;-1;0;0.5,0.5,0.5;0;4;FLOAT3;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleDivideOpNode;28;305.8015,139.5998;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.ColorNode;34;401.1954,-80.5995;Float;False;Property;_Tint;Tint;1;0;0,0,0,0;0;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;29;479.1028,217.2994;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;678.9996,117.8999;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ASESampleShaders/ScreenSpaceCurvature;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;3;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;7;0;33;0
WireConnection;2;0;33;0
WireConnection;17;0;33;0
WireConnection;17;1;2;0
WireConnection;20;0;33;0
WireConnection;20;1;7;0
WireConnection;19;0;33;0
WireConnection;19;1;7;0
WireConnection;18;0;33;0
WireConnection;18;1;2;0
WireConnection;21;0;17;0
WireConnection;21;1;18;0
WireConnection;22;0;19;0
WireConnection;22;1;20;0
WireConnection;23;0;21;0
WireConnection;24;0;22;0
WireConnection;25;0;23;1
WireConnection;25;1;24;0
WireConnection;26;0;25;0
WireConnection;26;1;27;0
WireConnection;16;0;14;0
WireConnection;28;0;26;0
WireConnection;28;1;16;0
WireConnection;29;0;28;0
WireConnection;29;1;30;0
WireConnection;0;0;34;0
WireConnection;0;2;29;0
ASEEND*/
//CHKSM=67422282B1E766E645F2E366CBF259FA214AA5C3