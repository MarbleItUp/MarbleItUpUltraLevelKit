// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MIU/RainbowOpaque"
{
	Properties
	{
		_Speed("Speed", Range( -100 , 100)) = 44.48342
		_Scale("Scale", Range( 0 , 1000)) = 1
		_Saturation("Saturation", Range( 0 , 1)) = 1
		_TextureSample0("Normal Map", 2D) = "bump" {}
		_TextureSample1("Metallic", 2D) = "white" {}
		_EmissionMap_ST("Emission Alpha", 2D) = "white" {}
		_Intensity("Intensity", Range( 0 , 10)) = 3
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.2
		[Toggle]_ToggleSwitch0("Change Rainbow Direction", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
		[Header(Forward Rendering Options)]
		[ToggleOff] _GlossyReflections("Reflections", Float) = 1.0
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		ZTest LEqual
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _GLOSSYREFLECTIONS_OFF
		#pragma surface surf Standard keepalpha noshadow nodynlightmap 
		struct Input
		{
			half2 uv_texcoord;
		};

		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform half _Scale;
		uniform half _ToggleSwitch0;
		uniform half _Speed;
		uniform half _Saturation;
		uniform sampler2D _EmissionMap_ST;
		uniform float4 _EmissionMap_ST_ST;
		uniform half _Intensity;
		uniform sampler2D _TextureSample1;
		uniform float4 _TextureSample1_ST;
		uniform half _Smoothness;


		half3 HSVToRGB( half3 c )
		{
			half4 K = half4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			half3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			o.Normal = UnpackNormal( tex2D( _TextureSample0, uv_TextureSample0 ) );
			float HueGradient30 = fmod( ( ( ( ( 1.0 / _Scale ) * lerp(i.uv_texcoord.x,i.uv_texcoord.y,_ToggleSwitch0) ) + (( _Speed * _Time )).x ) * UNITY_PI ) , 1.0 );
			float2 uv_EmissionMap_ST = i.uv_texcoord * _EmissionMap_ST_ST.xy + _EmissionMap_ST_ST.zw;
			float3 hsvTorgb199 = HSVToRGB( half3(HueGradient30,_Saturation,tex2D( _EmissionMap_ST, uv_EmissionMap_ST ).r) );
			o.Emission = ( hsvTorgb199 * _Intensity );
			float2 uv_TextureSample1 = i.uv_texcoord * _TextureSample1_ST.xy + _TextureSample1_ST.zw;
			o.Metallic = tex2D( _TextureSample1, uv_TextureSample1 ).r;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
1995;66;1778;940;2310.628;981.421;1.475;True;False
Node;AmplifyShaderEditor.CommentaryNode;170;-3034.644,574.8189;Float;False;2377.06;920.5361;Comment;11;26;27;10;106;105;107;3;11;30;201;206;Scan Lines;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-3074.061,1043.207;Float;False;Property;_Speed;Speed;0;0;Create;True;0;0;False;0;44.48342;-1;-100;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;171;-3433.434,840.8839;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-3045.074,638.2322;Float;False;Property;_Scale;Scale;1;0;Create;True;0;0;False;0;1;10;0;1000;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;26;-2999.543,1220.855;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;212;-3120.285,846.8173;Float;False;Property;_ToggleSwitch0;Change Rainbow Direction;8;0;Create;False;0;0;False;0;1;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;201;-2767.669,784.8218;Float;False;2;0;FLOAT;1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-2660.706,1070.032;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;105;-2417.398,1035.307;Float;True;True;False;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-2614.166,804.2753;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PiNode;107;-2136.346,1133.916;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;3;-2294.866,885.7679;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;11;-2105.655,739.0015;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleRemainderNode;206;-1782.508,689.4019;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-924.644,885.7119;Float;False;HueGradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;114;-1554.199,-447.7565;Float;False;30;HueGradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;200;-1568.715,-357.1104;Float;False;Property;_Saturation;Saturation;2;0;Create;True;0;0;False;0;1;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;174;-1833.073,-192.95;Float;True;Property;_EmissionMap_ST;Emission Alpha;5;0;Create;False;0;0;False;0;None;ccb94f5df1fc7014f8b4283bfbed8f05;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.HSVToRGBNode;199;-889.8348,-360.7621;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;213;-873.2736,-78.6186;Float;False;Property;_Intensity;Intensity;6;0;Create;True;0;0;False;0;3;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;220;-832.6769,114.5035;Float;True;Property;_TextureSample1;Metallic;4;0;Create;False;0;0;False;0;None;ccb94f5df1fc7014f8b4283bfbed8f05;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;214;-321.7603,-275.9363;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;217;-809.2515,304.7788;Float;False;Property;_Smoothness;Smoothness;7;0;Create;True;0;0;False;0;0.2;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;215;-1017.227,-742.4695;Float;True;Property;_TextureSample0;Normal Map;3;0;Create;False;0;0;False;0;None;ccb94f5df1fc7014f8b4283bfbed8f05;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;89.8217,-401.0934;Half;False;True;2;Half;ASEMaterialInspector;0;0;Standard;MIU/RainbowOpaque;False;False;False;False;False;False;False;True;False;False;False;False;False;False;True;False;True;False;False;True;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;True;0;Opaque;0.5;True;False;0;True;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;False;0;1;False;-1;1;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;212;0;171;1
WireConnection;212;1;171;2
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
WireConnection;206;0;11;0
WireConnection;30;0;206;0
WireConnection;199;0;114;0
WireConnection;199;1;200;0
WireConnection;199;2;174;0
WireConnection;214;0;199;0
WireConnection;214;1;213;0
WireConnection;0;1;215;0
WireConnection;0;2;214;0
WireConnection;0;3;220;0
WireConnection;0;4;217;0
ASEEND*/
//CHKSM=EA4D394660B528733B06078B11218391488001A6