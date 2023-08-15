// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MIU/ChevronSurface"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_AnimationSpeed("AnimationSpeed", Float) = 2
		[HDR]_ColorMultiply("ColorMultiply", Color) = (1,1,1,0.7843137)
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Off
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		

		Pass
		{
			Name "Unlit"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			uniform sampler2D _TextureSample0;
			uniform float _AnimationSpeed;
			uniform float4 _ColorMultiply;
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float2 uv5 = i.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult34 = clamp( ( tan( ( _Time.y * _AnimationSpeed ) ) * 0.5 ) , 0.0 , 0.5 );
				
				
				finalColor = ( tex2D( _TextureSample0, ( uv5 + ( float2( -1,0 ) * clampResult34 ) ) ) * _ColorMultiply );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16200
1990;44;1778;940;1711.826;504.1115;1;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;24;-1854.792,-114.7029;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-1846.304,137.9424;Float;False;Property;_AnimationSpeed;AnimationSpeed;1;0;Create;True;0;0;False;0;2;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1604.703,38.74261;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TanOpNode;33;-1326.794,49.64534;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;44;-1340.704,326.7422;Float;False;Constant;_Float0;Float 0;1;0;Create;True;0;0;False;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-1111.905,153.9423;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;8;-1125.096,-272.8092;Float;False;Constant;_Vector0;Vector 0;1;0;Create;True;0;0;False;0;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ClampOpNode;34;-876.3181,12.64532;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-598.6751,-273.7134;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-1156.449,-556.3018;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;27;-292.4365,-300.748;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;49;-179.043,-0.07474804;Float;False;Property;_ColorMultiply;ColorMultiply;2;1;[HDR];Create;True;0;0;False;0;1,1,1,0.7843137;1.2,1.2,1.2,0.9;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-69.99998,-309.5997;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;a234ec03c256d7248844b752a66cffd7;9c7f63918f1a3bb40af340541a90845b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;309.7568,-206.7746;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;48;630.7993,-159.1001;Float;False;True;2;Float;ASEMaterialInspector;0;1;MIU/ChevronSurface;0770190933193b94aaa3065e307002fa;0;0;Unlit;2;True;0;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;True;0;False;-1;0;False;-1;True;False;True;2;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;0;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;45;0;24;0
WireConnection;45;1;46;0
WireConnection;33;0;45;0
WireConnection;43;0;33;0
WireConnection;43;1;44;0
WireConnection;34;0;43;0
WireConnection;28;0;8;0
WireConnection;28;1;34;0
WireConnection;27;0;5;0
WireConnection;27;1;28;0
WireConnection;1;1;27;0
WireConnection;50;0;1;0
WireConnection;50;1;49;0
WireConnection;48;0;50;0
ASEEND*/
//CHKSM=D57ECCD4D82B58344F124844A0C086ABF7D0D917