Shader "Unlit/Skybox"
{
    Properties {

        _Scale ("Scale", Vector) = (1,1,1,1)
        _Offset ("Offset", Vector) = (1,1,1,1)
        [Toggle] _Stars ("Stars", Int) = 1
        _StarsSize ("Stars Size", Range(0,0.1)) = 0.009
        _StarsDensity ("Stars Density", Range(0,1)) = 0.02
        _StarsHash ("Stars Hash", Vector) = (641, -113, 271, 1117)
    }

    SubShader {

        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            uniform half4 _Offset;
            uniform half4 _Scale;
            uniform half _StarsDensity;
            uniform half _StarsSize;
            uniform half4 _StarsHash;

            struct appdata_t {

                float4 vertex : POSITION;
            };

            struct v2f {

                float4  pos : SV_POSITION;
                float4   vertex : TEXCOORD0;
            };

            v2f vert (appdata_t v) {

                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                return o;
            }

            half4 frag (v2f i) : SV_Target {

                // initialise colour
                half3 col   = half3(0.0, 0.0, 0.0);

                // setup view and light directions
                half3 ray   = normalize(mul((float3x3)unity_ObjectToWorld, i.vertex ));
                half3 ray2   = normalize(mul((float3x3)unity_ObjectToWorld, i.vertex * _Scale + _Offset));
                half eyeCos = dot(_WorldSpaceLightPos0.xyz, ray2);

                // overall radial gradient
                half x = pow((1-eyeCos)*2000, 0.5) ;

                half r = 1.7 * pow(1.16, -x*2);
                half g = 1.7 * pow(1.08, -x*2);
                half b = 1.7 * pow(1.03, -x*2);

                col += half3(r, g, b);

                // light spikes
                half3 delta = _WorldSpaceLightPos0 + ray2;
                half theta  = atan2(delta.y, delta.z);
               
                half radius = length(delta);

                half spike = pow(sin(14*theta) * cos(7*delta.x), 2) * 2.5;
                col += clamp(spike  / ((radius+1)*10) * saturate((1.414-radius)), 0, 3);

                // stars by n-yoda
                half3 pos = ray / _StarsSize;
                half3 center = round(pos);
                half hash = dot(_StarsHash.xyz, center) % _StarsHash.w;
                half threshold = _StarsHash.w * _StarsDensity;

                if (abs(hash) < threshold) {

                    half dist = length(pos - center);
                    half star = pow(saturate(0.5 - dist * dist) * 2, 14);
                    col += star;
                }

                // output colour
                return half4(col, 1.0);
            }

            ENDCG
        }
    }

    Fallback Off
}
