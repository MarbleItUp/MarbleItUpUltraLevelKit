// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.
Shader "Custom/NewTileShader" {
	Properties {
		_ColorLightA ("High Light", Color) = (0,0,0,1)
		_ColorDarkA("Low Light", Color) = (0,0,0,1)
		_ColorLightB("High Dark", Color) = (0,1,0,1)
		_ColorDarkB("Low Dark", Color) = (0,1,0,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorOverlayLight("Overlay Color Light", Color) = (0,0,0,1)
		_ColorOverlayDark("Overlay Color Dark", Color) = (0,0,0,1)
		_OverlayTex ("Overlay (RGB)", 2D) = "white" {}
		_OverlayFactor ("Overlay Factor", Range(0,1)) = 0.1
		[Normal] _NormalTex ("Normal", 2D) = "white" {}
		_MetallicTex ("Metallic", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_MetalFactor ("Metal Factor", Range(0,2)) = 1
		_NoiseContrast("Noise Contrast", Range(0,1)) = 0.5
		[NoScaleOffset] _NoiseTexture ("Noise", 2D) = "white" {}
		[Toggle] _WorldSpace("World Space Noise", Float) = 1
		[Header(Grid)]
		_ColorGrid("Grid Color", Color) = (0,1,0,1)
		_GridScale ("Grid Scale", Range(0,0.25)) = 0.5
		_GridRadius ("Grid Radius", Range(0,0.5)) = 0.08
		_GridEdgeScale ("Grid Edge Scale", Range(0, 100)) = 30
		_GridEdgeOffset ("Grid Edge Offset", Range(-1, 1)) = 0.1
		_GridEdgeTexelOffsetX("Grid Nudge X", Range(-0.5, 0.5)) = 0.0
		_GridEdgeTexelOffsetY("Grid Nudge Y", Range(-0.5, 0.5)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
		// Physically based Standard lighting model.
		#pragma surface surf Standard noforwardadd nodynlightmap nofog
		#pragma require derivatives
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		sampler2D _MainTex;
		sampler2D _OverlayTex;
		sampler2D _NormalTex;
		sampler2D _MetallicTex;
		sampler2D _NoiseTexture;
		struct Input {
			float2 uv_MainTex;
			float2 uv_OverlayTex;
			float2 uv_NormalTex;
			float2 uv_MetallicTex;
			float3 worldPos;
		};
		uniform half _Glossiness, _MetalFactor, _NoiseContrast, _WorldSpace;
		uniform half _OverlayFactor, _GridScale, _GridRadius;
		uniform half _BaseGroutWidth, _GroutDDYScale, _GroudDDYOffset;
		uniform fixed4 _ColorLightA, _ColorLightB, _ColorDarkA, _ColorDarkB;
		uniform fixed4 _ColorOverlayLight, _ColorOverlayDark;
		uniform fixed4 _ColorGrid;
		uniform half _GridEdgeScale, _GridEdgeOffset;
		uniform half4 _NormalTex_TexelSize;
		uniform half4 _MainTex_TexelSize;
		uniform half4 _NoiseTexture_TexelSize;
		uniform half _GridEdgeTexelOffsetX, _GridEdgeTexelOffsetY;
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)
		void surf (Input IN, inout SurfaceOutputStandard o) {
			float4 worldPos = mul(unity_WorldToObject,float4( 0,0,0,1 ));
			float2 realMainUV = IN.uv_MainTex;
			// Albedo comes from a texture tinted by color
			half base = tex2D (_MainTex, IN.uv_MainTex).r;
			half overlay = tex2D (_OverlayTex, IN.uv_MainTex).r;
			// Sample the noise texture.
			fixed noiseVal = 0.0f;
			// And apply to the overlay.
			fixed4 cA = lerp(_ColorDarkA, _ColorLightA, noiseVal);
			fixed4 cB = lerp(_ColorDarkB, _ColorLightB, noiseVal);
			fixed4 overlayColor = lerp(_ColorOverlayDark, _ColorOverlayLight, noiseVal);
			// Combined overlay and base into final color.
			o.Albedo = lerp(lerp(cA, cB, 1-base), overlayColor, overlay * _OverlayFactor);
			// Much simpler cases!
			o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
			o.Metallic = _MetalFactor * tex2D(_MetallicTex, IN.uv_MainTex).r;
			o.Smoothness = _Glossiness * tex2D(_MetallicTex, IN.uv_MainTex).a;
		}
		ENDCG
	}
	FallBack "Diffuse"
	CustomEditor "TileShaderEditor"
}