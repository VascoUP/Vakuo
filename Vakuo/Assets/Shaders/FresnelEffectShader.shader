Shader "Custom/FresnelEffectShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_FresnelColor ("Fresnel (RGB)", Color) = (1,0,0,0)
		_FresnelPower ("Fresnel Power", Range(0.0,5.0)) = 1.0
		_FresnelSpeed ("Fresnel Speed", Range(0.0,10.0)) = 1.0
		_EmissionIntensity ("Emission Intensity", Range(0.0,5.0)) = 1.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		struct Input {
			float2 uv_MainTex;
         	float3 viewDir;
		};

		fixed4 _Color;
		sampler2D _MainTex;
		float4 _FresnelColor;
		half _FresnelPower;
		half _FresnelSpeed;
		half _EmissionIntensity;
		half _Glossiness;
		half _Metallic;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		half remap(half In, float2 InMinMax, float2 OutMinMax) {
			half Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
			return Out;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;

			float3 nViewDir = normalize(IN.viewDir);
			half fresnel = (1.0 - saturate(dot (nViewDir, o.Normal))) * remap(sin(_Time.y * _FresnelSpeed), float2(-1.0,1.0), float2(0.0,1.0)) * _FresnelPower;
			o.Emission = _FresnelColor.rgba * fresnel * _EmissionIntensity;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
