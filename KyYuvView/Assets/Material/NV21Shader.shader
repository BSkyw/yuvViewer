Shader "Unlit/NV21Shader"
{
   Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _uvTexture("Textrue", 2D) = "white" {}

    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _uvTexture;
            float4x4 _TextureRotation;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = float2(o.uv.x, 1.0 - o.uv.y);
                o.vertex = mul(_TextureRotation, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                const float4x4 ycbcrToRGBTransform = float4x4(
                    float4(1.0 ,+0.0000, +1.4020, -0.7010),
                    float4(1.0, -0.3441, -0.7141, +0.5291),
                    float4(1.0, +1.7720, +0.0000, -0.8860),
                    float4(0.0, +0.0000, +0.0000, +1.0000)
                    );
                float2 texcoord = i.uv;
                float y = tex2D(_MainTex, texcoord).a;
                float4 rgb4444 = tex2D(_uvTexture, texcoord);
                float v = rgb4444.b*16/17 + rgb4444.a/17;
                float u = rgb4444.r*16/17 + rgb4444.g/17;
                float4 ycbcr = float4(y, u, v, 1.0);
                fixed4 res1 = mul(ycbcrToRGBTransform, ycbcr);
                return res1;

                //return mul(ycbcrToRGBTransform, ycbcr);
            }
            ENDCG
        }
    }
}
