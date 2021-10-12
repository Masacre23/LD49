Shader "Custom/CartoonWater" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_WaterText ("Water texture", 2D) = "white" {}
		_WaterText2 ("Water texture2", 2D) = "white" {}
		_SpeedX1 ("Texture x 1 speed", Float) = 1
		_SpeedY1 ("Texture y 1 speed", Float) = 1		
		_SpeedX2 ("Texture x 2 speed", Float) = 1
		_SpeedY2 ("Texture y 2 speed", Float) = 1
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_EmissionLM("Emission (Lightmapper)", 2D) = "white" {}
		[Toggle] _DynamicEmissionLM("Dynamic Emission (Lightmapper)", Int) = 0

	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _WaterText;
		sampler2D _WaterText2;

		struct Input {
			float2 uv_WaterText;
			float2 uv_WaterText2;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _SpeedX1;
		float _SpeedY1;
		
		float _SpeedX2;
		float _SpeedY2;
		

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
		    float sinTime = _SinTime.y;
		    fixed offsetX = _SpeedX1 * sinTime;
		    fixed offsetY = _SpeedY1 * sinTime;
		    
		    fixed2 offsetA = fixed2(offsetX, offsetY);
		    
		    fixed offsetX2 = _SpeedX2 * sinTime;
		    fixed offsetY2 = _SpeedY2 * sinTime;
		    
		    fixed2 offsetB = fixed2(offsetX2, offsetY2);
		    
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_WaterText, IN.uv_WaterText + offsetA) * _Color;
			fixed4 c2 = tex2D (_WaterText2, IN.uv_WaterText2 + offsetB) * _Color;
			
			c2.a = 0;
			
			o.Albedo = c.rgb + c2.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			//o.Alpha = c.a;
			
		}
		ENDCG
	}
	FallBack "Diffuse"
}
