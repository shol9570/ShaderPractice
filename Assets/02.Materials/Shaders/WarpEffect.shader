Shader "SHOL/Warp Effect"
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
        zWrite Off
        Cull Off

        GrabPass{ "_GrabTexture" }

        PASS{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _CircleTex;
            sampler2D _GrabTexture;
            float _CenterDistort;
            float2 _WarpFocus;

            struct appdata {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata _in)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(_in.vertex);
                o.uv = _in.uv;
                o.screenPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f _in) : COLOR
            {
                float2 screenUV = _in.screenPos.xy / _in.screenPos.w;
                float nearCenter = tex2D(_CircleTex, _in.uv).x;
                if (nearCenter == 0) clip(-1);
                float4 adjustedUV = float4(0,0,0,1);
                adjustedUV.xy = (screenUV - _WarpFocus) / pow(1 - _CenterDistort * 0.999, 10 * nearCenter) + _WarpFocus;
                fixed4 color = tex2Dproj(_GrabTexture, adjustedUV);
                return color;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
