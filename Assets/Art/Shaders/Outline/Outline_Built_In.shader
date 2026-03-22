Shader "StandardOutline_Built_In"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Range(0,1)) = 1
        _EmissionColor ("Emission Color", Color) = (0,0,0,0)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0,2)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }
        LOD 300

        // ======================
        // MAIN SURFACE PASS
        // ======================
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _Color;
        sampler2D _BumpMap;
        float _BumpScale;
        float _Metallic;
        float _Smoothness;
        fixed4 _EmissionColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * _BumpScale;
            o.Emission = _EmissionColor.rgb;
            o.Alpha = c.a;
        }
        ENDCG

        // ======================
        // OUTLINE PASS
        // ======================
        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode"="Always" }
            Cull Front
            ZWrite On
            ZTest LEqual
            Offset 5,5  // evita z-fighting con el mesh original

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _OutlineWidth;
            float4 _OutlineColor;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // Transform to world
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Object center
                float3 objCenter = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;

                // Direction from center
                float3 dir = normalize(worldPos - objCenter);

                // Offset for outline
                float3 offsetPos = worldPos + dir * _OutlineWidth;

                // Clip space
                o.pos = UnityWorldToClipPos(float4(offsetPos,1.0));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }

    FallBack "Standard"
}