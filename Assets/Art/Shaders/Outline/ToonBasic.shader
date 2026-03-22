Shader "Custom/ToonBasic"
{
Properties
{
    _MainTex ("Albedo", 2D) = "white" {}
    _Color ("Tint", Color) = (1,1,1,1)

    [Toggle] _UseRamp ("Use Shadow Ramp", Float) = 0
    _RampTex ("Shadow Ramp", 2D) = "gray" {}

    _ShadowColor ("Shadow Color", Color) = (0.6,0.6,0.7,1)
    _Levels ("Toon Levels", Range(2,25)) = 3

    [Toggle] _UseNormal ("Use Normal Map", Float) = 0
    _NormalMap ("Normal Map", 2D) = "bump" {}

    [Toggle] _UseRim ("Use Rim Light", Float) = 1
    _RimColor ("Rim Color", Color) = (1,1,1,1)
    _RimPower ("Rim Power", Range(0.5,8)) = 3

    [Toggle] _UseSpec ("Use Specular", Float) = 1
    _ToonSpecColor ("Specular Color", Color) = (1,1,1,1)
    _SpecSize ("Specular Size", Range(0.01,1)) = 0.25

    _AmbientStrength ("Ambient Strength", Range(0,1)) = 0.25

    // Outline 1
    [Toggle] _UseOutline ("Use Outline", Float) = 1
    _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    _Outline ("Outline Width", Range(0,55)) = 15
    [Toggle] _OutlineWorld ("Outline1 World Space", Float) = 1
    [Toggle] _OutlineVertex ("Outline1 Vertex Normals", Float) = 0
    [Toggle] _OutlineScreen ("Outline1 Screen-space", Float) = 0
    [Toggle] _OutlineSilhouette ("Outline1 Camera Silhouette", Float) = 0

    // Outline 2
    [Toggle] _UseOutline2 ("Use Outline2", Float) = 0
    _Outline2Color ("Outline2 Color", Color) = (0,0,0,1)
    _Outline2 ("Outline2 Width", Range(0,55)) = 10
    [Toggle] _Outline2World ("Outline2 World Space", Float) = 1
    [Toggle] _Outline2Vertex ("Outline2 Vertex Normals", Float) = 0
    [Toggle] _Outline2Screen ("Outline2 Screen-space", Float) = 0
    [Toggle] _Outline2Silhouette ("Outline2 Camera Silhouette", Float) = 0
}

SubShader
{
Tags { "RenderType"="Opaque" }

//////////////////////////////////////////////////////////////
// Outline Pass 1
//////////////////////////////////////////////////////////////
Pass
{
    Name "OUTLINE1"
    Tags { "LightMode"="Always" }
    Cull Front
    Offset 3,3
    ZWrite Off
    ZTest LEqual

    CGPROGRAM
    #pragma vertex vertOutline
    #pragma fragment fragOutline
    #include "UnityCG.cginc"

    float _Outline;
    float4 _OutlineColor;
    float _OutlineWorld;
    float _OutlineVertex;
    float _OutlineScreen;
    float _OutlineSilhouette;

    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
    };

    float3 ComputeOutlineOffset(float3 worldPos, float3 normalWorld)
    {
        float3 offset = float3(0,0,0);

        if (_OutlineWorld > 0.5)
            offset += normalize(worldPos - unity_ObjectToWorld._m03_m13_m23) * (_Outline * 0.02);
        if (_OutlineVertex > 0.5)
            offset += normalWorld * (_Outline * 0.02);
        if (_OutlineSilhouette > 0.5)
            offset += normalWorld * (_Outline * 0.02) * dot(normalWorld, normalize(_WorldSpaceCameraPos - worldPos));

        return offset;
    }

    v2f vertOutline(appdata v)
    {
        v2f o;
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        float3 normalWorld = UnityObjectToWorldNormal(v.normal);

        float3 finalPos = worldPos + ComputeOutlineOffset(worldPos, normalWorld);

        if (_OutlineScreen > 0.5)
        {
            float4 clipPos = UnityWorldToClipPos(finalPos);
            float2 ndc = clipPos.xy / clipPos.w;
            ndc += float2(_Outline / 500.0, _Outline / 500.0);
            clipPos.xy = ndc * clipPos.w;
            o.pos = clipPos;
        }
        else
        {
            o.pos = UnityWorldToClipPos(finalPos);
        }
        return o;
    }

    fixed4 fragOutline(v2f i) : SV_Target
    {
        return _OutlineColor;
    }
    ENDCG
}

//////////////////////////////////////////////////////////////
// Outline Pass 2
//////////////////////////////////////////////////////////////
Pass
{
    Name "OUTLINE2"
    Tags { "LightMode"="Always" }
    Cull Front
    Offset 4,4
    ZWrite Off
    ZTest LEqual

    CGPROGRAM
    #pragma vertex vertOutline2
    #pragma fragment fragOutline2
    #include "UnityCG.cginc"

    float _Outline2;
    float4 _Outline2Color;
    float _Outline2World;
    float _Outline2Vertex;
    float _Outline2Screen;
    float _Outline2Silhouette;

    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };

    struct v2f
    {
        float4 pos : SV_POSITION;
    };

    float3 ComputeOutline2Offset(float3 worldPos, float3 normalWorld)
    {
        float3 offset = float3(0,0,0);

        if (_Outline2World > 0.5)
            offset += normalize(worldPos - unity_ObjectToWorld._m03_m13_m23) * (_Outline2 * 0.02);
        if (_Outline2Vertex > 0.5)
            offset += normalWorld * (_Outline2 * 0.02);
        if (_Outline2Silhouette > 0.5)
            offset += normalWorld * (_Outline2 * 0.02) * dot(normalWorld, normalize(_WorldSpaceCameraPos - worldPos));

        return offset;
    }

    v2f vertOutline2(appdata v)
    {
        v2f o;
        float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
        float3 normalWorld = UnityObjectToWorldNormal(v.normal);

        float3 finalPos = worldPos + ComputeOutline2Offset(worldPos, normalWorld);

        if (_Outline2Screen > 0.5)
        {
            float4 clipPos = UnityWorldToClipPos(finalPos);
            float2 ndc = clipPos.xy / clipPos.w;
            ndc += float2(_Outline2 / 500.0, _Outline2 / 500.0);
            clipPos.xy = ndc * clipPos.w;
            o.pos = clipPos;
        }
        else
        {
            o.pos = UnityWorldToClipPos(finalPos);
        }
        return o;
    }

    fixed4 fragOutline2(v2f i) : SV_Target
    {
        return _Outline2Color;
    }
    ENDCG
}

//////////////////////////////////////////////////////////////
// Main Lighting Pass (Toon)
//////////////////////////////////////////////////////////////
Pass
{
Tags { "LightMode"="ForwardBase" }

CGPROGRAM
#pragma multi_compile_fwdbase
#pragma vertex vertMain
#pragma fragment fragMain

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

sampler2D _MainTex;
float4 _Color;

sampler2D _RampTex;
float _UseRamp;

sampler2D _NormalMap;
float _UseNormal;

float4 _ShadowColor;
float _Levels;
float _AmbientStrength;

float4 _RimColor;
float _RimPower;
float _UseRim;

float4 _ToonSpecColor;
float _SpecSize;
float _UseSpec;

struct appdata
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
    float4 tangent : TANGENT;
};

struct v2f
{
    float4 pos : SV_POSITION;
    float3 normal : TEXCOORD0;
    float3 worldPos : TEXCOORD1;
    float2 uv : TEXCOORD2;

    float3 tangent : TEXCOORD3;
    float3 bitangent : TEXCOORD4;

    SHADOW_COORDS(5)
};

v2f vertMain(appdata v)
{
    v2f o;

    o.pos = UnityObjectToClipPos(v.vertex);

    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.normal = UnityObjectToWorldNormal(v.normal);

    o.uv = v.uv;

    o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
    o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;

    TRANSFER_SHADOW(o);

    return o;
}

fixed4 fragMain(v2f i) : SV_Target
{
    float3 normal = normalize(i.normal);

    if (_UseNormal > 0.5)
    {
        float3 normalTex = UnpackNormal(tex2D(_NormalMap, i.uv));
        float3x3 TBN = float3x3(
            normalize(i.tangent),
            normalize(i.bitangent),
            normalize(i.normal)
        );
        normal = normalize(mul(normalTex, TBN));
    }

    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
    float NdotL = saturate(dot(normal, lightDir));

    float light;
    if (_UseRamp > 0.5)
        light = tex2D(_RampTex, float2(NdotL,0)).r;
    else
    {
        float scaled = NdotL * _Levels;
        float band = floor(scaled);
        light = band / (_Levels - 1);
    }

    float shadow = SHADOW_ATTENUATION(i);
    float3 albedo = tex2D(_MainTex, i.uv).rgb * _Color.rgb;

    float3 lit = albedo * light;
    float3 shadowCol = _ShadowColor.rgb;
    float3 toon = lerp(shadowCol, lit, light);

    float3 lightColor = toon * _LightColor0.rgb * shadow;
    float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * _AmbientStrength * albedo;

    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

    float3 rimLight = 0;
    if (_UseRim > 0.5)
    {
        float rim = 1 - saturate(dot(viewDir, normal));
        rim = pow(rim, _RimPower);
        rimLight = _RimColor.rgb * rim;
    }

    float3 specular = 0;
    if (_UseSpec > 0.5)
    {
        float3 halfDir = normalize(lightDir + viewDir);
        float spec = saturate(dot(normal, halfDir));
        spec = step(1 - _SpecSize, spec);
        specular = _ToonSpecColor.rgb * spec;
    }

    float3 finalColor = lightColor + ambient + rimLight + specular;
    return float4(finalColor,1);
}

ENDCG
}
}
}