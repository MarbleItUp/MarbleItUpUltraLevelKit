// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MIU/RainbowAdditive-FullVertex"
{
	Properties
	{
		[Toggle]_ToggleSwitch0("Change Rainbow Direction", Float) = 1
		_Speed("Speed", Range( -100 , 100)) = 2
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float _Scale;
		uniform float _ToggleSwitch0;
		uniform float _Speed;
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
			float HueGradient30 = fmod( ( ( ( ( 1 / _Scale ) * lerp(ase_vertex3Pos.x,ase_vertex3Pos.y,_ToggleSwitch0) ) + (( _Speed * _Time )).x ) * UNITY_PI ) , 1 );
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
1927;87;1906;1044;1205.084;536.6259;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;170;-3034.644,574.8189;Float;False;2377.06;920.5361;Comment;11;26;27;10;106;105;107;3;11;30;201;206;Scan Lines;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-3074.061,1043.207;Float;False;Property;_Speed;Speed;2;0;Create;True;0;2;-10;-100;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-3045.074,638.2322;Float;False;Property;_Scale;Scale;3;0;Create;True;0;10;10;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;26;-2999.543,1220.855;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;213;-3436.285,848.8172;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;212;-3120.285,846.8173;Float;False;Property;_ToggleSwitch0;Change Rainbow Direction;1;0;Create;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;201;-2767.669,784.8218;Float;False;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2660.706,1070.032;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-2614.166,804.2753;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;105;-2417.398,1035.307;Float;True;True;False;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3;-2294.866,885.7679;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;107;-2136.346,1133.916;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-2105.655,739.0015;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleRemainderNode;206;-1782.508,689.4019;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1554.199,-447.7565;Float;False;30;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-924.644,885.7119;Float;False;HueGradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;174;-1833.073,-192.95;Float;True;Property;_EmissionMap_ST;Emission Alpha;6;0;Create;False;0;None;ccb94f5df1fc7014f8b4283bfbed8f05;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;200;-1568.715,-357.1104;Float;False;Property;_Saturation;Saturation;4;0;Create;True;0;0.8;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;214;-682.6767,-53.0374;Float;False;Property;_Intensity;Intensity;5;0;Create;True;0;3;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;199;-889.8348,-360.7621;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;215;-225.1633,-225.3551;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;89.8217,-401.0934;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MIU/RainbowAdditive-FullVertex;False;False;False;False;True;True;True;True;True;True;False;False;False;False;True;False;False;False;False;Back;0;0;False;0;0;False;0;Custom;0.5;True;True;0;True;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;4;One;One;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;212;0;213;1
WireConnection;212;1;213;2
WireConnection;201;1;10;0
WireConnection;27;0;6;0
WireConnection;27;1;26;0
WireConnection;106;0;201;0
WireConnection;106;1;212;0
WireConnection;105;0;27;0
WireConnection;3;0;106;0
WireConnection;3;1;105;0
WireConnection;11;0;3;0
WireConnection;11;1;107;0
WireConnection;206;0;11;0
WireConnection;30;0;206;0
WireConnection;199;0;114;0
WireConnection;199;1;200;0
WireConnection;199;2;174;0
WireConnection;215;0;199;0
WireConnection;215;1;214;0
WireConnection;0;2;215;0
ASEEND*/
//CHKSM=0FCFC47062FFDAA77771CD864FD10B7CD722E5AF