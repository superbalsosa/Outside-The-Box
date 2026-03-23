Shader "Custom/ParallaxScreen"
{
    Properties
    {
        _MainTex ("Base", 2D) = "white" {}
        _Layer2 ("Layer 2", 2D) = "black" {}
        _Layer3 ("Layer 3", 2D) = "black" {}
        _NoiseTex ("Noise", 2D) = "gray" {}

        _Cursor ("Cursor", Vector) = (0,0,0,0)

        _Depth1 ("Depth 1", Range(0,1)) = 0.01
        _Depth2 ("Depth 2", Range(0,1)) = 0.03
        _Depth3 ("Depth 3", Range(0,1)) = 0.08

        _Distortion ("Distortion", Range(0,0.1)) = 0.02
        _ScanlineStrength ("Scanlines", Range(0,1)) = 0.25

        _Glow ("Glow", Range(0,5)) = 2
        _Tint ("Tint", Color) = (0.4,0.8,1,1)

        _CursorRadius ("Cursor Radius", Range(0,1)) = 0.25
        _CursorBoost ("Cursor Boost", Range(0,3)) = 1.5

        _Power ("Power", Range(0,1)) = 0

        _GlowBoost ("Glow Boost", Range(0,5)) = 0
        _Shutdown ("Shutdown", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend One One

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex, _Layer2, _Layer3, _NoiseTex;

            float _Depth1, _Depth2, _Depth3;
            float _Distortion, _ScanlineStrength;
            float _Glow;

            float4 _Cursor;
            float _CursorRadius, _CursorBoost;

            float _Power;
            float _GlowBoost;
            float _Shutdown;

            float4 _Tint;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 Parallax(float2 uv, float depth)
            {
                return uv + _Cursor.xy * depth;
            }

            float Noise(float2 uv)
            {
                return tex2D(_NoiseTex, uv + _Time.y * 0.1).r;
            }

            float PowerEffect(float2 uv)
            {
                float center = abs(uv.y - 0.5);

                float thinLine = smoothstep(0.01, 0.0, center);
                float expand = smoothstep(0.05, 0.4, _Power);

                float mask = lerp(thinLine, 1.0, expand);

                return mask * _Power;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float n = Noise(i.uv);
                float2 distortion = (n - 0.5) * _Distortion;

                float2 uv1 = Parallax(i.uv + distortion, _Depth1);
                float2 uv2 = Parallax(i.uv + distortion, _Depth2);
                float2 uv3 = Parallax(i.uv + distortion, _Depth3);

                fixed4 col = tex2D(_MainTex, uv1)
                           + tex2D(_Layer2, uv2)
                           + tex2D(_Layer3, uv3);

                float scan = sin(i.uv.y * 800.0) * 0.5 + 0.5;
                col.rgb *= lerp(1.0, scan, _ScanlineStrength);

                float2 cursorUV = (_Cursor.xy * 0.5 + 0.5);
                float dist = distance(i.uv, cursorUV);
                float focus = smoothstep(_CursorRadius, 0.0, dist);
                col.rgb += focus * _CursorBoost;

                float powerMask = PowerEffect(i.uv);
                col.rgb *= powerMask;

                col.rgb += _GlowBoost;

                col.rgb *= lerp(1.0, 0.0, _Shutdown);

                col.rgb *= _Tint.rgb * _Glow;

                return col;
            }
            ENDCG
        }
    }
}