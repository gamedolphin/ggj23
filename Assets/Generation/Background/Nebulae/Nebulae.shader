Shader "Unlit/Nebulae"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Size("Size",float) = 50.0
        _OCTAVES("OCTAVES", range(0,20)) = 0
        _Seed("Seed",range(1, 10)) = 1
        _Pixels("Pixels",float) = 100.0
        _BackgroundColor ("Background Color", Vector) = (0,0,0,0)

        _ColorScheme("ColorScheme", 2D) = "white" {}
        _UV_Correct("UVCorrect", Vector) = (1.0,0.0,0.0,0.0)

        [Toggle] _ShouldTile("ShouldTile", Float) = 0
        [Toggle] _ReduceBackground("ReduceBackground", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 100

        Pass
        {
            Tags { "LightMode"="UniversalForward"}

            CULL Off
            ZWrite Off // don't write to depth buffer
            Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "../../Planets/cginc/hlmod.cginc"

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
            float4 _MainTex_ST;

            float _ReduceBackground;
            float _ShouldTile;
            float4 _BackgroundColor;
            float _Size;
            int _OCTAVES;
            int _Seed;
            float _Pixels;

            sampler2D _ColorScheme;
            float2 _UV_Correct;

            float rand(float2 coord, float tilesize) {
                if (_ShouldTile != 0.0) {
                    coord = mod(coord / _UV_Correct, tilesize );
                }

                return frac(sin(dot(coord.xy ,float2(12.9898,78.233))) * (15.5453 + _Seed));
            }

            float noise(float2 coord, float tilesize){
                float2 i = floor(coord);
                float2 f = frac(coord);

                float a = rand(i, tilesize);
                float b = rand(i + float2(1.0, 0.0), tilesize);
                float c = rand(i + float2(0.0, 1.0), tilesize);
                float d = rand(i + float2(1.0, 1.0), tilesize);
                float2 cubic = f * f * (3.0 - 2.0 * f);

                return lerp(a, b, cubic.x) + (c - a) * cubic.y * (1.0 - cubic.x) + (d - b) * cubic.x * cubic.y;
            }

            float fbm(float2 coord, float tilesize){
                float value = 0.0;
                float scale = 0.5;

                for(int i = 0; i < _OCTAVES ; i++){
                    value += noise(coord, tilesize ) * scale;
                    coord *= 2.0;
                    scale *= 0.5;
                }
                return value;
            }

            bool dither(float2 uv1, float2 uv2) {
                 return mod(uv1.y+uv2.x,2.0/_Pixels) <= 1.0 / _Pixels;
            }

            float circleNoise(float2 uv, float tilesize) {
                  if (_ShouldTile) {
                     uv = mod(uv, tilesize);
                  }

                  float uv_y = floor(uv.y);
                  uv.x += uv_y*.31;
                  float2 f = frac(uv);
                  float h = rand(float2(floor(uv.x),floor(uv_y)), tilesize);
                  float m = (length(f-0.25-(h*0.5)));
                  float r = h*0.25;
                  return smoothstep(0.0, r, m*0.75);
             }

             float2 rotate(float2 vec, float angle) {
                  vec -=float2(0.5, 0.5);
                  vec = mul(vec,float2x2(cos(angle),-sin(angle), sin(angle),cos(angle)));
                  vec += float2(0.5,0.5);
                  return vec;
             }

             float cloud_alpha(float2 uv, float tilesize) {
                   float c_noise = 0.0;

                   // more iterations for more turbulence
                   int iters = 2;
                   for (int i = 0; i < iters; i++) {
                       c_noise += circleNoise(uv * 0.5 + (float(i+1)) + float2(-0.3, 0.0), ceil(tilesize * 0.5));
                   }
                   float fbmVar = fbm(uv+c_noise, tilesize);

                   return fbmVar;
               }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // pixelizing and dithering
                float2 uv = floor((i.uv) * _Pixels) / _Pixels;

                float d =  distance(uv, float2(0.5, 0.5)) * 0.4;

                uv *= _UV_Correct;

                bool dith = dither(uv, i.uv);

                float n = cloud_alpha(uv*_Size, _Size);
                float n2 = fbm(uv * _Size + float2(1, 1), _Size);
                float n_lerp = n2 * n;
                float n_dust = cloud_alpha(uv * _Size, _Size);
                float n_dust_lerp = n_dust * n_lerp;

                if (dith) {
                    n_dust_lerp *= 0.95;
                    n_lerp *= 0.95;
                    d*= 0.98;
                }

                // slightly offset alpha values to create thin bands around the nebulae
                float a = step(n2, 0.1 + d);
                float a2 = step(n2, 0.115 + d);
                if (_ShouldTile) {
                    a = step(n2, 0.3);
                    a2 = step(n2, 0.315);
                }

                if (_ReduceBackground) {
                    n_dust_lerp = pow(n_dust_lerp, 1.2) * 0.7;
                }
                float col_value = 0.0;
                if (a2 > a) {
                    col_value = floor(n_dust_lerp * 35.0) / 7.0;
                } else {
                    col_value = floor(n_dust_lerp * 14.0) / 7.0;
                }

                // apply colors
                float3 col = tex2D(_ColorScheme, float2(col_value, 0.0)).rgb;
                if (col_value < 0.1) {
                    col = _BackgroundColor.rgb;
                }

                return float4(col, a2);
            }
            ENDCG
        }
    }
}
