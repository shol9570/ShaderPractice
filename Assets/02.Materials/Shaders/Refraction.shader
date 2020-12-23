Shader "SHOL/Refraction"
{
    Properties
    {
        // _Color ("Color", Color) = (1,1,1,1)
        // _MainTex ("Albedo (RGB)", 2D) = "white" {}
        // _Glossiness ("Smoothness", Range(0,1)) = 0.5
        // _Metallic ("Metallic", Range(0,1)) = 0.0
        _CenterDistort("Center distort strength", Range(0,1)) = 0.0
        _CircleTex("Circle texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        zWrite Off
        Cull Off

        GrabPass{ }

        CGPROGRAM
        #pragma surface surf nolight noambient alpha:fade
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _CircleTex;
        sampler2D _GrabTexture;
        float _CenterDistort;

        struct Input
        {
            float2 uv_CircleTex;
            float4 screenPos;
        };

        //UNITY_INSTANCING_BUFFER_START(Props)
        //UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            float3 screenUV = IN.screenPos.xyz / IN.screenPos.w;
            float nearCenter = 1 - tex2D(_CircleTex, IN.uv_CircleTex).x;// * sin(radians(frac(_Time.y) * 180));
            float3 adjustedUV = (screenUV - 0.5) / (1 - pow(0.5, 10 * nearCenter * (1 - _CenterDistort)) * _CenterDistort) + 0.5;//(screenUV - 0.5) / ((-log10(1 + nearCenter * 9) - 1) * _CenterDistort) + 0.5;
            o.Emission = tex2D(_GrabTexture, adjustedUV);
            o.Alpha = 1;
        }

        float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(0,0,0,1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
