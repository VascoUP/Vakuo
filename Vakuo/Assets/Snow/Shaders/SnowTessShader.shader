Shader "Custom/SnowTessShader" {
	Properties{
		_Tess("Tessellation", Range(1,32)) = 4
		_Displacement("Displacement", Range(0,1)) = 0.5
		_SnowTex ("Snow (RGB)", 2D) = "white" {}
		_SnowColor ("Snow", Color) = (1,1,1,1)
		_GroundTex ("Ground (RGB)", 2D) = "black" {}
		_GroundColor ("Ground", Color) = (0,0,0,0)
		_MinLerpDisp ("Minimum displacement", Range(0,0.99)) = 0.7
		_MaxLerpDisp("Maximum displacement", Range(0,0.99)) = 0.7
		_DispTex("Disp Texture", 2D) = "gray" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:disp tessellate:tessDistance nolightmap
		#pragma target 4.6
		#include "Tessellation.cginc"

		struct appdata {
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		float _Tess;

		float4 tessDistance(appdata v0, appdata v1, appdata v2) {
			float minDist = 10.0;
			float maxDist = 25.0;
			return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
		}

		sampler2D _DispTex;
		float _Displacement;

		void disp(inout appdata v)
		{
			float d = tex2Dlod(_DispTex, float4(1-v.texcoord.x,v.texcoord.y,0,0)).r * _Displacement;
			v.vertex.xyz += v.normal * _Displacement - v.normal * d;
		}

		sampler2D _SnowTex, _GroundTex;

		struct Input {
			float2 uv_SnowTex;
			float2 uv_GroundTex;
		};

		half _Glossiness;
		half _Metallic;
		half _MinLerpDisp;
		half _MaxLerpDisp;
		fixed4 _SnowColor, _GroundColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		half remap(half In, float2 InBounds, float2 OutBounds) {
			half Out = OutBounds.x + (In - InBounds.x) * (OutBounds.y - OutBounds.x) / (InBounds.y - InBounds.x);
			return Out;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			half amount = tex2D(_DispTex, float2(1-IN.uv_SnowTex.x, IN.uv_SnowTex.y)).r;
			if (amount < _MinLerpDisp) {
				amount = 0;
			}
			else {
				amount = remap(amount, float2(_MinLerpDisp, 1), float2(0, _MaxLerpDisp));
			}

			half4 c = 
				lerp(tex2D(_SnowTex, float2(1-IN.uv_SnowTex.x, IN.uv_SnowTex.y)) * _SnowColor,
					tex2D(_GroundTex, float2(IN.uv_GroundTex.x, IN.uv_GroundTex.y)) * _GroundColor,
					amount);

			o.Albedo = c;
			o.Specular = _Metallic;
			o.Gloss = _Glossiness;
			o.Normal = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
