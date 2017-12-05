// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyExample/BurningShader1"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Normals("Normals", 2D) = "bump" {}
		_Mask("Mask", 2D) = "white" {}
		_Spec("Spec", 2D) = "white" {}
		_WoodFireTexture("WoodFireTexture", 2D) = "white" {}
		_FireIntensity("FireIntensity", Range( 0 , 2)) = 0.374282
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_FireSpeed("FireSpeed", Range( 0 , 25)) = 0
		_Opacity("Opacity", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normals;
		uniform float4 _Normals_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _Mask;
		uniform float4 _Mask_ST;
		uniform sampler2D _WoodFireTexture;
		uniform float _FireSpeed;
		uniform float _FireIntensity;
		uniform sampler2D _Spec;
		uniform float4 _Spec_ST;
		uniform float _Smoothness;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normals, uv_Normals ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = ( tex2D( _Albedo, uv_Albedo ) * float4( 0,0,0,0 ) ).rgb;
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			float2 temp_cast_1 = (( _Time.x * _FireSpeed )).xx;
			float2 uv_TexCoord8 = i.uv_texcoord * float2( 1,1 ) + float2( 0,0 );
			float2 panner7 = ( uv_TexCoord8 + 1.0 * _Time.y * temp_cast_1);
			o.Emission = ( ( tex2D( _Mask, uv_Mask ) * tex2D( _WoodFireTexture, panner7 ) ) * ( _FireIntensity * ( _SinTime.w + 1.5 ) ) ).rgb;
			float2 uv_Spec = i.uv_texcoord * _Spec_ST.xy + _Spec_ST.zw;
			o.Metallic = tex2D( _Spec, uv_Spec ).r;
			o.Smoothness = _Smoothness;
			o.Alpha = _Opacity;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

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
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
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
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
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
1706;104;1839;958;1650.762;401.5713;1.574498;True;False
Node;AmplifyShaderEditor.TimeNode;9;-1118.32,620.5584;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;18;-956.2141,962.5571;Float;False;Property;_FireSpeed;FireSpeed;8;0;0;0;25;0;1;FLOAT
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-939.6387,417.3337;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-805.8735,654.7175;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.PannerNode;7;-670.6387,533.3337;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2
Node;AmplifyShaderEditor.SinTimeNode;13;81.31544,790.1285;Float;False;0;5;FLOAT4;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;3;-344.3198,490.6458;Float;True;Property;_Mask;Mask;3;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/SceneTextures/emiss_2.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;5;-312.0533,714.8657;Float;True;Property;_WoodFireTexture;WoodFireTexture;5;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/Misc/TileableFire.jpg;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;11;162.7707,538.8146;Float;False;Property;_FireIntensity;FireIntensity;6;0;0.374282;0;2;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleAddOpNode;12;370.6688,815.719;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;1.5;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;39.73854,444.6898;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;503.0132,527.3564;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-359.5,-126.5;Float;True;Property;_Albedo;Albedo;0;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/SceneTextures/WoodDiffuse.tif;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;1879.778,533.7338;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;2;-356.5,97.5;Float;True;Property;_Normals;Normals;2;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/SceneTextures/WoodNormal.tif;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;23;608.6428,484.8712;Float;False;Property;_Opacity;Opacity;9;0;1;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;612.8577,328.5961;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;474.8884,-47.31805;Float;False;2;2;0;COLOR;0.0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR
Node;AmplifyShaderEditor.SamplerNode;4;107.4002,195.8551;Float;True;Property;_Spec;Spec;4;0;Assets/AmplifyShaderEditor/Examples/Assets/Textures/SceneTextures/WoodSpecular.tif;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;16;520.5857,240.9155;Float;False;Property;_Smoothness;Smoothness;7;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;854.678,57.55034;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;AmplifyExample/BurningShader1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Custom;0.5;True;True;0;False;Transparent;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;1;1;-1;-1;0;0;0;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;17;0;9;1
WireConnection;17;1;18;0
WireConnection;7;0;8;0
WireConnection;7;2;17;0
WireConnection;5;1;7;0
WireConnection;12;0;13;4
WireConnection;6;0;3;0
WireConnection;6;1;5;0
WireConnection;14;0;11;0
WireConnection;14;1;12;0
WireConnection;10;0;6;0
WireConnection;10;1;14;0
WireConnection;20;0;1;0
WireConnection;0;0;20;0
WireConnection;0;1;2;0
WireConnection;0;2;10;0
WireConnection;0;3;4;0
WireConnection;0;4;16;0
WireConnection;0;9;23;0
ASEEND*/
//CHKSM=207FEB4D28818554AC58F184BC66250A4BEE4211