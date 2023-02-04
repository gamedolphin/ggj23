Shader "Unlit/BGStar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorScheme("ColorScheme", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Cutout" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _ColorScheme;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float col_val = col.r;
                float a = col.a;
                float4 replace_col = tex2D(_ColorScheme, float2(round(col_val * 7.0) / 7.0, 0.0));
                return float4(replace_col.rgb, a);
            }
            ENDCG
        }
    }
}
