// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

//////////////////////////////////////////////////////////////
/////// BEN! look at this and make sure it's efficient ///////
/////// Was free from Unity on the asset store         ///////
//////////////////////////////////////////////////////////////
//Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FX/Gem Inner"
{
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_ReflectionStrength ("Reflection Strength", Range(0.0,10.0)) = 1.0
		_EnvironmentLight ("Environment Light", Range(0.0,10.0)) = 1.0
		_Emission ("Emission", Range(0.0,2.0)) = 0.0
		[NoScaleOffset] _RefractTex ("Refraction Texture", Cube) = "" {}
	}
	SubShader {
		Tags {
			"Queue" = "Geometry+100"
		}

		// First pass - here we render the backfaces of the diamonds. Since those diamonds are more-or-less
		// convex objects, this is effectively rendering the inside of them.
		Pass {

			Cull Front
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing

			#include "UnityCG.cginc"
        
			struct v2f {
				float4 pos : SV_POSITION;
				float3 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            struct appdata
            {
                float4 v : POSITION;
				float3 n : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
#define _Color_arr Props
				UNITY_DEFINE_INSTANCED_PROP(half, _EnvironmentLight)
#define _EnvironmentLight_arr Props
				UNITY_DEFINE_INSTANCED_PROP(half, _ReflectionStrength)
#define _ReflectionStrength_arr Props
				UNITY_DEFINE_INSTANCED_PROP(half, _Emission)
#define _Emission_arr Props
            UNITY_INSTANCING_BUFFER_END(Props)

			v2f vert (appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o); // necessary only if you want to access instanced properties in the fragment Shader.

				o.pos = UnityObjectToClipPos(v.v);

				// TexGen CubeReflect:
				// reflect view direction along the normal, in view space.
				float3 viewDir = normalize(ObjSpaceViewDir(v.v));
				o.uv = -reflect(viewDir, v.n);
				o.uv = mul(unity_ObjectToWorld, float4(o.uv,0));
				return o;
			}

			samplerCUBE _RefractTex;

			half4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);

				half3 refraction = texCUBE(_RefractTex, i.uv).rgb * UNITY_ACCESS_INSTANCED_PROP(_Color_arr, _Color).rgb;
				half4 reflection = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, i.uv);
				reflection.rgb = DecodeHDR (reflection, unity_SpecCube0_HDR);
				half3 multiplier = reflection.rgb * UNITY_ACCESS_INSTANCED_PROP(_EnvironmentLight_arr, _EnvironmentLight) + UNITY_ACCESS_INSTANCED_PROP(_Emission_arr, _Emission);
				return half4(refraction.rgb * multiplier.rgb, 1.0f);
			}
			ENDCG 
		}

		// No shader caster path as this makes the depth prepass render all crystal pickups(!)
	}
}
