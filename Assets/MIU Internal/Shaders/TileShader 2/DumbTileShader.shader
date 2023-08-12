// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "DumbTileShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[Normal] _NormalTex ("Bumpmap", 2D) = "white" {}
		_MetallicTex ("Metallic", 2D) = "white" {}
		_Smoothness ("Smoothness", Range(0,1)) = 0.5
		_MetalFactor ("Metal Factor", Range(0,1)) = 1
		_NormalFactor ("Normal Factor", Range(0,1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200


		CGINCLUDE

#if SHADER_API_METAL
		#define _GLOSSYREFLECTIONS_OFF
#endif

		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"

		ENDCG

		CGPROGRAM

		// Physically based Standard lighting model.
		#pragma surface surf Standard noforwardadd nodynlightmap nofog

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		sampler2D _MetallicTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalTex;
			float2 uv_MetallicTex;
		};

		uniform float _MetalFactor = 1.0f;
		uniform float _Smoothness = 1.0f;
		uniform float _NormalFactor = 1.0f;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Much simpler cases!
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex);
			o.Normal = lerp(UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex)), float3(0,0,1), _NormalFactor);
			o.Metallic = _MetalFactor * tex2D(_MetallicTex, IN.uv_MainTex).r;
			o.Smoothness = _Smoothness * tex2D(_MetallicTex, IN.uv_MainTex).a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
