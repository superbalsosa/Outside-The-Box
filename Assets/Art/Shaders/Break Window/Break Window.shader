Shader "Custom/Break_Window"
{
    Properties
    {
        // Base colors
        _GlassColor ("Glass Color", Color) = (0.55,0.78,0.95,1)
        _CrackHighlightColor ("Crack Highlight Color", Color) = (0.95,0.98,1,1)
        _CrackShadowColor ("Crack Shadow Color", Color) = (0.16,0.24,0.32,1)

        // Damage & impact
        _Damage ("Damage", Range(0,1)) = 0
        _ImpactCenter ("Impact Center", Vector) = (0.5,0.5,0,0)
        _ImpactRadius ("Impact Radius", Range(0.001,0.3)) = 0.035
        _ImpactSoftness ("Impact Softness", Range(0.001,0.2)) = 0.05

        // Surface properties
        _Smoothness ("Smoothness", Range(0,1)) = 0.96
        _SpecularStrength ("Specular Strength", Range(0,2)) = 1
        _ReflectionStrength ("Reflection Strength", Range(0,2)) = 1
        _FresnelPower ("Fresnel Power", Range(0.1,10)) = 4.5
        _FresnelStrength ("Fresnel Strength", Range(0,2)) = 1
        _TintStrength ("Tint Strength", Range(0,1)) = 0.85
        _AmbientStrength ("Ambient Strength", Range(0,3)) = 1.45
        _EnvironmentTintStrength ("Environment Tint Strength", Range(0,2)) = 0.85

        [Toggle] _EnableSpecularHighlights ("Specular Highlights", Float) = 1
        [Toggle] _EnableReflections ("Reflections", Float) = 1

        // Glass alpha
        _GlassAlpha ("Glass Alpha", Range(0,1)) = 0.14
        _CrackAlphaBoost ("Crack Alpha Boost", Range(0,1)) = 0.06

        // Crack parameters
        _RadialCount ("Radial Count", Range(4,32)) = 12
        _RadialThickness ("Radial Thickness", Range(0.0005,0.03)) = 0.003
        _RadialWarp ("Radial Warp", Range(0,2)) = 0.35
        _RadialStrength ("Radial Strength", Range(0,3)) = 1

        _RingFrequency ("Ring Frequency", Range(1,40)) = 10
        _RingThickness ("Ring Thickness", Range(0.0005,0.03)) = 0.004
        _RingIrregularity ("Ring Irregularity", Range(0,2)) = 0.5
        _RingStrength ("Ring Strength", Range(0,3)) = 0.7

        _SecondaryScale ("Secondary Scale", Range(1,80)) = 18
        _SecondaryThickness ("Secondary Thickness", Range(0.0005,0.02)) = 0.0018
        _SecondaryStrength ("Secondary Strength", Range(0,3)) = 0.18

        _NormalStrength ("Crack Normal Strength", Range(0,2)) = 0.8
        _MicroNormalStrength ("Micro Normal Strength", Range(0,2)) = 0.25
        _MicroNormalTex ("Micro Normal Map", 2D) = "bump" {}
        
        _HighlightStrength ("Crack Highlight Strength", Range(0,1)) = 0.09
        _ShadowStrength ("Crack Shadow Strength", Range(0,1)) = 0.65

        _DebugMode ("Debug Mode", Range(0,6)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            Tags { "LightMode"="ForwardBase" }

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MicroNormalTex;
            fixed4 _GlassColor;
            fixed4 _CrackHighlightColor;
            fixed4 _CrackShadowColor;

            float _Damage;
            float _GlassAlpha;
            float _CrackAlphaBoost;

            float _Smoothness;
            float _SpecularStrength;
            float _ReflectionStrength;
            float _FresnelPower;
            float _FresnelStrength;
            float _TintStrength;
            float _AmbientStrength;
            float _EnvironmentTintStrength;

            float _EnableSpecularHighlights;
            float _EnableReflections;

            float4 _ImpactCenter;
            float _ImpactRadius;
            float _ImpactSoftness;

            float _RadialCount;
            float _RadialThickness;
            float _RadialWarp;
            float _RadialStrength;

            float _RingFrequency;
            float _RingThickness;
            float _RingIrregularity;
            float _RingStrength;

            float _SecondaryScale;
            float _SecondaryThickness;
            float _SecondaryStrength;

            float _NormalStrength;
            float _MicroNormalStrength;

            float _HighlightStrength;
            float _ShadowStrength;

            float _DebugMode;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBinormal : TEXCOORD4;
                float3 viewDir : TEXCOORD5;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;
                o.viewDir = UnityWorldSpaceViewDir(o.worldPos);
                return o;
            }

            // Hash function for secondary cracks
            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            // Secondary cracks function
            float secondaryCracks(float2 uv)
            {
                float2 g = uv * _SecondaryScale;
                float2 id = floor(g);
                float2 f = frac(g) - 0.5;

                float minLine = 1.0;

                [unroll] for (int y=-1;y<=1;y++)
                    [unroll] for (int x=-1;x<=1;x++)
                    {
                        float2 cell = id + float2(x,y);
                        float a = hash21(cell) * 6.28318;
                        float2 dir = float2(cos(a), sin(a));
                        float d = abs(dot(f - float2(x,y), float2(-dir.y, dir.x)));
                        minLine = min(minLine, d);
                    }

                return smoothstep(_SecondaryThickness, 0.0, minLine) * _SecondaryStrength;
            }

            // Fracture field function
            float fractureField(float2 uv, out float radialMask, out float ringMask, out float secondaryMask, out float impactMask)
            {
                // Remap damage: 0.5->0, 1->1
                float remappedDamage = saturate((_Damage - 0.5) / 0.5);

                float2 center = _ImpactCenter.xy;
                float2 p = uv - center;
                float r = length(p);
                float a = atan2(p.y, p.x);

                float angleNoise = sin(a*7 + r*18)*0.08 + cos(a*13 - r*22)*0.05 + sin(a*19 + r*9)*0.04;
                radialMask = smoothstep(_RadialThickness, 0.0, abs(frac((a/6.28318 + 0.5 + angleNoise*_RadialWarp)*_RadialCount) - 0.5)) * _RadialStrength;

                float ringNoise = sin(a*9 + r*15)*0.03 + cos(a*5 - r*11)*0.02;
                ringMask = smoothstep(_RingThickness, 0.0, abs(frac((r + ringNoise*_RingIrregularity)*_RingFrequency) - 0.5)) * _RingStrength;

                secondaryMask = secondaryCracks(uv);

                impactMask = 1.0 - smoothstep(_ImpactRadius, _ImpactRadius + _ImpactSoftness, r);

                float radialReveal = remappedDamage;
                float ringReveal = remappedDamage;
                float secondaryReveal = remappedDamage;

                radialMask *= radialReveal * saturate(1.0 - r * 0.25);
                ringMask   *= ringReveal * saturate(1.0 - r * 0.18);
                secondaryMask *= secondaryReveal * saturate(1.0 - r * 0.12);

                ringMask *= saturate(radialMask*1.4 + 0.15);

                return saturate(radialMask + ringMask + secondaryMask + impactMask * 0.85 * remappedDamage);
            }

            // Fake normal computation with micro normal map
            float3 computeFakeNormal(float3 N, float3 T, float3 B, float2 uv)
            {
                float eps = 0.0025;
                float radialMask, ringMask, secondaryMask, impactMask;
                float fC = fractureField(uv, radialMask, ringMask, secondaryMask, impactMask);
                float fX = fractureField(uv + float2(eps,0), radialMask, ringMask, secondaryMask, impactMask);
                float fY = fractureField(uv + float2(0,eps), radialMask, ringMask, secondaryMask, impactMask);
                float2 grad = float2(fX - fC, fY - fC);

                float3 fakeN = normalize(N - T*grad.x*_NormalStrength - B*grad.y*_NormalStrength);

                float3 micro = tex2D(_MicroNormalTex, uv).rgb*2-1;
                fakeN = normalize(fakeN + micro*_MicroNormalStrength);

                return fakeN;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float radialMask, ringMask, secondaryMask, impactMask;
                float bodyMask = fractureField(uv, radialMask, ringMask, secondaryMask, impactMask);
                float lineMask = saturate(radialMask + ringMask*0.85 + secondaryMask*0.7);

                float stressMask = saturate(bodyMask*(0.75 + impactMask*0.35));
                float shadow = saturate(stressMask*_ShadowStrength);
                float crackHighlight = saturate(lineMask*_HighlightStrength*6);

                float3 fakeN = computeFakeNormal(i.worldNormal, i.worldTangent, i.worldBinormal, uv);

                if(_DebugMode>0.5 && _DebugMode<1.5) return fixed4(radialMask,radialMask,radialMask,1);
                if(_DebugMode>1.5 && _DebugMode<2.5) return fixed4(ringMask,ringMask,ringMask,1);
                if(_DebugMode>2.5 && _DebugMode<3.5) return fixed4(secondaryMask,secondaryMask,secondaryMask,1);
                if(_DebugMode>3.5 && _DebugMode<4.5) return fixed4(impactMask,impactMask,impactMask,1);
                if(_DebugMode>4.5 && _DebugMode<5.5) return fixed4(fakeN*0.5+0.5,1);
                if(_DebugMode>5.5) return fixed4(lineMask,bodyMask,impactMask,1);

                float3 V = normalize(i.viewDir);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 H = normalize(L+V);

                float NdotL = saturate(dot(fakeN,L));
                float NdotV = saturate(dot(fakeN,V));
                float NdotH = saturate(dot(fakeN,H));

                float3 ambient = ShadeSH9(float4(fakeN,1))*_AmbientStrength;
                float3 directLight = _LightColor0.rgb*NdotL;

                float fresnel = pow(1.0-NdotV,_FresnelPower)*_FresnelStrength;
                float specPower = lerp(12,320,_Smoothness);
                float spec = pow(NdotH,specPower)*_SpecularStrength*NdotL*step(0.5,_EnableSpecularHighlights);

                float3 reflection = 0;
                float3 envColor = 0;
                if(_EnableReflections>0.5)
                {
                    float3 reflDir = reflect(-V,fakeN);
                    half4 envSample = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0,reflDir);
                    envColor = DecodeHDR(envSample,unity_SpecCube0_HDR).rgb;
                    reflection = envColor*(_ReflectionStrength*(0.35+fresnel));
                }

                float3 baseGlass = lerp(float3(1,1,1),_GlassColor.rgb,_TintStrength);
                float3 environmentTint = lerp(float3(1,1,1),envColor,_EnvironmentTintStrength);

                float3 litGlass = baseGlass*environmentTint*(ambient+directLight+0.14);
                float3 stressedGlass = lerp(litGlass,litGlass*_CrackShadowColor.rgb,shadow);
                float3 crackLight = _CrackHighlightColor.rgb*crackHighlight;

                float3 finalColor = stressedGlass + spec.xxx + reflection + crackLight;
                float alpha = saturate(_GlassAlpha + stressMask*_CrackAlphaBoost + fresnel*0.08);

                return fixed4(saturate(finalColor),alpha);
            }
            ENDCG
        }
    }

    FallBack Off
}