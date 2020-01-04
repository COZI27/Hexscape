Shader "Tutorials/HexShader"
{
	Properties
	{
		_MainTexture("Main Texture", 2D) = "white" {}
		//_MaskTexture("Mask", 2D) = "white" {}
		_Colour("Colour", Color) = (1, 1, 1, 1)
		//_AnimationSpeed("Animation Speed", Range(0, 3)) = 0
		//_OffsetSize("Offset Size", Range(0, 10)) = 0
		//_Threshold("Cutout threshold", Range(0,1)) = 0.1
		//_Softness("Cutout softness", Range(0,0.5)) = 0.0
		_Angle("Angle", Range(0, 360)) = 0
		_Arc1("Arc Point 1", Range(0, 360)) = 15
		_Arc2("Arc Point 2", Range(0, 360)) = 15
	}
		SubShader
		{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "PreviewType" = "Plane"}
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vertexFunc alpha
				#pragma fragment fragmentFunc alpha
				
				#define PI 3.14159265359

				#include "UnityCG.cginc"

				struct appData
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f { // Vert to Frag
					float4 position : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 worldSpacePosition : TEXCOORD1;
				};


				fixed4 _Colour;
				sampler2D _MainTexture;
				//sampler2D _MaskTexture;				
				//float _Threshold;
				//float _Softness;
				//float _AnimationSpeed;
				//float _OffsetSize;
				float _Angle;
				float _Arc1;
				float _Arc2;


				v2f vertexFunc(appData IN)
				{
					v2f OUT;

					//IN.vertex.y += sin(_Time.y * _AnimationSpeed + IN.vertex.x * _OffsetSize);
					OUT.position = UnityObjectToClipPos(IN.vertex);
					OUT.worldSpacePosition = mul(unity_ObjectToWorld, IN.vertex);
					OUT.uv = IN.uv;

					//OUT.uv.xy = IN.uv.xy + frac(_Time.y * float2(1, 1));

					return OUT;
				}

				//fixed4 fragmentFunc(v2f IN) : SV_Target
				//{

				//	fixed4 pixelColour = tex2D(_MaskTexture, IN.uv) *
				//	lerp(fixed4(0.50f,0.8f,1.f,0.f), fixed4(0.4f,0.3f,0.8f,0.f), abs(sin(IN.worldSpacePosition.x * cos(_Time.y)))); //  IN.worldSpacePosition.x);// * abs(sin(_Time.y * 5)) );
				//	//fixed4(IN.worldSpacePosition.x, IN.worldSpacePosition.z, abs(sin(_Time.y * 4)), 0); //fixed4(IN.worldSpacePositon.x, IN.worldSpacePositon.y, abs(sin(_Time.y)) , 0 ) ;
				//	//fixed4 pixelColour = tex2D(_MaskTexture, IN.uv) * float4(IN.position.x, IN.position.y, abs(sin(0.5f)), 0.f);//* _Colour; // * fixed4(IN.uv.x, IN.uv.y, IN.uv.x, 0.0f);
				//	pixelColour.a = smoothstep(_Threshold, _Threshold + _Softness, 0.333 * (pixelColour.r + pixelColour.g + pixelColour.b)) * _Colour.a;


				//	return pixelColour;
				//}


				fixed4 fragmentFunc(v2f IN) : SV_Target
				{

					//fixed4 pixelColour = tex2D(_MainTexture, IN.uv) *
					//lerp(fixed4(0.50f,0.8f,1.f,0.f), fixed4(0.4f,0.3f,0.8f,0.f), abs(sin(IN.worldSpacePosition.x * cos(_Time.y))));
					//pixelColour.a = smoothstep(_Threshold, _Threshold + _Softness, 0.333 * (pixelColour.r + pixelColour.g + pixelColour.b)) * _Colour.a;


					fixed4 pixelColour;

					float size = 1.2; // temp
					float width = 1.4; // temp
					float rotation = _Time.y * 1.5; // temp
					rotation = 0; // temp

					float2 uvOffsetOuter = (IN.uv * size - (size / 2));
					float2 uvOffsetInner = (IN.uv * width - (width / 2));

					int N = 6;
					//Outer
					float a = atan2(uvOffsetOuter.x, uvOffsetOuter.y) + rotation; // +_Time.y / 20 * IN.worldSpacePosition.y;
					float b = 6.28319 / float(N);
					fixed3 outer = smoothstep(0.5, .54, cos(floor(.5 + a / b) * b - a) * length(uvOffsetOuter.xy));

					//Inner
					float c = atan2(uvOffsetOuter.x, uvOffsetOuter.y) + rotation; // +_Time.y / 20 * IN.worldSpacePosition.y;
					float d = 6.28319 / float(N);
					fixed3 inner = smoothstep(0.5, .52, cos(floor(.5 + c / d) * d - c) * length(uvOffsetInner.xy));

					fixed3 combined = lerp(1 - outer, 1 -  inner, normalize(inner));

					combined = 1 - (outer + (1 - inner));

					float2 translate = float2(cos(_Time.y * 1), sin(_Time.y * 20));
					fixed4 textureCol = tex2D(_MainTexture, IN.uv + translate * 0.35);
					pixelColour = textureCol * _Colour;



					//-------- Creating arc --------//
					// sector start/end angles
					float startAngle = _Angle - _Arc1;
					float endAngle = _Angle + _Arc2;

					// check offsets
					float offset0 = clamp(0, 360, startAngle + 360);
					float offset360 = clamp(0, 360, endAngle - 360);

					// convert uv to atan coordinates
					float2 atan2Coord = float2(lerp(-1, 1, IN.uv.x), lerp(-1, 1, IN.uv.y));
					float atanAngle = atan2(atan2Coord.y, atan2Coord.x) * 57.3; // angle in degrees

					// convert angle to 360 system
					if (atanAngle < 0) atanAngle = 360 + atanAngle;


					//pixelColour = tex2D(_MainTexture, IN.uv) * _Colour;
					pixelColour.a = fixed4(combined, 1.) * _Colour.a;

					if (atanAngle >= startAngle && atanAngle <= endAngle) discard;
					if (atanAngle <= offset360) discard;
					if (atanAngle >= offset0) discard;



					//pixelColour.a = smoothstep(_Threshold, _Threshold + _Softness, 0.333 * (pixelColour.r + pixelColour.g + pixelColour.b)) * _Colour.a;

					return pixelColour;
				}

				ENDCG
			}
		}
}
