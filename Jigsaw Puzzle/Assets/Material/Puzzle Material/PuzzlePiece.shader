Shader "Custom/Puzzle Piece" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Mask("Mask (A)", 2D) = "white" {}
		_Mask2("Mask (A)", 2D) = "white" {}
		_Mask3("Mask (A)", 2D) = "white" {}
	}

	SubShader{
		Tags{ "RenderType" = "Transparent" "IgnoreProjector" = "True" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert alpha
#pragma target 3.0
		sampler2D _MainTex;
		sampler2D _Mask;
		sampler2D _Mask2;
		sampler2D _Mask3;

		struct Input {
			float2 uv_MainTex;
			float2 uv2_Mask;
			float4 color : COLOR;
		};

		float GetPieceAlpha(float pieceIdentifier, float pieceAlpha, float2 direction)
		{
			switch (pieceIdentifier)
			{
				case 0: pieceAlpha *= tex2D(_Mask, direction);
					break;
				case 1: pieceAlpha *= tex2D(_Mask2, direction);
					break;
				case 2: pieceAlpha *= tex2D(_Mask3, direction);
					break;
			}
			return pieceAlpha;
		}

		fixed4 _Color;
		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;

			float2x2 rotationMatrix = float2x2(0, -1, 1, 0);	
			float2 uv_Top = IN.uv2_Mask;
			float2 uv_Left = mul(uv_Top.xy, rotationMatrix);
			float2 uv_Bot = mul(uv_Left.xy, rotationMatrix);
			float2 uv_Right = mul(uv_Bot.xy, rotationMatrix);

			o.Alpha = GetPieceAlpha(IN.color.r, o.Alpha, uv_Left);
			o.Alpha = GetPieceAlpha(IN.color.g, o.Alpha, uv_Right);
			o.Alpha = GetPieceAlpha(IN.color.b, o.Alpha, uv_Top);
			o.Alpha = GetPieceAlpha(IN.color.a, o.Alpha, uv_Bot);
		}
		ENDCG
	}
	FallBack "Diffuse"
}