//A custom cloud shader I made to simulate cloud shadows being rendered on screen above all in-game assets
//Implements a custom gpu noise function in order to create virtually infinite combinations of noise distribution (vs. using a pre-baked perlin noise texture as the base)
//Supports runtime adjustment of all the properties and can be changed to suit many different needs/weather conditions

Shader "Custom/CloudShader"
{
    Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_CurPosition("Camera Current Pos", Vector) = (0, 0, 0, 0)
		_CloudDirection("Cloud Movement in X and Y", Vector) = (0, 0, 0, 0)
		_StartingOffset("Random Offset in X and Y", Vector) = (0, 0, 0, 0)
		_Cutoff("Min Alpha Required to render", Range(0.0, 1.0)) = 0.75
		_CloudDarkness("How Dark Cloud Shadows Are", Range(0.0, 1.0)) = 0.35
		_ExtraSeconds("Seconds Weather Has Elapsed", Int) = 0
		_octaves("Octaves", Int) = 7
		_lacunarity("Lacunarity", Range(1.0, 5.0)) = 2
    _gain("Gain", Range(0.0, 1.0)) = 0.5
    _value("Value", Range(-2.0, 2.0)) = 0.0
    _amplitude("Amplitude", Range(0.0, 5.0)) = 1.5
    _frequency("Frequency", Range(0.0, 6.0)) = 2.0
    _power("Power", Range(0.1, 5.0)) = 1.0
    _scale("Scale", Float) = 1.0
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
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			float _Cutoff, _CloudDarkness, _lacunarity, _gain, _value, _amplitude, _frequency, _power, _scale;
			int _ExtraSeconds, _octaves;
			float4 _StartingOffset;
			float4 _CurPosition;
			float4 _CloudDirection;
			uniform float4 _MainTex_TexelSize;
			fixed4x4 _TilesInOutMatrix[200];
			fixed _LeftRightSign;
			float _LeftRightDisplacement;
			fixed _UpDownSign;
			float _UpDownDisplacement;

			float noise(float2 p, float2 adjustments)
			{
				        p = p * _scale + adjustments;
                for( int i = 0; i < _octaves; i++ )
                {
                    float2 i = floor(p * _frequency);
                    float2 f = frac(p * _frequency);      
                    float2 t = f * f * f * (f * (f * 6.0 - 15.0) + 10.0);
                    float2 a = i + float2(0.0, 0.0);
                    float2 b = i + float2(1.0, 0.0);
                    float2 c = i + float2(0.0, 1.0);
                    float2 d = i + float2(1.0, 1.0);
                    a = -1.0 + 2.0 * frac(sin(float2(dot(a, float2(127.1, 311.7)), dot(a, float2(269.5,183.3)))) * 43758.5453123);
                    b = -1.0 + 2.0 * frac(sin(float2(dot(b, float2(127.1, 311.7)), dot(b, float2(269.5,183.3)))) * 43758.5453123);
                    c = -1.0 + 2.0 * frac(sin(float2(dot(c, float2(127.1, 311.7)), dot(c, float2(269.5,183.3)))) * 43758.5453123);
                    d = -1.0 + 2.0 * frac(sin(float2(dot(d, float2(127.1, 311.7)), dot(d, float2(269.5,183.3)))) * 43758.5453123);
                    float A = dot(a, f - float2(0.0, 0.0));
                    float B = dot(b, f - float2(1.0, 0.0));
                    float C = dot(c, f - float2(0.0, 1.0));
                    float D = dot(d, f - float2(1.0, 1.0));
                    float noise = (lerp(lerp(A, B, t.x), lerp(C, D, t.x), t.y));              
                    _value += _amplitude * noise;
                    _frequency *= _lacunarity;
                    _amplitude *= _gain;
                }
                _value = clamp(_value, -1.0, 1.0);
                return pow(_value * 0.5 + 0.5, _power);
			}

			float4 frag(v2f i) : SV_Target
			{
				//this used to just be noise(i.uv.xy, etc.);
				float noiseVal = noise(float2(floor(i.uv.x / (_MainTex_TexelSize.x * 0.5)) / (_MainTex_TexelSize.z * 2), floor(i.uv.y / (_MainTex_TexelSize.y * 0.5)) / (_MainTex_TexelSize.w * 2)), float2(_StartingOffset.x + (_CurPosition.x * _scale) + (_CloudDirection.x * -1 * (_Time.y + _ExtraSeconds)), _StartingOffset.y + (_CurPosition.y * _scale) + (_CloudDirection.y * -1 * (_Time.y + _ExtraSeconds))));
				//noiseVal = noise(i.uv.xy, float2(_StartingOffset.z + (_CurPosition.x * _scale) + (_CloudDirection.z * -1 * _Time.y), _StartingOffset.w + (_CurPosition.y * _scale) + (_CloudDirection.w * -1 * _Time.y)));
				//noiseVal = pow(clamp(noiseVal, 0.0, 1.0), 0.5); do this and make the ternary be noiseVal >= 0.01 and you get a cool sort of fence looking effect
				float tileIndex = clamp(floor((1 - i.uv.y) / 0.1) * 20, 0, 180) + clamp((i.uv.x / 0.05), 0, 19);
				float rowHere = 1 - ((((i.uv.y % 0.1) / 0.1) * _UpDownSign * -1 <= ((1 - _UpDownDisplacement) % 1.0) * _UpDownSign * -1) ? _UpDownSign : 0);
				float columnHere = 1 + ((((i.uv.x % 0.05) / 0.05) * _LeftRightSign * -1 <= ((1 - _LeftRightDisplacement) % 1.0) * _LeftRightSign * -1) ? _LeftRightSign : 0);
				
				float alphaVal = (noiseVal >= _Cutoff) ? _CloudDarkness * _TilesInOutMatrix[tileIndex][rowHere][columnHere] : 0;
				float4 colour = float4(0, 0, 0, alphaVal * i.color.a);
				return colour;
			}
			ENDCG
		}
	}
}
