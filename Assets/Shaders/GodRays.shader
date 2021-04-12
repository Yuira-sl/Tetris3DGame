Shader "Unlit/GodRays"
{
    Properties
    {
        _Tint("Tint", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _AnimationRate("Rate", Vector) = (1,1,0,0)
        _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha One
        Cull Off Lighting Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #pragma multi_compile_particles

            uniform sampler2D _MainTex;
            uniform half4 _AnimationRate;
            uniform half4 _Tint;
            uniform half _InvFade;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD1;
                #endif
            };

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos (o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif
                o.color = v.color;
                v.uv += _AnimationRate.xy *_Time.y;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate (_InvFade * (sceneZ-partZ));
                i.color.a *= fade;
                #endif
                
                half4 col = i.color * tex2D(_MainTex, i.uv);
                col.rgb *= col.a;

                return col * _Tint;
            }
            ENDCG
        }
    }
}
