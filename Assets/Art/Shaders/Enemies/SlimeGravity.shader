Shader "Custom/SlimeGravity"
{
    Properties
    {
        _MainTex ("Mask (Alpha)", 2D) = "white" {}

        _ColorA ("Color A", Color) = (0.2,0.8,1,1)
        _ColorB ("Color B", Color) = (0.8,0.2,1,1)
        _ColorC ("Color C", Color) = (0.3,1,0.8,1)

        _GradientStrength ("Gradient Strength", Float) = 2.0

        _NoiseScale ("Noise Scale", Float) = 2
        _NoiseSpeed ("Noise Speed", Float) = 1.5

        _Gravity ("Gravity", Float) = 0.8
        _DripStrength ("Drip", Float) = 0.6
        _DripLength ("Length", Float) = 0.5

        _Glow ("Glow", Float) = 2.5
        _EdgeWidth ("Edge Width", Float) = 0.1

        // eyes
        _EyeColor ("Eye Color", Color) = (1,1,1,1)
        _EyePos1 ("Eye 1 Pos", Vector) = (-0.15, 0.1, 0, 0)
        _EyePos2 ("Eye 2 Pos", Vector) = (0.15, 0.1, 0, 0)
        _EyeSize ("Eye Size", Float) = 0.12
        _EyeSoftness ("Eye Softness", Float) = 0.02

        // outline
        _OutlineColor ("Outline Color", Color) = (0.1,0.9,1,1)
        _OutlineWidth ("Outline Width", Float) = 0.08
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        // -------- OUTLINE --------
        Pass
        {
            Cull Front
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _OutlineWidth;
            float4 _OutlineColor;

            float _NoiseScale,_NoiseSpeed,_Gravity,_DripStrength,_DripLength;

            float hash(float n){ return frac(sin(n)*43758.5453); }

            float noise(float3 x)
            {
                float3 p=floor(x),f=frac(x);
                f=f*f*(3-2*f);
                float n=p.x+p.y*57+113*p.z;

                return lerp(
                    lerp(lerp(hash(n),hash(n+1),f.x),lerp(hash(n+57),hash(n+58),f.x),f.y),
                    lerp(lerp(hash(n+113),hash(n+114),f.x),lerp(hash(n+170),hash(n+171),f.x),f.x),f.y);
            }

            float3 deform(float3 v,float3 w)
            {
                float t=_Time.y*_NoiseSpeed;
                float n=noise(w*_NoiseScale+float3(0,-t,0));

                float g=saturate(1.0-v.y);
                float drip=smoothstep(0.6,1.0,n)*_DripStrength*g;

                v.y -= drip*_Gravity*(1+_DripLength);
                return v;
            }

            struct appdata{float4 vertex:POSITION;};
            struct v2f{float4 pos:SV_POSITION;};

            v2f vert(appdata v)
            {
                v2f o;

                float3 w=mul(unity_ObjectToWorld,v.vertex).xyz;
                float3 d=deform(v.vertex.xyz,w);

                float3 wd=mul(unity_ObjectToWorld,float4(d,1)).xyz;
                float3 c=mul(unity_ObjectToWorld,float4(0,0,0,1)).xyz;

                float3 dir=normalize(wd-c);
                float3 off=wd+dir*_OutlineWidth;

                o.pos=UnityWorldToClipPos(float4(off,1));
                return o;
            }

            fixed4 frag(v2f i):SV_Target { return _OutlineColor; }
            ENDCG
        }

        // -------- MAIN --------
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            float4 _ColorA,_ColorB,_ColorC;
            float _GradientStrength;

            float _NoiseScale,_NoiseSpeed,_Gravity,_DripStrength,_DripLength;

            float _Glow,_EdgeWidth;

            float4 _EyeColor;
            float4 _EyePos1,_EyePos2;
            float _EyeSize,_EyeSoftness;

            struct appdata
            {
                float4 vertex:POSITION;
                float2 uv:TEXCOORD0;
            };

            struct v2f
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
                float height:TEXCOORD1;
            };

            float hash(float n){ return frac(sin(n)*43758.5453); }

            float noise(float3 x)
            {
                float3 p=floor(x),f=frac(x);
                f=f*f*(3-2*f);
                float n=p.x+p.y*57+113*p.z;

                return lerp(
                    lerp(lerp(hash(n),hash(n+1),f.x),lerp(hash(n+57),hash(n+58),f.x),f.y),
                    lerp(lerp(hash(n+113),hash(n+114),f.x),lerp(hash(n+170),hash(n+171),f.x),f.x),f.y);
            }

            float3 deform(float3 v,float3 w)
            {
                float t=_Time.y*_NoiseSpeed;
                float n=noise(w*_NoiseScale+float3(0,-t,0));

                float g=saturate(1.0-v.y);
                float drip=smoothstep(0.6,1.0,n)*_DripStrength*g;

                v.y -= drip*_Gravity*(1+_DripLength);
                return v;
            }

            v2f vert(appdata v)
            {
                v2f o;

                float3 w=mul(unity_ObjectToWorld,v.vertex).xyz;
                float3 d=deform(v.vertex.xyz,w);

                o.pos=UnityObjectToClipPos(float4(d,1));
                o.uv=v.uv;
                o.height=d.y;

                return o;
            }

            // eye mask
            float eyeMask(float2 uv, float2 pos)
            {
                float d = distance(uv, pos);
                return 1.0 - smoothstep(_EyeSize, _EyeSize + _EyeSoftness, d);
            }

            fixed4 frag(v2f i):SV_Target
            {
                float4 mask = tex2D(_MainTex, i.uv);

                // gradient
                float h = i.height * _GradientStrength;
                float t = sin(h + _Time.y) * 0.5 + 0.5;

                float3 colAB = lerp(_ColorA.rgb, _ColorB.rgb, t);

                float t2 = cos(h * 0.7 + _Time.y * 0.5) * 0.5 + 0.5;
                float3 col = lerp(colAB, _ColorC.rgb, t2 * 0.5);

                // glow
                float edge = smoothstep(0.0, _EdgeWidth, mask.a);
                float glow = pow(1 - edge, 3) * _Glow;
                col += glow;

                // eyes (on top)
                float e = saturate(
                    eyeMask(i.uv, _EyePos1.xy) +
                    eyeMask(i.uv, _EyePos2.xy)
                );

                col = lerp(col, _EyeColor.rgb, e);

                return float4(col, mask.a);
            }

            ENDCG
        }
    }
}