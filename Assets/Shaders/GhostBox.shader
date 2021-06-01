Shader "Unlit/GhostBox"
{
	Properties 
	{
		_MainTex ("MainTex", 2D) = "white" {}
		_MaskTex ("MaskTex", 2D) = "white" {}
		_MainColor ("Color", Color) = (1,1,1,1)
		_HighlightColor("HighlightColor", Color) = (0,0,1,1)
		_EdgePow("Threshold", Range(0 , 5)) = 0.5
		_RimNum("Rim", Range(0 , 1)) = 0.06
	}

	SubShader 
	{
		Tags 
		{
			"Queue"="Transparent" 
			"RenderPipeline" = "UniversalRenderPipeline" 
			"RenderType"="Transparent"
		}
		
		Pass
		{
			Tags { "LightMode"="UniversalForward" }	
			
			Blend One One
			ZWrite Off
			Cull Off
			
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _MainColor,_HighlightColor;
			uniform float _EdgePow, _RimNum;
			uniform sampler2D _MainTex, _MaskTex, _CameraDepthTexture;

			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal: NORMAL;
				float2 uv: TEXCOORD0;
			};

			struct v2f
			{
				float4 pos: SV_POSITION;
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
				half4 col = tex2D(_MainTex, i.uv);
				half mask = tex2D(_MaskTex, i.uv).r;
				
				col *= (1 - mask);

				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.scrPos)));
				float partZ = i.scrPos.z;

				half rim = pow(1 - abs(dot(normalize(i.worldNormal), normalize(i.worldViewDir))), _RimNum);

				col = lerp(col * rim, _HighlightColor, rim);
				return col * _MainColor * _EdgePow;
			}

			ENDCG
		}
	}
}