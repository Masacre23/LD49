// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

Shader "CameraFilterPack/Blizzard" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTex2 ("Base (RGB)", 2D) = "white" {}
		}
	SubShader 
	{
		Pass
		{
			ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			
			uniform sampler2D _MainTex;
			uniform sampler2D _MainTex2;
			uniform float _TimeX;
			uniform float _Speed;
			uniform float _Value;
				uniform float4 _ScreenResolution;
			uniform float2 _MainTex_TexelSize;
			
		       struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                  half2 texcoord  : TEXCOORD0;
                  float4 vertex   : SV_POSITION;
                  fixed4 color    : COLOR;
           };   
             
  			v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                
                return OUT;
            }
            
fixed4 frag (v2f i) : COLOR
{
	float2 uvx = i.texcoord.xy;
	float2 uv=uvx;
	#if UNITY_UV_STARTS_AT_TOP
	if (_MainTex_TexelSize.y < 0)
	uvx.y = 1-uvx.y;
	#endif
	float3 col = tex2D(_MainTex,uv).rgb;
	// 1
	float timx=_TimeX*_Value;
	float tx=timx;
	float t = 1+(tx * sin(tx)/16);
  	uv.x+=t; uv.x-=tx+sin(uv.x+tx/16)/16; uv.y+=tx;
  	uv.y=uv.y+(uv.x* t/16)/2;
  	float4 col2=float4(0,0,0,0);
  	col2.r = tex2D(_MainTex2,uv).r;
 	
 	// 2
	uv=uvx;
	tx=timx/2;
	t = 1+(tx * sin(tx)/4);
  	uv.x+=t; uv.x-=tx+sin(uv.x+tx/8)/8; uv.y+=tx;
  	col2.g = tex2D(_MainTex2,uv).g;
 	
 	// 3
	uv=uvx*2;
	tx=timx;
	t = 1+(tx * sin(tx)/2);
  	uv.x+=t; uv.x-=tx+sin(uv.x+tx/12)/8; uv.y+=tx;
 	uv.y=uv.y+(uv.x* t)/64;
   	col2.b = tex2D(_MainTex2,uv).b;

  	// 4
	uv=uvx/2;
	tx=timx/3;
	t = 1+(tx * sin(tx)/3);
  	uv.x+=t; uv.x-=tx+sin(uv.x+tx/6)/12; uv.y+=tx;
  	col2.a = tex2D(_MainTex2,uv).g*2;


   	col2.r=max(col2.r*sin(t/10),0);
 	//col2.g=max(col2.g*sin(1+t/15),0);
 	col2.b=max(col2.b*sin(2+t/64),0);
 	col.rgb+=(col2.r+col2.g+col2.b+col2.a)/4;
 	
 	return fixed4(col, 1.0);
}
			
			ENDCG
		}
		
	}
}