// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Skybox/Dithered 6 Sided" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    [Gamma] _Exposure ("Exposure", Range(0, 8)) = 1.0
    _Rotation ("Rotation", Range(0, 360)) = 0
	_RotationAxis("Rotation axis", Vector) = (0, 1, 0)
    [NoScaleOffset] _FrontTex ("Front [+Z]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _BackTex ("Back [-Z]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _LeftTex ("Left [+X]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _RightTex ("Right [-X]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _UpTex ("Up [+Y]   (HDR)", 2D) = "grey" {}
    [NoScaleOffset] _DownTex ("Down [-Y]   (HDR)", 2D) = "grey" {}
    _NoiseBlendScaleFactor ("Noise Blend Scale Factor", Range(0, 8)) = 2
    _NoiseBlendPowerFactor ("Noise Blend Power Factor", Range(0.125, 8)) = 2
    _NoiseClampMaximum ("Noise Blend Maximum", Range(0.0, 1.0)) = 1
    _NoiseClampMinimum ("Noise Blend Minimum", Range(0.0, 1.0)) = 0
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    CGINCLUDE
    #include "UnityCG.cginc"

    half4 _Tint;
    half _Exposure;
    float _Rotation;
	float3 _RotationAxis;
    float _NoiseBlendScaleFactor;
    float _NoiseBlendPowerFactor;
    float _NoiseClampMaximum;
    float _NoiseClampMinimum;

    float3 RotateAroundYInDegrees (float3 vertex, float degrees)
    {
		float alpha = degrees * UNITY_PI / 180.0;
		float sina, cosa;
		sincos(alpha, sina, cosa);
		float2x2 m = float2x2(cosa, -sina, sina, cosa);
		return float3(mul(m, vertex.xz), vertex.y).xzy;
    }

	float4x4 rotationMatrix(float3 axis, float angle)
	{
		axis = normalize(axis);
		float s = sin(angle);
		float c = cos(angle);
		float oc = 1.0 - c;

		return float4x4(oc * axis.x * axis.x + c, oc * axis.x * axis.y - axis.z * s, oc * axis.z * axis.x + axis.y * s, 0.0,
			oc * axis.x * axis.y + axis.z * s, oc * axis.y * axis.y + c, oc * axis.y * axis.z - axis.x * s, 0.0,
			oc * axis.z * axis.x - axis.y * s, oc * axis.y * axis.z + axis.x * s, oc * axis.z * axis.z + c, 0.0,
			0.0, 0.0, 0.0, 1.0);
	}

    struct appdata_t {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    struct v2f {
        float4 vertex : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

	// note: valve edition
	//       from http://alex.vlachos.com/graphics/Alex_Vlachos_Advanced_VR_Rendering_GDC2015.pdf
	// note: input in pixels (ie not normalized uv)
	float3 ScreenSpaceDither( float2 vScreenPos )
	{
		// Iestyn's RGB dither (7 asm instructions) from Portal 2 X360, slightly modified for VR
		//vec3 vDither = vec3( dot( vec2( 171.0, 231.0 ), vScreenPos.xy + iTime ) );
		float t = dot( float2( 171.0, 231.0 ), vScreenPos.xy );
		float3 vDither = float3( t,t,t );
		vDither.rgb = frac( vDither.xyz / float3( 103.0, 71.0, 97.0 ) );
		return vDither.rgb / 255.0; //note: looks better without 0.375...

		//note: not sure why the 0.5-offset is there...
		//vDither.rgb = fract( vDither.rgb / vec3( 103.0, 71.0, 97.0 ) ) - vec3( 0.5, 0.5, 0.5 );
		//return (vDither.rgb / 255.0) * 0.375;
	}

    v2f vert (appdata_t v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        //float3 rotated = RotateAroundYInDegrees(v.vertex, _Rotation);
		float3 rotated = mul(rotationMatrix(normalize(_RotationAxis.xyz), _Rotation * UNITY_PI / 180.0), v.vertex).xyz;
        o.vertex = UnityObjectToClipPos(rotated);
        o.texcoord = v.texcoord;
        return o;
    }

	float toLinear(float v) {
		return pow(v, 2.2);
	}

	float2 toLinear(float2 v) {
		return pow(v, 2.2);
	}

	float3 toLinear(float3 v) {
		return pow(v, 2.2);
	}

	float4 toLinear(float4 v) {
		return float4(toLinear(v.rgb), v.a);
	}

	float toGamma(float v) {
		return pow(v, 1.0 / 2.2);
	}

	float2 toGamma(float2 v) {
		return pow(v, 1.0 / 2.2);
	}

	float3 toGamma(float3 v) {
		return pow(v, 1.0 / 2.2); 
	}

	float4 toGamma(float4 v) {
		return float4(toGamma(v.rgb), v.a);
	}

	// Noise due to https://www.shadertoy.com/view/4ssXRX

	//note: uniformly distributed, normalized rand, [0;1[
	float nrand( float2 n )
	{
		return frac(sin(dot(n.xy, float2(12.9898, 78.233)))* 43758.5453);
	}

	float n4rand( float2 n )
	{
		float t = _SinTime.w * 2.0;
		float nrnd0 = nrand( n + 0.07*t );
		float nrnd1 = nrand( n + 0.11*t );	
		float nrnd2 = nrand( n + 0.13*t );
		float nrnd3 = nrand( n + 0.17*t );
		return (nrnd0+nrnd1+nrnd2+nrnd3) / 4.0;
	}

	float n8rand( float2 n )
	{
		float t = _SinTime.w * 2.0;
		float nrnd0 = nrand( n + 0.07*t );
		float nrnd1 = nrand( n + 0.11*t );	
		float nrnd2 = nrand( n + 0.13*t );
		float nrnd3 = nrand( n + 0.17*t );
		
		float nrnd4 = nrand( n + 0.19*t );
		float nrnd5 = nrand( n + 0.23*t );
		float nrnd6 = nrand( n + 0.29*t );
		float nrnd7 = nrand( n + 0.31*t );
		
		return (nrnd0+nrnd1+nrnd2+nrnd3+nrnd4+nrnd5+nrnd6+nrnd7) / 8.0;
	}

    float4 skybox_frag (v2f i, sampler2D smp, float4 smpDecode)
	{
		float2 ditherPos = 0.5*(i.vertex.xy+1.0) * _ScreenParams.xy;

        float3 tex = tex2D (smp, i.texcoord).xyz;
		tex = toLinear(tex);

		float3 c = tex * _Tint.rgb * unity_ColorSpaceDouble.rgb;
        c *= _Exposure;

		float blendFactor = pow(length(c), _NoiseBlendPowerFactor);
		blendFactor = clamp(blendFactor * _NoiseBlendScaleFactor, _NoiseClampMinimum, _NoiseClampMaximum);
		float noise = n4rand(i.vertex.xy);

		c *= lerp(noise, 1, blendFactor);
		c = toGamma(c);
        
		return float4(c, 1);
    }
    ENDCG

    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _FrontTex;
        half4 _FrontTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_FrontTex, _FrontTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _BackTex;
        half4 _BackTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_BackTex, _BackTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _LeftTex;
        half4 _LeftTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_LeftTex, _LeftTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _RightTex;
        half4 _RightTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_RightTex, _RightTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _UpTex;
        half4 _UpTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_UpTex, _UpTex_HDR); }
        ENDCG
    }
    Pass{
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 2.0
        sampler2D _DownTex;
        half4 _DownTex_HDR;
        half4 frag (v2f i) : SV_Target { return skybox_frag(i,_DownTex, _DownTex_HDR); }
        ENDCG
    }
}
}
