Shader "Custom/IlluminColor" {
    Properties {
        _Illumi ("Illumi Color", Color) = (1,1,1,1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma surface surf Lambert
 
        fixed4 _Illumi;
       
        struct Input {
            float4 color : COLOR;
        };
       
        void surf (Input IN, inout SurfaceOutput o) {
            o.Emission = _Illumi.rgb;
           
        }
        ENDCG
    }
   
}