Shader "Unlit/Whiteboard"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
		_PaintMap("Paint Map", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name "Forward"
			Tags{"LightMode" = "UniversalForward"}

			Blend One Zero
			ZWrite On
			Cull Back

			HLSLPROGRAM
			#pragma exclude_renderers gles gles3 glcore
			#pragma target 4.5

			// -------------------------------------
			// Universal Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex LitPassVertex
			#pragma fragment LitPassFragment

			#include "WhiteboardLitInput.hlsl"
			#include "WhiteboardLitForwardPass.hlsl"
			ENDHLSL
	}

   //     Pass
   //     {
   //         CGPROGRAM
   //         #pragma vertex vert
   //         #pragma fragment frag
   //         // make fog work
   //         #pragma multi_compile_fog

   //         #include "UnityCG.cginc"

   //         struct appdata
   //         {
   //             float4 vertex : POSITION;
   //             float2 uv : TEXCOORD0;
			//	UNITY_VERTEX_INPUT_INSTANCE_ID
   //         };

   //         struct v2f
   //         {
   //             float2 uv : TEXCOORD0;
   //             UNITY_FOG_COORDS(1)
   //             float4 vertex : SV_POSITION;
			//	UNITY_VERTEX_INPUT_INSTANCE_ID
			//	UNITY_VERTEX_OUTPUT_STEREO
   //         };

   //         sampler2D _MainTex;
			//sampler2D _PaintTex;
   //         float4 _MainTex_ST;

   //         v2f vert (appdata v)
   //         {
   //             v2f o;

			//	UNITY_SETUP_INSTANCE_ID(v);
			//	UNITY_TRANSFER_INSTANCE_ID(v, o);
			//	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

   //             o.vertex = UnityObjectToClipPos(v.vertex);
   //             o.uv = TRANSFORM_TEX(v.uv, _MainTex);
   //             UNITY_TRANSFER_FOG(o,o.vertex);
   //             return o;
   //         }

			//half4 frag (v2f i) : SV_Target
   //         {
			//	UNITY_SETUP_INSTANCE_ID(i);
			//	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

			//	half4 mainColor = tex2D(_MainTex, i.uv);
			//	half4 paintColor = tex2D(_PaintTex, i.uv);
   //             return mainColor * (1 - paintColor.a) + paintColor;
   //         }
   //         ENDCG
   //     }
    }
}
