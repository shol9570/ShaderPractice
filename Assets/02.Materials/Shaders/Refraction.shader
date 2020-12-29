Shader "SHOL/Refraction"
{
    Properties
    {
        _CenterDistort("Center distort strength", Range(0,1)) = 0.0
        _CircleTex("Circle texture", 2D) = "white" {}
        _WarpFocus("Warp focus position", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        ZTest Off
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
        float2 _WarpFocus;

        struct Input
        {
            float2 uv_CircleTex;
            float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float nearCenter = tex2D(_CircleTex, IN.uv_CircleTex).x;
            if(nearCenter == 0) clip(-1);
            float2 adjustedUV = (screenUV - _WarpFocus) / (pow(0.5, 10 * nearCenter * _CenterDistort) * (1 - _CenterDistort)) + _WarpFocus;
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
