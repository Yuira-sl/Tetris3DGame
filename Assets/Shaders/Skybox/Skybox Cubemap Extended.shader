Shader "Skybox/Cubemap Extended"
{
	Properties
	{
		[HideInInspector]_IsStandardPipeline("_IsStandardPipeline", Float) = 1
		[StyledBanner(Skybox Cubemap Extended)]_SkyboxExtended("< SkyboxExtended >", Float) = 1
		[StyledCategory(Cubemap, 5, 10)]_Cubemapp("[ Cubemapp ]", Float) = 1
		[NoScaleOffset]_Tex("Cubemap (HDR)", CUBE) = "black" {}
		_Exposure("Cubemap Exposure", Range( 0 , 8)) = 1
		[Gamma]_TintColor("Cubemap Tint Color", Color) = (0.5,0.5,0.5,1)
		_CubemapPosition("Cubemap Position", Float) = 0
		[StyledCategory(Rotation)]_Rotationn("[ Rotationn ]", Float) = 1
		[Toggle(_ENABLEROTATION_ON)] _EnableRotation("Enable Rotation", Float) = 0
		[IntRange][Space(10)]_Rotation("Rotation", Range( 0 , 360)) = 0
		
		_RotationY("RotationY", Range(0 , 3.14)) = 3.14
		_RotationSpeed("Rotation Speed", Float) = 1
		[StyledCategory(Fog)]_Fogg("[ Fogg ]", Float) = 1
		[Toggle(_ENABLEFOG_ON)] _EnableFog("Enable Fog", Float) = 0
		[StyledMessage(Info, The fog color is controlled by the fog color set in the Lighting panel., _EnableFog, 1, 10, 0)]_FogMessage("# FogMessage", Float) = 0
		[Space(10)]_FogIntensity("Fog Intensity", Range( 0 , 1)) = 1
		_FogHeight("Fog Height", Range( 0 , 1)) = 1
		_FogSmoothness("Fog Smoothness", Range( 0.01 , 1)) = 0.01
		_FogFill("Fog Fill", Range( 0 , 1)) = 0.5
		[HideInInspector]_Tex_HDR("DecodeInstructions", Vector) = (0,0,0,0)
		[ASEEnd]_FogPosition("Fog Position", Float) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Background" "Queue"="Background" }
		LOD 0

		CGINCLUDE
		#pragma target 2.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
				//only defining to not throw compilation error over Unity 5.5
				#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _ENABLEFOG_ON
			#pragma shader_feature_local _ENABLEROTATION_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _SkyboxExtended;
			uniform half _FogMessage;
			uniform half _IsStandardPipeline;
			uniform half4 _Tex_HDR;
			uniform half _Cubemapp;
			uniform half _Fogg;
			uniform half _Rotationn;
			uniform samplerCUBE _Tex;
			uniform float _CubemapPosition;
			uniform half _Rotation, _RotationY;
			uniform half _RotationSpeed;
			uniform half4 _TintColor;
			uniform half _Exposure;
			uniform float _FogPosition;
			uniform half _FogHeight;
			uniform half _FogSmoothness;
			uniform half _FogFill;
			uniform half _FogIntensity;
			
			inline half3 DecodeHDR1189( float4 Data )
			{
				return DecodeHDR(Data, _Tex_HDR);
			}
						
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float lerpResult268 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
				half CAMERA_MODE300 = lerpResult268;
				float3 appendResult1220 = (float3(v.vertex.xyz.x , ( v.vertex.xyz.y * CAMERA_MODE300 ) , v.vertex.xyz.z));
				float3 appendResult1208 = (float3(0.0 , -_CubemapPosition , 0.0));
				half3 VertexPos40_g1 = appendResult1220;
				float3 appendResult74_g1 = (float3(0.0 , VertexPos40_g1.y , 0.0));
				float3 VertexPosRotationAxis50_g1 = appendResult74_g1;
				float3 break84_g1 = VertexPos40_g1;
				float3 appendResult81_g1 = (float3(break84_g1.x , cos(_RotationY) , break84_g1.z));
				float3 VertexPosOtherAxis82_g1 = appendResult81_g1;
				half Angle44_g1 = ( 1.0 - radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) );
				#ifdef _ENABLEROTATION_ON
					float3 staticSwitch1164 = ( ( VertexPosRotationAxis50_g1 + ( VertexPosOtherAxis82_g1 * cos( Angle44_g1 ) ) + ( cross( float3(0,1,0) , VertexPosOtherAxis82_g1 ) * sin( Angle44_g1 ) ) ) + appendResult1208 );
				#else
					float3 staticSwitch1164 = ( appendResult1220 + appendResult1208 );
				#endif
				float3 vertexToFrag774 = staticSwitch1164;
				o.ase_texcoord1.xyz = vertexToFrag774;
				
				o.ase_texcoord2 = v.vertex;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
					vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
					float3 WorldPosition = i.worldPos;
				#endif
				float3 vertexToFrag774 = i.ase_texcoord1.xyz;
		
				half4 Data1189 = texCUBE( _Tex, vertexToFrag774 );
				half3 localDecodeHDR1189 = DecodeHDR1189( Data1189 );
				half4 CUBEMAP222 = ( float4( localDecodeHDR1189 , 0.0 ) * unity_ColorSpaceDouble * _TintColor * _Exposure );
				float lerpResult678 = lerp( saturate( pow( (0.0 + (abs( ( i.ase_texcoord2.xyz.y + -_FogPosition ) ) - 0.0) * (1.0 - 0.0) / (_FogHeight - 0.0)) , ( 1.0 - _FogSmoothness ) ) ) , 0.0 , _FogFill);
				float lerpResult1205 = lerp( 1.0 , lerpResult678 , _FogIntensity);
				half FOG_MASK359 = lerpResult1205;
				float4 lerpResult317 = lerp( unity_FogColor , CUBEMAP222 , FOG_MASK359);
				#ifdef _ENABLEFOG_ON
					float4 staticSwitch1179 = lerpResult317;
				#else
					float4 staticSwitch1179 = CUBEMAP222;
				#endif
				
				
				finalColor = staticSwitch1179;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "SkyboxExtendedShaderGUI"
	Fallback "Skybox/Cubemap"
}