// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "SHOL/Refraction"
{
    Properties
    {
        _CenterDistort("Center distort strength", Range(0,1)) = 0.0
        _CircleTex("Circle texture", 2D) = "white" {}
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

        struct Input
        {
            float2 uv_CircleTex;
            float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float4 objWorldPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
            float4 objScreenPos = ComputeScreenPos(objWorldPos);
            float2 objScreenUV = objScreenPos.xy / _ScreenParams.xy;// objScreenPos.xyz / IN.screenPos.w;
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float nearCenter = 1 - tex2D(_CircleTex, IN.uv_CircleTex).x;// * sin(radians(frac(_Time.y) * 180));
            if(nearCenter == 1) clip(-1);
            float2 adjustedUV = (screenUV - 0.5) / (pow(0.5, 10 * (1 - nearCenter) * _CenterDistort) * (1 - _CenterDistort)) + 0.5;//(screenUV - 0.5) / ((-log10(1 + nearCenter * 9) - 1) * _CenterDistort) + 0.5;
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
