Shader "Particles/Additive (Soft) HDR" {
	Properties{
		[HDR]_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Particle Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0,3.0)) = 1.0
	}

		Category{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			Blend One One
			//ColorMask RGB
			Cull Off Lighting Off ZWrite Off

			SubShader {
				Pass {

					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					#include "UnityCG.cginc"

					sampler2D _MainTex;
					float4 _TintColor;

					struct appdata_t {
						float4 vertex : POSITION;
						float4 color : COLOR;
						float2 texcoord : TEXCOORD0;
					};

					struct v2f {
						float4 vertex : SV_POSITION;
						float4 color : COLOR;
						float2 texcoord : TEXCOORD0;
						float4 projPos : TEXCOORD3;
					};

					float4 _MainTex_ST;

					v2f vert(appdata_t v)
					{
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.projPos = ComputeScreenPos(o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
						o.color = v.color;
						o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
						return o;
					}

					sampler2D_float _CameraDepthTexture;
					float _InvFade;

					float4 frag(v2f i) : SV_Target
					{
						float z = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos));
						float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(z));
						float partZ = i.projPos.z;
						float fade = saturate(_InvFade * (sceneZ - partZ));
						i.color.a *= fade;

						float4 prev = i.color * tex2D(_MainTex, i.texcoord);
						prev.rgb *= prev.a * (_TintColor * 2) * prev.a;
						return prev;
					}
					ENDCG
				}
			}
		}
}