// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MIU/TriplanarSnow"
{
	Properties
	{
		_AlbedoTiling("Albedo Tiling", Float) = 2
		_BlendFalloff("Blend Falloff", Float) = 5
		_Metallic("Metallic", Range( 0 , 1)) = 2
		_Smoothness("Smoothness", Range( 0 , 1)) = 2
		_NormalTiling("Normal Tiling", Float) = 2
		_NormalTop("Normal Top", 2D) = "bump" {}
		_Normal("Normal", 2D) = "bump" {}
		_AlbedoTop("Albedo Top", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma multi_compile_instancing
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) fixed3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalTop;
		uniform sampler2D _Normal;
		uniform half _NormalTiling;
		uniform half _BlendFalloff;
		uniform sampler2D _AlbedoTop;
		uniform sampler2D _Albedo;
		uniform half _AlbedoTiling;
		uniform half _Metallic;
		uniform half _Smoothness;


		inline float3 TriplanarSamplingCNF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm = ( tex2D( midTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			yNormN = ( tex2D( botTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( midTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			xNorm.xyz = half3( UnpackNormal( xNorm ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz = half3( UnpackNormal( yNorm ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz = half3( UnpackNormal( zNorm ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			yNormN.xyz = half3( UnpackNormal( yNormN ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + yNormN.xyz * negProjNormalY + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSamplingCF( sampler2D topTexMap, sampler2D midTexMap, sampler2D botTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			float negProjNormalY = max( 0, projNormal.y * -nsign.y );
			projNormal.y = max( 0, projNormal.y * nsign.y );
			half4 xNorm; half4 yNorm; half4 yNormN; half4 zNorm;
			xNorm = ( tex2D( midTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			yNormN = ( tex2D( botTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( midTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + yNormN * negProjNormalY + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			half3 ase_worldTangent = WorldNormalVector( i, half3( 1, 0, 0 ) );
			half3 ase_worldBitangent = WorldNormalVector( i, half3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 triplanar339 = TriplanarSamplingCNF( _NormalTop, _Normal, _Normal, ase_worldPos, ase_worldNormal, _BlendFalloff, _NormalTiling, float3(0,0,0) );
			float3 tanTriplanarNormal339 = mul( ase_worldToTangent, triplanar339 );
			o.Normal = tanTriplanarNormal339;
			float4 triplanar337 = TriplanarSamplingCF( _AlbedoTop, _Albedo, _Albedo, ase_worldPos, ase_worldNormal, _BlendFalloff, _AlbedoTiling, float3(0,0,0) );
			o.Albedo = triplanar337.xyz;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred nodynlightmap nofog noforwardadd 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 tSpace0 : TEXCOORD1;
				float4 tSpace1 : TEXCOORD2;
				float4 tSpace2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				fixed3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				fixed tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				fixed3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
987;486;1781;875;4.016724;96.03958;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;338;529.7946,182.6389;Float;False;Property;_AlbedoTiling;Albedo Tiling;0;0;Create;True;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;362;116.1805,405.7134;Float;True;Property;_Normal;Normal;6;0;Create;True;0;33a91cc0367ae4246b76c4467d5217de;33a91cc0367ae4246b76c4467d5217de;True;bump;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;348;658.6306,601.8474;Float;False;Property;_NormalTiling;Normal Tiling;4;0;Create;True;0;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;360;531.1805,-18.28656;Float;True;Property;_Albedo;Albedo;8;0;Create;True;0;a5ccf2fb89d66f14aab432878291234b;5bbb03aa3d155064cac86cb5c341b6f7;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;359;530.1805,-213.2866;Float;True;Property;_AlbedoTop;Albedo Top;7;0;Create;True;0;c09d9670746e01e4a98ccb603d8a7552;7f176c93e05bea540bd186fa0c218237;False;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;341;565.7946,432.6389;Float;True;Property;_BlendFalloff;Blend Falloff;1;0;Create;True;0;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;361;108.1805,149.7134;Float;True;Property;_NormalTop;Normal Top;5;0;Create;True;0;21bd5809850aad340a72d1ba22af34f8;10ff51d2d87fb7b46b70b55f8551c146;True;bump;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TriplanarNode;339;970.795,315.6389;Float;True;Cylindrical;World;True;Top Texture 0;_TopTexture0;white;4;None;Normal Mid;_NormalMid;white;0;None;Normal Bot;_NormalBot;white;1;None;Triplanar Normal;False;8;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;354;999.778,673.2098;Float;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;2;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;337;965.803,105.6904;Float;True;Cylindrical;World;False;Top Texture 0;_TopTexture0;white;5;None;Albedo Mid;_AlbedoMid;white;2;None;Albedo Bot;_AlbedoBot;white;3;None;Triplanar Albedo;False;8;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;3;FLOAT;1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;355;927.1805,590.7134;Float;False;Property;_Metallic;Metallic;2;0;Create;True;0;2;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1810.899,136.9;Half;False;True;6;Half;ASEMaterialInspector;0;0;Standard;MIU/TriplanarSnow;False;False;False;False;False;False;False;True;False;True;False;True;False;False;False;False;True;False;False;Back;0;3;False;0;0;True;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;339;0;361;0
WireConnection;339;1;362;0
WireConnection;339;2;362;0
WireConnection;339;3;348;0
WireConnection;339;4;341;0
WireConnection;337;0;359;0
WireConnection;337;1;360;0
WireConnection;337;2;360;0
WireConnection;337;3;338;0
WireConnection;337;4;341;0
WireConnection;0;0;337;0
WireConnection;0;1;339;0
WireConnection;0;3;355;0
WireConnection;0;4;354;0
ASEEND*/
//CHKSM=37088F9D3214AF7BEDBF6B7ACEC03D9413639A30