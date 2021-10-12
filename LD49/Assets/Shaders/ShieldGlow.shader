Shader "Custom/ShieldGlow" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
        _EmissionForce ("Emission", Range(0,1)) = 0
	}
	SubShader {
		Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
		LOD 200
        ZWrite off
        Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

        fixed _EmissionForce;
		fixed4 _Color;
		fixed4 _EmissionColor;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Alpha = c.a + 0.25;
			o.Emission = _EmissionForce * _EmissionColor;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
