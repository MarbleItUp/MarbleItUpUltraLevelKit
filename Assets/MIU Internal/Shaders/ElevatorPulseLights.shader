// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MIU/ElevatorPulseLights"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Metal("Metal", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_Emissive("Emissive", 2D) = "white" {}
		_IntensityMin("IntensityMin", Range( 0 , 10)) = 0
		_IntensityMax("IntensityMax", Range( 0 , 10)) = 1
		_PulseSpeed("PulseSpeed", Range( 1 , 10)) = 4
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float _PulseSpeed;
		uniform float _IntensityMin;
		uniform float _IntensityMax;
		uniform sampler2D _Emissive;
		uniform float4 _Emissive_ST;
		uniform sampler2D _Metal;
		uniform float4 _Metal_ST;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			o.Albedo = tex2D( _Albedo, uv_Albedo ).rgb;
			float2 uv_Emissive = i.uv_texcoord * _Emissive_ST.xy + _Emissive_ST.zw;
			o.Emission = ( (_IntensityMin + (sin( ( _PulseSpeed * _Time.y ) ) - -1) * (_IntensityMax - _IntensityMin) / (1 - -1)) * tex2D( _Emissive, uv_Emissive ) ).rgb;
			float2 uv_Metal = i.uv_texcoord * _Metal_ST.xy + _Metal_ST.zw;
			o.Metallic = tex2D( _Metal, uv_Metal ).r;
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
50;68;1693;780;37.12598;1298.383;1;True;True
Node;AmplifyShaderEditor.TimeNode;34;-1328.344,-1403.177;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;40;-1349.072,-1584.379;Float;False;Property;_PulseSpeed;PulseSpeed;7;0;Create;True;0;4;3;1;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;34;-1327.344,-1403.177;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-917.2064,-1466.347;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-973.614,-1015.994;Float;False;Property;_IntensityMax;IntensityMax;6;0;Create;True;0;1;3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-969.9603,-1115.938;Float;False;Property;_IntensityMin;IntensityMin;5;0;Create;True;0;0;0.1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;37;-631.8163,-1392.939;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-732.614,-997.9937;Float;False;Property;_IntensityMax;IntensityMax;6;0;Create;True;0;1;3;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-969.9603,-1115.938;Float;False;Property;_IntensityMin;IntensityMin;5;0;Create;True;0;0;0.1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-701.9741,-800.3206;Float;True;Property;_Emissive;Emissive;4;0;Create;True;0;48b6b239cf6dfbf41930cfe56138c00c;48b6b239cf6dfbf41930cfe56138c00c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;38;-357.2684,-1249.349;Float;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;195.3778,-1015.753;Float;True;2;2;0;FLOAT;0;False;1;COLOR;2,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-43.02431,-1669.353;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;ad1468ef922dcd445be69f1a02ea6013;ad1468ef922dcd445be69f1a02ea6013;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;73.44913,-636.3885;Float;True;Property;_Metal;Metal;2;0;Create;True;0;12d3b7f4f7a124a4ca6fdd06497cd2ef;12d3b7f4f7a124a4ca6fdd06497cd2ef;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;4;-106.0273,-1475.514;Float;True;Property;_Normal;Normal;1;0;Create;True;0;8202247eff139ac40b24ae3785253a8c;8202247eff139ac40b24ae3785253a8c;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;73.44913,-636.3885;Float;True;Property;_Metal;Metal;2;0;Create;True;0;12d3b7f4f7a124a4ca6fdd06497cd2ef;12d3b7f4f7a124a4ca6fdd06497cd2ef;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;511.2951,-592.2595;Float;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-45.22009,-1669.353;Float;True;Property;_Albedo;Albedo;0;0;Create;True;0;ad1468ef922dcd445be69f1a02ea6013;ad1468ef922dcd445be69f1a02ea6013;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;195.3778,-1015.753;Float;True;2;2;0;FLOAT;0;False;1;COLOR;2,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1017.146,-1204.766;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MIU/ElevatorPulseLights;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;41;0;40;0
WireConnection;41;1;34;2
WireConnection;37;0;41;0
WireConnection;38;0;37;0
WireConnection;38;3;35;0
WireConnection;38;4;36;0
WireConnection;44;0;38;0
WireConnection;44;1;6;0
WireConnection;0;0;1;0
WireConnection;0;1;4;0
WireConnection;0;2;44;0
WireConnection;0;3;5;0
WireConnection;0;4;51;0
ASEEND*/
//CHKSM=87399C64CC0A94B210AB4F57F941E14A9F20FAFC
