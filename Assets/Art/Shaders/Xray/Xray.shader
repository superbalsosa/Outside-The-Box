Shader "Custom/XRayCharacter_BuiltIn_DepthSafe"
{
    Properties
    {
        _MainTex ("Optional Texture", 2D) = "white" {}
        _XRayColor ("X-Ray Fill Color", Color) = (0,1,1,1)
        _OutlineColor ("X-Ray Edge Color", Color) = (0,1,1,1)

        _Alpha ("X-Ray Fill Alpha", Range(-1,1)) = 1
        _OutlineAlpha ("X-Ray Edge Alpha", Range(-1,1)) = 0

        _DepthBias ("Depth Bias", Range(-1, 1)) = 0
        _DepthThreshold ("Depth Threshold", Range(-1, 5)) = 2
        _RimPower ("Edge Power", Range(-1, 8.0)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent+20" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Name "XRAY_OCCLUDED"

            Cull Front
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _CameraDepthTexture;

            fixed4 _XRayColor;
            fixed4 _OutlineColor;

            float _Alpha;
            float _OutlineAlpha;
            float _DepthBias;
            float _DepthThreshold;
            float _RimPower;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float eyeDepth : TEXCOORD4;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float3 normalOffset = normalize(v.normal) * _DepthBias;
                float4 displacedVertex = v.vertex + float4(normalOffset, 0.0);

                float4 clipPos = UnityObjectToClipPos(displacedVertex);
                o.pos = clipPos;
                o.screenPos = ComputeScreenPos(clipPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 worldPos = mul(unity_ObjectToWorld, displacedVertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                o.eyeDepth = -UnityObjectToViewPos(displacedVertex).z;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                float sceneRawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV);
                float sceneEyeDepth = LinearEyeDepth(sceneRawDepth);

                if (i.eyeDepth <= sceneEyeDepth + _DepthThreshold)
                    discard;

                fixed4 tex = tex2D(_MainTex, i.uv);

                float3 n = normalize(i.worldNormal);
                float3 v = normalize(i.viewDir);
                float rim = pow(1.0 - saturate(dot(n, v)), _RimPower);

                fixed4 fill = _XRayColor;
                fill.a = _Alpha * tex.a;

                fixed4 edge = _OutlineColor;
                edge.a = rim * _OutlineAlpha * tex.a;

                fixed4 col = fill;
                col.rgb = lerp(fill.rgb, edge.rgb, rim);
                col.a = saturate(fill.a + edge.a);

                return col;
            }
            ENDCG
        }
    }

    FallBack Off
}