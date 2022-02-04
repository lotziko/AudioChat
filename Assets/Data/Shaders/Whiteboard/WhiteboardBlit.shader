Shader "Unlit/WhiteboardBlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Points("Points", Vector) = (0, 0, 0.5, 0.5)
		_PaintColor("Color", Color) = (1, 1, 1, 1)
		_ThicknessMultiplier("Thickness Multiplier", Float) = 1.0

		_BlendOp("Blend OP", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		ZWrite Off
		BlendOp [_BlendOp]
		Blend One OneMinusSrcAlpha

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
            float4 _MainTex_ST;
			float4 _Points;
			float4 _PaintColor;
			float _ThicknessMultiplier;

			float lineSegment(float2 p, float2 a, float2 b) {
				float thickness = 1.0 / 100.0 * _ThicknessMultiplier;

				float2 pa = p - a;
				float2 ba = b - a;

				float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
				float idk = length(pa - ba * h);

				return smoothstep(thickness * 0.5f, thickness, idk);
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float4 frag (v2f i) : SV_Target
            {
                float4 textureColor = tex2D(_MainTex, i.uv);
				float segment = lineSegment(i.uv, float2(_Points.x, _Points.y), float2(_Points.z, _Points.w));
				float4 outColor = float4(_PaintColor.rgb, (1 - segment)); //alpha blending issue try to fix
                return outColor;
            }
            ENDCG
        }
    }
}
