// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MIU/RainbowAdditive-PartialVertex"
{
	Properties
	{
		[Toggle]_ToggleSwitch0("Change Rainbow Direction", Float) = 1
		_HueMin("HueMin", Range( 0 , 1)) = 0.5
		_HueMax("HueMax", Range( 0 , 1)) = 0.8
		_Speed("Speed", Range( -100 , 100)) = 10
		_Scale("Scale", Range( 0 , 1000)) = 10
		_Saturation("Saturation", Range( 0 , 1)) = 0.8
		_Intensity("Intensity", Range( 0 , 10)) = 3
		_EmissionMap_ST("Emission Alpha", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend One One
		BlendOp Add
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _Scale;
		uniform float _ToggleSwitch0;
		uniform float _Speed;
		uniform float _HueMin;
		uniform float _HueMax;
		uniform float _Saturation;
		uniform sampler2D _EmissionMap_ST;
		uniform float4 _EmissionMap_ST_ST;
		uniform float _Intensity;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, clamp( p - K.xxx, 0.0, 1.0 ), c.y );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float clampResult197 = clamp( (_HueMin + (sin( ( ( ( ( 1 / _Scale ) * lerp(ase_vertex3Pos.x,ase_vertex3Pos.y,_ToggleSwitch0) ) + (( _Speed * _Time )).x ) * UNITY_PI ) ) - -1) * (_HueMax - _HueMin) / (1 - -1)) , 0 , 1 );
			float HueGradient30 = clampResult197;
			float2 uv_EmissionMap_ST = i.uv_texcoord * _EmissionMap_ST_ST.xy + _EmissionMap_ST_ST.zw;
			float3 hsvTorgb199 = HSVToRGB( float3(HueGradient30,_Saturation,tex2D( _EmissionMap_ST, uv_EmissionMap_ST ).r) );
			o.Emission = ( hsvTorgb199 * _Intensity );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
1927;87;1906;1044;2068.309;758.7562;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;170;-3034.644,574.8189;Float;False;2377.06;920.5361;Comment;16;26;27;10;106;105;107;3;11;13;30;196;197;201;204;205;212;Scan Lines;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;214;-3496.957,862.3364;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-3156.988,667.4452;Float;False;Property;_Scale;Scale;5;0;Create;True;0;10;3;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;26;-2999.543,1220.855;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-3074.061,1043.207;Float;False;Property;_Speed;Speed;4;0;Create;True;0;10;-10;-100;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;212;-3180.167,870.8378;Float;False;Property;_ToggleSwitch0;Change Rainbow Direction;1;0;Create;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;201;-2767.669,784.8218;Float;False;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2744.706,1065.032;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;105;-2564.398,997.307;Float;True;True;False;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-2614.166,804.2753;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3;-2294.866,885.7679;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;107;-2297.799,1171.031;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-2057.841,874.132;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;205;-2015.917,1391.131;Float;False;Property;_HueMax;HueMax;3;0;Create;True;0;0.8;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;204;-2020.245,1266.511;Float;False;Property;_HueMin;HueMin;2;0;Create;True;0;0.5;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;13;-1719.852,952.9218;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;196;-1489.615,1121.812;Float;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;197;-1219.541,896.9277;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-924.644,885.7119;Float;False;HueGradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;-1568.715,-357.1104;Float;False;Property;_Saturation;Saturation;6;0;Create;True;0;0.8;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;174;-1833.073,-192.95;Float;True;Property;_EmissionMap_ST;Emission Alpha;8;0;Create;False;0;None;ccb94f5df1fc7014f8b4283bfbed8f05;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1554.199,-447.7565;Float;False;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-849.8975,-75.37974;Float;False;Property;_Intensity;Intensity;7;0;Create;True;0;3;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;199;-889.8348,-360.7621;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;219;-392.3834,-247.6975;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;89.8217,-401.0934;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MIU/RainbowAdditive-PartialVertex;False;False;False;False;True;True;True;True;True;True;True;False;False;False;True;False;False;False;False;Back;0;0;False;0;0;False;0;Custom;0.5;True;True;0;True;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;4;One;One;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;212;0;214;1
WireConnection;212;1;214;2
WireConnection;201;1;10;0
WireConnection;27;0;6;0
WireConnection;27;1;26;0
WireConnection;105;0;27;0
WireConnection;106;0;201;0
WireConnection;106;1;212;0
WireConnection;3;0;106;0
WireConnection;3;1;105;0
WireConnection;11;0;3;0
WireConnection;11;1;107;0
WireConnection;13;0;11;0
WireConnection;196;0;13;0
WireConnection;196;3;204;0
WireConnection;196;4;205;0
WireConnection;197;0;196;0
WireConnection;30;0;197;0
WireConnection;199;0;114;0
WireConnection;199;1;200;0
WireConnection;199;2;174;0
WireConnection;219;0;199;0
WireConnection;219;1;220;0
WireConnection;0;2;219;0
ASEEND*/
//CHKSM=DDA16B0718DD250A8052E73799C6ECE5104B1652