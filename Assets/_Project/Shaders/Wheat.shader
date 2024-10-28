Shader "_Project/Wheat"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Masks ("Texture", 2D) = "white" {}

        _Noise("Noise", Float) = 2
        _Speed("Speed", Vector) = (-1, 0.2, 0, 0)
        _Power("Power", Float) = 0.5
        _Split("Split", Float) = 0.5
        _PushPower("PushPower", Float) = 0.5
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha


		Pass
		{
        CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float3 worldPos 	: TEXCOORD1;
				float2 screenPos 	: TEXCOORD2;
			};
			
			half4 _Color;

			float _Noise;
        	float2 _Speed;
        	float _Power;
			float _Split;
			float _PushPower;

			sampler2D _MainTex;
			sampler2D _Masks;
			float4 _MainTex_ST;

			float3 Unity_ColorspaceConversion_Linear_RGB_float(float3 In)
			{
			    float3 sRGBLo = In * 12.92;
			    float3 sRGBHi = (pow(max(abs(In), 1.192092896e-07), float3(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
			    return float3(In <= 0.0031308) ? sRGBLo : sRGBHi;
			}
			float2 unity_gradientNoise_dir(float2 p)
			{
				p = p % 289;
				float x = (34 * p.x + 1) * p.x % 289 + p.y;
				x = (34 * x + 1) * x % 289;
				x = frac(x / 41) * 2 - 1;
				return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
			}

			float unity_gradientNoise(float2 p)
			{
				float2 ip = floor(p);
				float2 fp = frac(p);
				float d00 = dot(unity_gradientNoise_dir(ip), fp);
				float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
				float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
				float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
				fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
				return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
			}
			float1 inverseLerp(float1 a, float1 b, float1 value) 
			{
				return (value - a) / (b - a);
			}
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				float3 worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
				 
				OUT.vertex = UnityObjectToClipPos(IN.vertex); 
				OUT.screenPos = ComputeScreenPos(OUT.vertex);
				OUT.worldPos = worldPos;

				//float3 maskVert = float3(OUT.screenPos, 0);
				float2 maskVert = worldPos.xz / 21;
				maskVert.x /= 2;
				maskVert += 0.5;

                fixed4 maskCol = tex2Dlod(_Masks, float4(maskVert + float2(0, -_Split / 2), 0, 0));
				maskCol = fixed4(Unity_ColorspaceConversion_Linear_RGB_float(maskCol), maskCol.a);
				
				float2 pos = IN.vertex.xy;
				float time = _Time.y * _Speed;
				
                fixed4 maskCol1 = tex2Dlod(_Masks, float4(maskVert + float2(-_Split, 0), 0, 0));
				maskCol1 = fixed4(Unity_ColorspaceConversion_Linear_RGB_float(maskCol1), 1);
				fixed4 maskCol2 = tex2Dlod(_Masks, float4(maskVert + float2(_Split, 0), 0, 0));
				maskCol2 = fixed4(Unity_ColorspaceConversion_Linear_RGB_float(maskCol2), 1);

				// шум1
				float x = unity_gradientNoise((worldPos + time) * _Noise);
				// шум2
				float x2 = unity_gradientNoise((worldPos + time * 3) * _Noise * 3) * maskCol.r * 3;

				// снимает влияние ветра в месте с деформацией
				x *= 1 - maskCol.b;
				// снимает влияние ветра в месте с деформацией
				x = lerp(x, x * 2, maskCol.r);

				// стягивание
				x -= (maskCol1.r * _PushPower);
				x += (maskCol2.r * _PushPower);

				// расталкивание
				x += (maskCol1.b * _PushPower);
				x -= (maskCol2.b * _PushPower);
				
				// вращение торнадо
				x += x2;

				// влияние смещениия меньше у корня
				x *= IN.texcoord.y;

				// смешение к позиции
				x += pos.x;
				// blend
				x = lerp(pos.x, x, _Power);

				float y = pos.y;
				y -= maskCol.b * IN.texcoord.y;
				y += maskCol.r / 2 * IN.texcoord.y;

				float3 vertex = float3(x, y, IN.vertex.z);

				OUT.vertex = UnityObjectToClipPos(vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}



			fixed4 frag(v2f IN) : SV_Target
			{
				//float2 maskVert = IN.screenPos;
				float2 maskVert = IN.worldPos.xz / 21;
				maskVert.x /= 2;
				maskVert += 0.5;
				

                float4 maskCol = tex2D(_Masks, maskVert);
				 float4 maskCol2 = maskCol - 0.5;
				

				 
                fixed4 col = tex2D(_MainTex, IN.texcoord);
                //UNITY_APPLY_FOG(i.fogCoord, col);
				
				float a = col.a;
				a *= clamp(
					(1 - (maskCol.y * IN.texcoord.y * 4) 
					), 0, 1);

				//a = lerp(a, 1, mask.r) * col.a;

                return col * a;
			}
    	ENDCG
		}
	}
}
