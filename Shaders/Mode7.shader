//this is a multi-purpose shader created in an effort to replicate the hardware scrolling and other similar features present on old school nintentdo hardware
//supports runtime adjustment of effects (interweave, etc.) and their strength (how intense the waving is, etc.)

Shader "Custom/Mode7"
{
		Properties
		{
			_MainTex("Texture", 2D) = "white" {}

			_Amp("Amp (Amount Of Waving)", Float) = 0.02
			_Freq("Freq", Float) = 0.1
			_Speed("Speed (Time Mod.)", Float) = 2.75
			_Inter("Interweaved (0 On, 2 Off)", Int) = 2
			_Comp("Compression (Vert)", Float) = 1
			_Vert("Vert Comp (0 Off, 1 On)", Int) = 0
			_Horiz("Horiz Bool (0 Off, 1 On)", Int) = 0

			_NewFloat("Floatey", Float) = 1

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

		sampler2D _MainTex;
		float4 _MainTex_TexelSize;

		float _Amp;
		float _Freq;
		float _Speed;
		int _Inter;
		float _Comp;
		int _Vert;
		int _Horiz;

		float _NewFloat;

		float4 frag(v2f i) : SV_Target
		{
			float x = i.uv.x;
			float y = ((i.uv.y - 1) * -1) + (-1 * _NewFloat);

			x += (((_Amp * sin(_Freq * y * _MainTex_TexelSize.w + _Speed * _Time.y)) * sign(((floor(((abs(y) % 1) * _MainTex_TexelSize.w) + 0.5)) % 2) - 0.5 + _Inter)) * _Horiz);
			y = y * _Comp + ((_Amp * sin(_Freq * y * _MainTex_TexelSize.w + _Speed * _Time.y)) * _Vert);

			float2 pixel = float2((x + 10.0) % 1, ((y - 30000.0) * -1.0) % 1);

			float4 colour = tex2D(_MainTex, pixel) * i.color;
			colour.rgb *= colour.a;
			return colour;
		}
			ENDCG
		}
	}
}
