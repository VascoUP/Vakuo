﻿Shader "Custom/Disintegrate" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture (RGB)", 2D) = "white" {}
		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		[PerRendererData] 
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0
		_EmissionSliceWidth("Emission Slice Width", Range(0.0, 1.0)) = 0.5
		_EmissionColor("Emissive Color", Color) = (1,1,1,1)
		_EmissionIntensity("Emissive Intensity", Range(0.0, 10.0)) = 1.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		//if you're not planning on using shadows, remove "addshadow" for better performance
		#pragma surface surf Lambert addshadow
		struct Input {
			float3 viewDir;
			float2 uv_MainTex;
			float2 uv_SliceGuide;
			float _SliceAmount;
		};

		float4 _Color;
		sampler2D _MainTex;
		sampler2D _SliceGuide;
		float _SliceAmount;
		float _EmissionSliceWidth;
		float4 _EmissionColor;
		float _EmissionIntensity;

		void surf(Input IN, inout SurfaceOutput o) {
			float cutoff = tex2D(_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount/* - _EmissionSliceWidth*/;
			if (_SliceAmount > _EmissionSliceWidth) {
				float emissionCutOff = cutoff - _EmissionSliceWidth;
				if (emissionCutOff < _EmissionSliceWidth)
					o.Emission = _EmissionColor * _EmissionIntensity;
			}
			clip(cutoff);
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb * _Color;
		}
		ENDCG
		}
	Fallback "Diffuse"
}