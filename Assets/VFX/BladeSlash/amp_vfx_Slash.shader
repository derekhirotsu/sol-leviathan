// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "amp_vfx_Slash"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		_MainTexture("Main Texture", 2D) = "white" {}
		_Opacity("Opacity", Float) = 20
		_DissolveNoise("DissolveNoise", 2D) = "white" {}
		_NoiseIntensity("NoiseIntensity", Range( 0 , 1)) = 1
		_PanningUVSpeed("Panning UV Speed", Vector) = (0,0,0,0)
		_EmissionTexture("Emission Texture", 2D) = "white" {}
		_EmissionIntensity("Emission Intensity", Float) = 4
		_SlashColor("Slash Color", Color) = (0,0,0,0)
		[ASEEnd]_EmissionSaturation("Emission Saturation", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}


	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				
				#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
				#endif
				
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_FRAG_COLOR


				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float4 ase_texcoord1 : TEXCOORD1;
				};

				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					float4 ase_texcoord3 : TEXCOORD3;
				};
				
				
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif

				//Don't delete this comment
				// uniform sampler2D_float _CameraDepthTexture;

				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float4 _SlashColor;
				uniform sampler2D _EmissionTexture;
				uniform float4 _EmissionTexture_ST;
				uniform float _EmissionSaturation;
				uniform float _EmissionIntensity;
				uniform sampler2D _MainTexture;
				uniform float4 _MainTexture_ST;
				uniform float _Opacity;
				uniform sampler2D _DissolveNoise;
				uniform float4 _PanningUVSpeed;
				uniform float4 _DissolveNoise_ST;
				uniform float _NoiseIntensity;
				float3 HSVToRGB( float3 c )
				{
					float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
					float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
					return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
				}
				
				float3 RGBToHSV(float3 c)
				{
					float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
					float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
					float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
					float d = q.x - min( q.w, q.y );
					float e = 1.0e-10;
					return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
				}


				v2f vert ( appdata_t v  )
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					o.ase_texcoord3 = v.ase_texcoord1;

					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					UNITY_SETUP_INSTANCE_ID( i );
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( i );

					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif

					float2 uv_EmissionTexture = i.texcoord.xy * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
					float3 hsvTorgb46 = RGBToHSV( tex2D( _EmissionTexture, uv_EmissionTexture ).rgb );
					float4 texCoord35 = i.ase_texcoord3;
					texCoord35.xy = i.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
					float3 hsvTorgb47 = HSVToRGB( float3(( 0.0 + hsvTorgb46.x + texCoord35.z ),hsvTorgb46.y,hsvTorgb46.z) );
					float3 desaturateInitialColor49 = hsvTorgb47;
					float desaturateDot49 = dot( desaturateInitialColor49, float3( 0.299, 0.587, 0.114 ));
					float3 desaturateVar49 = lerp( desaturateInitialColor49, desaturateDot49.xxx, _EmissionSaturation );
					float4 _Vector1 = float4(-0.3,1,-2,1);
					float3 temp_cast_1 = (_Vector1.x).xxx;
					float3 temp_cast_2 = (_Vector1.y).xxx;
					float3 temp_cast_3 = (_Vector1.z).xxx;
					float3 temp_cast_4 = (_Vector1.w).xxx;
					float3 clampResult39 = clamp( (temp_cast_3 + (desaturateVar49 - temp_cast_1) * (temp_cast_4 - temp_cast_3) / (temp_cast_2 - temp_cast_1)) , float3( 0,0,0 ) , float3( 1,1,1 ) );
					float2 uv_MainTexture = i.texcoord.xy * _MainTexture_ST.xy + _MainTexture_ST.zw;
					float clampResult5 = clamp( ( tex2D( _MainTexture, uv_MainTexture ).a * _Opacity ) , 0.0 , 1.0 );
					float2 appendResult24 = (float2(_PanningUVSpeed.z , _PanningUVSpeed.w));
					float4 uvs4_DissolveNoise = i.texcoord;
					uvs4_DissolveNoise.xy = i.texcoord.xy * _DissolveNoise_ST.xy + _DissolveNoise_ST.zw;
					float2 panner25 = ( 1.0 * _Time.y * appendResult24 + uvs4_DissolveNoise.xy);
					float2 break31 = panner25;
					float2 appendResult32 = (float2(break31.x , ( texCoord35.w + break31.y )));
					float TexCoord_T18 = uvs4_DissolveNoise.w;
					float TexCoord_W17 = uvs4_DissolveNoise.z;
					float3 _Vector0 = float3(0.3,0,1);
					float ifLocalVar13 = 0;
					if( ( ( tex2D( _DissolveNoise, appendResult32 ).r * _NoiseIntensity ) * TexCoord_T18 ) >= TexCoord_W17 )
					ifLocalVar13 = _Vector0.y;
					else
					ifLocalVar13 = _Vector0.z;
					float4 appendResult6 = (float4(( ( i.color * _SlashColor ) + ( float4( clampResult39 , 0.0 ) * _EmissionIntensity * i.color ) ).rgb , ( i.color.a * clampResult5 * ifLocalVar13 )));
					

					fixed4 col = appendResult6;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
	
	
	
}
/*ASEBEGIN
Version=18921
60;902;1988;418;6559.566;1252.001;5.150014;True;False
Node;AmplifyShaderEditor.CommentaryNode;21;-2938.283,-30.54104;Inherit;False;966.6559;478.0977;We can set these within the Particle System using the CustomData module.;7;17;18;16;23;24;22;25;Dissolve Parameters;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;22;-2885.663,102.1237;Inherit;False;Property;_PanningUVSpeed;Panning UV Speed;4;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;51;-2037.552,-634.2104;Inherit;False;1217.893;353.2383;;6;46;48;47;50;36;49;Emission Texture Mapping;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;16;-2618.589,141.3903;Inherit;False;0;10;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;24;-2628.188,318.9105;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;25;-2247.651,317.8296;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;34;-1901.557,-28.96834;Inherit;False;586.7402;480.2123;Comment;4;33;32;31;35;Random Texture Position;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;36;-1987.551,-584.2104;Inherit;True;Property;_EmissionTexture;Emission Texture;5;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;31;-1843.86,234.9367;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TextureCoordinatesNode;35;-1845.822,50.23569;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RGBToHSVNode;46;-1650.358,-577.9098;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-1427.612,-442.858;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-1651.877,328.5592;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-1517.381,233.4725;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.HSVToRGBNode;47;-1292.947,-563.3883;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;50;-1283.169,-396.9706;Inherit;False;Property;_EmissionSaturation;Emission Saturation;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;15;-1207.133,153.3912;Inherit;False;1000.016;553.8313;Used to dissolve the main texture based on a noise texture. The values are set in the Particle System.;8;13;14;19;12;20;10;28;30;Dissolve Conditional;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;10;-1172.733,204.6912;Inherit;True;Property;_DissolveNoise;DissolveNoise;2;0;Create;True;0;0;0;False;0;False;-1;f60bb5e8c69bd85438b0f12860947c14;f60bb5e8c69bd85438b0f12860947c14;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;28;-1154.361,404.834;Inherit;False;Property;_NoiseIntensity;NoiseIntensity;3;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-2250.091,228.7972;Inherit;False;TexCoord_T;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;49;-1019.659,-561.8793;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;38;-656.0977,-466.9498;Inherit;False;Constant;_Vector1;Vector 1;6;0;Create;True;0;0;0;False;0;False;-0.3,1,-2,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-784.1295,-228.7506;Inherit;True;Property;_MainTexture;Main Texture;0;0;Create;True;0;0;0;False;0;False;-1;6bb398e7c910eaf4a93e4828655d673a;6bb398e7c910eaf4a93e4828655d673a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-2247.544,136.1684;Inherit;False;TexCoord_W;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;20;-862.6653,320.8628;Inherit;False;18;TexCoord_T;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;37;-440.9858,-539.5417;Inherit;False;5;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,1;False;3;FLOAT3;0,0,0;False;4;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-878.5762,214.5049;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-659.1295,-0.7506981;Inherit;False;Property;_Opacity;Opacity;1;0;Create;True;0;0;0;False;0;False;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;14;-736.4497,525.2629;Inherit;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;0;False;0;False;0.3,0,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;41;-252.2457,-379.3466;Inherit;False;Property;_EmissionIntensity;Emission Intensity;6;0;Create;True;0;0;0;False;0;False;4;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;42;-239.3185,-718.0209;Inherit;False;Property;_SlashColor;Slash Color;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;8;-437.5583,-256.2474;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;45;-203.2784,-900.4097;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;3;-450.1303,-59.95544;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-659.3267,229.7656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-758.6694,437.8621;Inherit;False;17;TexCoord_W;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;39;-196.3319,-501.8507;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;5;-293.1987,-60.81895;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;13;-504.2848,229.6722;Inherit;True;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-9.431364,-406.4605;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;82.48901,-740.7532;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;43;261.1843,-499.8669;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-108.0092,-59.32688;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;445.4113,-261.6992;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;23;-2623.188,42.9115;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;586.4099,-257.6215;Float;False;True;-1;2;;0;7;amp_vfx_Slash;0b6a9f8b4f707c74ca64c0be8e590de0;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;4;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;24;0;22;3
WireConnection;24;1;22;4
WireConnection;25;0;16;0
WireConnection;25;2;24;0
WireConnection;31;0;25;0
WireConnection;46;0;36;0
WireConnection;48;1;46;1
WireConnection;48;2;35;3
WireConnection;33;0;35;4
WireConnection;33;1;31;1
WireConnection;32;0;31;0
WireConnection;32;1;33;0
WireConnection;47;0;48;0
WireConnection;47;1;46;2
WireConnection;47;2;46;3
WireConnection;10;1;32;0
WireConnection;18;0;16;4
WireConnection;49;0;47;0
WireConnection;49;1;50;0
WireConnection;17;0;16;3
WireConnection;37;0;49;0
WireConnection;37;1;38;1
WireConnection;37;2;38;2
WireConnection;37;3;38;3
WireConnection;37;4;38;4
WireConnection;30;0;10;1
WireConnection;30;1;28;0
WireConnection;3;0;2;4
WireConnection;3;1;4;0
WireConnection;12;0;30;0
WireConnection;12;1;20;0
WireConnection;39;0;37;0
WireConnection;5;0;3;0
WireConnection;13;0;12;0
WireConnection;13;1;19;0
WireConnection;13;2;14;2
WireConnection;13;3;14;2
WireConnection;13;4;14;3
WireConnection;40;0;39;0
WireConnection;40;1;41;0
WireConnection;40;2;8;0
WireConnection;44;0;45;0
WireConnection;44;1;42;0
WireConnection;43;0;44;0
WireConnection;43;1;40;0
WireConnection;7;0;8;4
WireConnection;7;1;5;0
WireConnection;7;2;13;0
WireConnection;6;0;43;0
WireConnection;6;3;7;0
WireConnection;23;0;22;1
WireConnection;23;1;22;2
WireConnection;1;0;6;0
ASEEND*/
//CHKSM=94D350DC5C7D3958AD657A6C79D75AF9BCBDFE69