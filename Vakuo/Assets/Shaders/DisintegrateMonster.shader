Shader "Custom/DisintegrateMonster" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture (RGB)", 2D) = "white" {}

		_EmissionGuide("Emission Guide (RGB)", 2D) = "black" {}
		_EmissionMinValue("Emission Minimun Value", Range(0.0, 1.0)) = 0.5
		_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_EmissionIntensity("Emission Intensity", Range(0.0, 10.0)) = 1.0

		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		[PerRendererData]
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0
		_EmissionSliceWidth("Emission Slice Width", Range(0.0, 1.0)) = 0.5
		_EmissionSliceColor("Emissive Slice Color", Color) = (1,1,1,1)
		_EmissionSliceIntensity("Emissive Slice Intensity", Range(0.0, 10.0)) = 1.0
	}
	SubShader{
			Tags{ "RenderType" = "Opaque" }
			Cull Off
			CGPROGRAM
			//if you're not planning on using shadows, remove "addshadow" for better performance
			#pragma surface surf Lambert addshadow

			struct Input {
				float2 uv_MainTex;
				float2 uv_EmissionGuide;
				float2 uv_SliceGuide;
				float _SliceAmount;
			};

			float4 _Color;
			sampler2D _MainTex;	

			sampler2D _EmissionGuide;
			float _EmissionMinValue;
			float4 _EmissionColor;
			float _EmissionIntensity;

			sampler2D _SliceGuide;
			float _SliceAmount;
			float _EmissionSliceWidth;
			float4 _EmissionSliceColor;
			float _EmissionSliceIntensity;

			float remap(float In, float2 InMinMax, float2 OutMinMax) {
				return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			}

			void surf(Input IN, inout SurfaceOutput o) {
				float cutoff = tex2D(_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount;
				float emissionCutOff = cutoff - _EmissionSliceWidth;
				if (_SliceAmount > _EmissionSliceWidth && emissionCutOff < _EmissionSliceWidth) {
					o.Emission = _EmissionSliceColor * _EmissionSliceIntensity;
				}
				else {
					float emi = tex2D(_EmissionGuide, IN.uv_EmissionGuide).rgb;
					if (emi > _EmissionMinValue) {
						o.Emission = remap(emi, float2(_EmissionMinValue,1), float2(0, 1)) * _EmissionColor * _EmissionIntensity;
					}
				}
				clip(cutoff);
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
			}
			ENDCG
			}
		Fallback "Diffuse"
}