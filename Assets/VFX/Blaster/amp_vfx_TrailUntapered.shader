// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "amp_vfx_TrailUntapered"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_MainTexture("Main Texture", 2D) = "white" {}
		[HDR]_Color1("Color 1", Color) = (0.8113208,0.06505875,0.08697848,0)
		[HDR]_Color2("Color 2", Color) = (0,0.2021527,1,0)
		_TexturePanDir("TexturePanDir", Vector) = (-1,0,0,0)
		_DissolvePanDir("DissolvePanDir", Vector) = (-1,0,0,0)
		_DissolveNoise("Dissolve Noise", 2D) = "white" {}
		_TexturePanSpeed("TexturePanSpeed", Range( 0 , 10)) = 1.395719
		_DissolvePanSpeed("DissolvePanSpeed", Range( 0 , 10)) = 0.6351369
		_NoiseSaturation("NoiseSaturation", Range( 0 , 1)) = 0.5
		_TextureSaturation("Texture Saturation", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "AlphaTest+0" "IsEmissive" = "true"  }
		Cull Off
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _NoiseSaturation;
		uniform sampler2D _DissolveNoise;
		uniform float _DissolvePanSpeed;
		uniform float2 _DissolvePanDir;
		uniform float _TextureSaturation;
		uniform sampler2D _MainTexture;
		uniform float _TexturePanSpeed;
		uniform float2 _TexturePanDir;
		uniform float4 _Color1;
		uniform float4 _Color2;
		uniform float _Cutoff = 0.5;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 panner37 = ( ( _DissolvePanSpeed * _Time.y ) * _DissolvePanDir + i.uv_texcoord);
			float2 DissolvePanningUV43 = panner37;
			float2 panner29 = ( ( _TexturePanSpeed * _Time.y ) * _TexturePanDir + i.uv_texcoord);
			float2 TexturePanningUV28 = panner29;
			float4 tex2DNode5 = tex2D( _MainTexture, TexturePanningUV28 );
			float4 temp_output_18_0 = ( ( _NoiseSaturation + tex2D( _DissolveNoise, DissolvePanningUV43 ) ) * ( ( _TextureSaturation * tex2DNode5 ) + tex2DNode5 ) );
			float4 lerpResult3 = lerp( _Color1 , _Color2 , i.uv_texcoord.x);
			o.Emission = ( temp_output_18_0 * lerpResult3 ).rgb;
			float4 clampResult46 = clamp( temp_output_18_0 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Alpha = clampResult46.r;
			clip( clampResult46.r - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit keepalpha fullforwardshadows 

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
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
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
Version=18921
124;906;1777;367;4666.378;775.4685;3.090928;True;False
Node;AmplifyShaderEditor.CommentaryNode;36;-3289.181,-520.2293;Inherit;False;1021.01;514.3773;;7;29;31;32;34;30;33;28;Main Texture Panning;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;44;-3296.787,-1263.943;Inherit;False;1027.398;514.3768;Comment;7;37;38;39;42;41;40;43;Dissolve Texture Panning;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;31;-3141.155,-116.852;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-3239.181,-199.7013;Inherit;False;Property;_TexturePanSpeed;TexturePanSpeed;7;0;Create;True;0;0;0;False;0;False;1.395719;3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-2947.155,-174.852;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;34;-3075.693,-470.2293;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;30;-3042.336,-337.4344;Inherit;False;Property;_TexturePanDir;TexturePanDir;4;0;Create;True;0;0;0;False;0;False;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;38;-3148.761,-860.5659;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-3246.787,-943.4152;Inherit;False;Property;_DissolvePanSpeed;DissolvePanSpeed;8;0;Create;True;0;0;0;False;0;False;0.6351369;3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;40;-3049.942,-1081.147;Inherit;False;Property;_DissolvePanDir;DissolvePanDir;5;0;Create;True;0;0;0;False;0;False;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-3083.298,-1213.943;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2954.761,-918.5659;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;29;-2773.652,-352.7121;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.81,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;26;-1930.31,-512.825;Inherit;False;1226.766;528.5471;Comment;6;51;50;18;5;35;49;Main Texture;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-2514.172,-356.7951;Inherit;False;TexturePanningUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;37;-2781.258,-1096.425;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.81,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-1893.189,-289.3528;Inherit;False;28;TexturePanningUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;25;-2189.541,-1296.694;Inherit;False;1256.848;632.0203;Comment;4;19;45;47;48;Noise Dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;-2519.389,-1099.779;Inherit;False;DissolvePanningUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;45;-2043.907,-990.6973;Inherit;False;43;DissolvePanningUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1635.763,-423.9589;Inherit;False;Property;_TextureSaturation;Texture Saturation;10;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;5;-1633.645,-294.8397;Inherit;True;Property;_MainTexture;Main Texture;1;0;Create;True;0;0;0;False;0;False;-1;a49d507d1c8ab3444863e5c2327c79b2;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;-1779.817,-1111.792;Inherit;False;Property;_NoiseSaturation;NoiseSaturation;9;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;27;-1928.002,101.0149;Inherit;False;600.4432;588.4158;Comment;4;2;4;1;3;Color Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;19;-1772.348,-1014.142;Inherit;True;Property;_DissolveNoise;Dissolve Noise;6;0;Create;True;0;0;0;False;0;False;-1;f60bb5e8c69bd85438b0f12860947c14;5690b7d7102897146bb4cdcff5fb2b8e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1273.637,-378.8951;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-1315.957,-1024.458;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;2;-1875.491,352.6154;Inherit;False;Property;_Color2;Color 2;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0.2021527,1,0;0,0.2021527,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;1;-1875.489,151.0148;Inherit;False;Property;_Color1;Color 1;2;1;[HDR];Create;True;0;0;0;False;0;False;0.8113208,0.06505875,0.08697848,0;0.8113208,0.06505875,0.08697848,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1878.002,530.4307;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;50;-1039.117,-314.3973;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;3;-1592.559,153.775;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-906.121,-357.0775;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;-390.282,119.6986;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;46;-466.561,-343.8171;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;5.428769,-233.437;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;amp_vfx_TrailUntapered;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;AlphaTest;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;2;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;33;0
WireConnection;32;1;31;0
WireConnection;39;0;41;0
WireConnection;39;1;38;0
WireConnection;29;0;34;0
WireConnection;29;2;30;0
WireConnection;29;1;32;0
WireConnection;28;0;29;0
WireConnection;37;0;42;0
WireConnection;37;2;40;0
WireConnection;37;1;39;0
WireConnection;43;0;37;0
WireConnection;5;1;35;0
WireConnection;19;1;45;0
WireConnection;51;0;49;0
WireConnection;51;1;5;0
WireConnection;47;0;48;0
WireConnection;47;1;19;0
WireConnection;50;0;51;0
WireConnection;50;1;5;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;3;2;4;1
WireConnection;18;0;47;0
WireConnection;18;1;50;0
WireConnection;6;0;18;0
WireConnection;6;1;3;0
WireConnection;46;0;18;0
WireConnection;0;2;6;0
WireConnection;0;9;46;0
WireConnection;0;10;46;0
ASEEND*/
//CHKSM=4FA56155A0868C4FC36A552FFED5CD08C082A59D