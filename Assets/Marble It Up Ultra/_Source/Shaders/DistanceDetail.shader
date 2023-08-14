// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MIU/DistanceDetail"
{
	Properties
	{
		_NearAlbedo("NearAlbedo", 2D) = "white" {}
		_DistantAlbedo("DistantAlbedo", 2D) = "white" {}
		_Falloff("Falloff", Float) = 10
		_Distance("Distance", Float) = 20
		_NearEmissive("NearEmissive", 2D) = "white" {}
		_DistantEmissive("DistantEmissive", 2D) = "white" {}
		_EmissiveStrength("Emissive Strength", Float) = 0
		_Normal("Normal", 2D) = "bump" {}
		_Metal("Metal", 2D) = "white" {}
		_Metallic("Metallic", Float) = 1
		_Smoothness("Smoothness", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float eyeDepth;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _NearAlbedo;
		uniform float4 _NearAlbedo_ST;
		uniform sampler2D _DistantAlbedo;
		uniform float4 _DistantAlbedo_ST;
		uniform float _Falloff;
		uniform float _Distance;
		uniform sampler2D _NearEmissive;
		uniform float4 _NearEmissive_ST;
		uniform sampler2D _DistantEmissive;
		uniform float4 _DistantEmissive_ST;
		uniform float _EmissiveStrength;
		uniform sampler2D _Metal;
		uniform float4 _Metal_ST;
		uniform float _Metallic;
		uniform float _Smoothness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_NearAlbedo = i.uv_texcoord * _NearAlbedo_ST.xy + _NearAlbedo_ST.zw;
			float2 uv_DistantAlbedo = i.uv_texcoord * _DistantAlbedo_ST.xy + _DistantAlbedo_ST.zw;
			float cameraDepthFade6 = (( i.eyeDepth -_ProjectionParams.y - _Distance ) / _Falloff);
			float clampResult25 = clamp( cameraDepthFade6 , 0 , 1 );
			float4 lerpResult16 = lerp( tex2D( _NearAlbedo, uv_NearAlbedo ) , tex2D( _DistantAlbedo, uv_DistantAlbedo ) , clampResult25);
			o.Albedo = lerpResult16.rgb;
			float2 uv_NearEmissive = i.uv_texcoord * _NearEmissive_ST.xy + _NearEmissive_ST.zw;
			float2 uv_DistantEmissive = i.uv_texcoord * _DistantEmissive_ST.xy + _DistantEmissive_ST.zw;
			float4 lerpResult34 = lerp( tex2D( _NearEmissive, uv_NearEmissive ) , tex2D( _DistantEmissive, uv_DistantEmissive ) , clampResult25);
			o.Emission = ( lerpResult34 * _EmissiveStrength ).rgb;
			float2 uv_Metal = i.uv_texcoord * _Metal_ST.xy + _Metal_ST.zw;
			o.Metallic = ( tex2D( _Metal, uv_Metal ) * _Metallic ).r;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
1927;89;1906;1042;2199.491;1143.394;1.765096;True;True
Node;AmplifyShaderEditor.RangedFloatNode;10;-1412.838,-649.5574;Float;False;Property;_Falloff;Falloff;2;0;Create;True;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1414.229,-446.5793;Float;False;Property;_Distance;Distance;3;0;Create;True;0;20;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CameraDepthFade;6;-1139.275,-530.6472;Float;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;25;-570.5868,-532.0706;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-1240.873,-126.9082;Float;True;Property;_DistantEmissive;DistantEmissive;5;0;Create;True;0;8bb9dfcf5c88a794eb2f0d807cecd2b3;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;32;-1240.511,-329.4454;Float;True;Property;_NearEmissive;NearEmissive;4;0;Create;True;0;92c9ed5e87d8dad48888e3a3a6c097eb;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-753.2373,-951.0603;Float;True;Property;_NearAlbedo;NearAlbedo;0;0;Create;True;0;92c9ed5e87d8dad48888e3a3a6c097eb;92c9ed5e87d8dad48888e3a3a6c097eb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;28;-540.0845,179.5175;Float;True;Property;_Metal;Metal;8;0;Create;True;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-940.2996,-743.4769;Float;True;Property;_DistantAlbedo;DistantAlbedo;1;0;Create;True;0;8bb9dfcf5c88a794eb2f0d807cecd2b3;8bb9dfcf5c88a794eb2f0d807cecd2b3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;34;-421.5521,-345.9997;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-401.1445,440.7658;Float;False;Property;_Metallic;Metallic;9;0;Create;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-406.7623,-115.0707;Float;False;Property;_EmissiveStrength;Emissive Strength;6;0;Create;True;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;16;-210.2751,-562.2992;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-40.47424,-267.8075;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-36.33252,132.209;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;26;-133.6051,-871.8876;Float;True;Property;_Normal;Normal;7;0;Create;True;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;23;80.49348,284.2917;Float;False;Property;_Smoothness;Smoothness;10;0;Create;True;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;342,-225;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MIU/DistanceDetail;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;10;0
WireConnection;6;1;11;0
WireConnection;25;0;6;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;34;2;25;0
WireConnection;16;0;1;0
WireConnection;16;1;2;0
WireConnection;16;2;25;0
WireConnection;35;0;34;0
WireConnection;35;1;36;0
WireConnection;30;0;28;0
WireConnection;30;1;31;0
WireConnection;0;0;16;0
WireConnection;0;1;26;0
WireConnection;0;2;35;0
WireConnection;0;3;30;0
WireConnection;0;4;23;0
ASEEND*/
//CHKSM=D0BC7369F5AB6FB9259F07FC0034B02AC5D8051F