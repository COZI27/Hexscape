Shader "Custom/WorldSpaceRippleShader_B"
{
	Properties
	{
		_Colour("Colour", Color) = (1, 1, 1, 1)
		_MainTexture("Main Texture", 2D) = "white" {}
		//_Glossiness("Smoothness", Range(0,1)) = 0.5
		//_Metallic("Metallic", Range(0,1)) = 0.0

		_RippleOrigin("Ripple Origin", Vector) = (0,0,0,0)
		_RippleDistance("Ripple Distance", Float) = 0 // DEPRECATED?
		_RippleWidth("Ripple Width", Float) = 0.1
		_RippleRadius("Ripple Radius", float) = 0.0

	}
	SubShader
	{

		//Tags{"RenderType" = "Opaque" "RenderPipeline" = "LightweightPipeline" /*"IgnoreProjector" = "True"*/}
		//Tags {"RenderType" = "Opaque" "PreviewType" = "Plane"}
				Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "PreviewType" = "Plane"}
			//Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

			Pass
			{
				CGPROGRAM
				#pragma vertex vertexFunc alpha
				#pragma fragment fragmentFunc alpha			


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

				sampler2D _MainTexture;
				half _Glossiness;
				half _Metallic;
				fixed4 _Colour;
				fixed4 _RippleOrigin;
				float _RippleDistance;
				float _RippleWidth;
				float _RippleRadius;


				//float circle(in float2 _st, in float _radius) {
				//	float2 dist = _st - 0.5;
				//	return 1. - smoothstep(_radius - (_radius * 0.01),
				//		_radius + (_radius * 0.01),
				//		dot(dist, dist) * 4.0);
				//}

				float circle(float2 position, float radius) {
					return step(radius, length(position));
				}



				v2f vertexFunc(appData IN)
				{
					v2f OUT;

					OUT.position = UnityObjectToClipPos(IN.vertex);;
					OUT.worldSpacePosition = mul(unity_ObjectToWorld, IN.vertex);
					OUT.uv = IN.uv;

					return OUT;
				}


				fixed4 fragmentFunc(v2f IN) : SV_Target
				{

					fixed4 fragCol;
					
					float4 offsetPos = (IN.worldSpacePosition - _RippleOrigin);

					float circleValOuter = circle(offsetPos.xz, _RippleRadius);
					float circleValInner = circle(offsetPos.xz, _RippleRadius - _RippleWidth);
					circleValInner = 1.0 - circleValInner;
					float circleVal = lerp(circleValOuter, circleValInner, normalize(circleValInner));


					fixed4 backgroundCol = tex2D(_MainTexture, IN.uv);
					fixed4 c = lerp(_Colour, backgroundCol, circleVal);

					fragCol = c; // fixed4(c, c, c, 1);

					return fragCol;
				}



				ENDCG
			}

	}
}



/* // BACKUP_A : Draws WOrld SPace Circle on origin point

				fixed4 fragmentFunc(v2f IN) : SV_Target
				{

					fixed4 fragCol;

					//float dist = sqrt(pow(IN.uv.x, 2) + pow(IN.uv.y, 2));

					//float pwidth = length(float2(ddx(dist), ddy(dist)));
					//float alpha = smoothstep(0.5, 0.5 - pwidth * 1.5, dist);


					//fragCol = fixed4(_Colour.r, _Colour.g, _Colour.b, _Colour.a * alpha);

					//float circleCol = length(circle(IN.uv, 0.5));


					//fixed4 c = tex2D(_MainTexture, IN.position.xz) * circleCol;


					float4 offsetPos = (IN.worldSpacePosition + _RippleOrigin);

					float c = circle(offsetPos.xz, 1);
					fragCol = fixed4(c, c, c, 1);
					//fragCol = c;

					return fragCol;
				}


*/