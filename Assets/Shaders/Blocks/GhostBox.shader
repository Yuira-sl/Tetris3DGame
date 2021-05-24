Shader "Unlit/GhostBox"
{
	Properties {
		_MainColor ("Color", Color) = (1,1,1,1)
		_HighlightColor("HighlightColor", Color) = (0,0,1,1)
		_EdgePow("Threshold", Range(0 , 5)) = 0.5
		_RimNum("Rim", Range(0 , 0.1)) = 0.06
	}

	SubShader {

		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderPipeline" = "UniversalRenderPipeline" "RenderType"="Transparent" "DisableBatching"="True"}
		
		Pass{
			Tags { "LightMode"="UniversalForward" }	
			
			Blend One One
			ZWrite Off
			Cull Off
			
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fwdbase

			float4 _MainColor;
			float4 _HighlightColor;
			sampler2D _CameraDepthTexture;
			float _EdgePow;
			sampler2D _MaskTex;
			float _RimNum;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				float2 uv: TEXCOORD0;
			};

			struct v2f
			{
				float4 pos: POSITION;
				float4 scrPos: TEXCOORD0;
				half3 worldNormal: TEXCOORD1;
				half3 worldViewDir: TEXCOORD2;
				float2 uv: TEXCOORD3;
			};

			v2f vert (appdata v )
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.pos);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; 
				
				o.worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				o.worldNormal = UnityObjectToWorldNormal(v.normal); 

				o.uv = v.uv;

				COMPUTE_EYEDEPTH(o.scrPos.z);
				return o;
			}
			
			half4 frag ( v2f i ) : SV_TARGET
			{
				half4 finalColor = _MainColor;
				
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
				float partZ = i.scrPos.z;

				float diff = 1-saturate((sceneZ-i.scrPos.z)*4 - _EdgePow);
				half rim = pow(1 - abs(dot(normalize(i.worldNormal), normalize(i.worldViewDir))), _RimNum);

				finalColor = lerp(finalColor, _HighlightColor, diff);
				finalColor = lerp(finalColor, _HighlightColor, rim);
				return finalColor;
			}

			ENDCG
		}
	}
}