Shader "Custom/AlertUI_Overlay"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,0.4,0.1,1)
        _EmissionColor ("Emission", Color) = (4,1,0.3,1)

        _Radius ("Radius", Float) = 0.4
        _Thickness ("Thickness", Float) = 0.12
        _Softness ("Softness", Float) = 0.02

        _Segments ("Segments", Float) = 8
        _RotationSpeed ("Rotation", Float) = 1
        _PulseSpeed ("Pulse", Float) = 2

        _Progress ("Progress", Range(0,1)) = 0

        _IconTex ("Icon", 2D) = "white" {}
        _IconSize ("Icon Size", Float) = 0.25

        _ScreenSize ("Screen Size", Float) = 0.1
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert

        sampler2D _IconTex;

        fixed4 _Color;
        fixed4 _EmissionColor;

        float _Radius;
        float _Thickness;
        float _Softness;

        float _Segments;
        float _RotationSpeed;
        float _PulseSpeed;

        float _Progress;
        float _IconSize;
        float _ScreenSize;

        struct Input
        {
            float2 uv_MainTex;
        };

        // Screen-space billboard
        void vert (inout appdata_full v)
        {
            float3 centerWS = mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
            float3 camPos = _WorldSpaceCameraPos;

            float dist = distance(centerWS, camPos);

            float3 forward = normalize(camPos - centerWS);
            float3 up = float3(0,1,0);
            float3 right = normalize(cross(up, forward));
            up = cross(forward, right);

            float scale = dist * _ScreenSize;

            float3 local = v.vertex.xyz * scale;

            float3 worldPos = centerWS + right * local.x + up * local.y;

            v.vertex = mul(unity_WorldToObject, float4(worldPos,1));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex - 0.5;

            float dist = length(uv);
            float angle = atan2(uv.y, uv.x);

            angle += _Time.y * _RotationSpeed;

            float normAngle = (angle / (2 * 3.14159)) + 0.5;

            float inner = smoothstep(_Radius, _Radius - _Softness, dist);
            float outer = smoothstep(_Radius + _Thickness, _Radius + _Thickness - _Softness, dist);
            float ring = outer - inner;

            float seg = frac(normAngle * _Segments);
            float segmentMask = step(0.4, seg);

            float segmentedRing = ring * segmentMask;

            float progressMask = step(normAngle, _Progress) * ring;

            float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;

            float iconMask = smoothstep(_IconSize, _IconSize - _Softness, dist);
            float4 icon = tex2D(_IconTex, IN.uv_MainTex) * iconMask;

            float alpha = max(segmentedRing + progressMask, icon.a * iconMask);

            o.Albedo = _Color.rgb;
            o.Emission =
                _EmissionColor.rgb * (segmentedRing * (1 + pulse) + progressMask * 2)
                + icon.rgb * 2;

            o.Alpha = alpha;
        }
        ENDCG
    }
}