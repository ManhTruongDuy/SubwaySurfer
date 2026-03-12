Shader "Custom/CurvedWorld"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Curvature ("Curvature (Độ cong)", Float) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Curvature;

            v2f vert (appdata v)
            {
                v2f o;
                // Chuyển đỉnh sang không gian Thế giới (World Space)
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

                // Tính toán khoảng cách từ Camera đến vật thể theo trục Z
                float dist = worldPos.z - _WorldSpaceCameraPos.z;

                // Công thức bẻ cong: y = y - (curvature * dist^2)
                // Chỉ bẻ cong những gì ở phía trước camera (dist > 0)
                if (dist > 0)
                {
                    worldPos.y -= (dist * dist) * _Curvature;
                }

                // Chuyển ngược lại không gian Clip để hiển thị lên màn hình
                o.vertex = mul(unity_MatrixVP, worldPos);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}