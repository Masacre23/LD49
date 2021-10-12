// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

//phong CG shader with panning uvs


Shader "CG Shaders/Phong/PanningShader"
{
	Properties
	{
		_diffuseColor("Diffuse Color", Color) = (1,1,1,1)
		_diffuseMap("Diffuse", 2D) = "white" {}
	_FrenselPower("Rim Power", Range(1.0, 10.0)) = 2.5
		_FrenselPower(" ", Float) = 2.5
		_rimColor("Rim Color", Color) = (1,1,1,1)
		_specularPower("Specular Power", Range(1.0, 50.0)) = 10
		_specularPower(" ", Float) = 10
		_specularColor("Specular Color", Color) = (1,1,1,1)
		_normalMap("Normal / Specular (A)", 2D) = "bump" {}
	//alpha attributes
	_alphaFrenselPower("Alpha Power", Range(1.0, 10.0)) = 1.0
		_alphaFrenselPower(" ", Float) = 1.0
		_alphaFrenselBias("Alpha Bias", Range(0.0, 1.0)) = 0.5
		_alphaFrenselBias(" ", Float) = 0.5
		//Unity only allows floats or float4s so we split the pan speed into 2
		//uv offset for panning - X
		_uvPanSpeedX("UV Pan Speed X", Range(0.0, 1.0)) = 0.1
		_uvPanSpeedX(" ", Float) = 0.1
		//uv offset for panning - Y
		_uvPanSpeedY("UV Pan Speed Y", Range(0.0, 1.0)) = 0.5
		_uvPanSpeedY(" ", Float) = 0.5
		//scaling for the second sample
		_doubleSampleScale("Double Sample Scale", Range(1.0, 2.0)) = 1.5
		_doubleSampleScale(" ", Float) = 1.5
		_doubleSampleOffset("Double Sample Speed Scale", Range(0.5, 1.5)) = 0.9
		_doubleSampleOffset(" ", Float) = 0.9

	}
		SubShader
	{
		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }

		//Alpha Blending - OneMinusSrcAlpha is the same as InvSrcAlpha in Maya
		Blend SrcAlpha OneMinusSrcAlpha

		//Additive Alpha Blending 
		//Blend SrcAlpha One 
		CGPROGRAM

#pragma vertex vShader
#pragma fragment pShader
#include "UnityCG.cginc"
#pragma multi_compile_fwdbase
#pragma target 3.0

		uniform fixed3 _diffuseColor;
	uniform sampler2D _diffuseMap;
	uniform half4 _diffuseMap_ST;
	uniform fixed4 _LightColor0;
	uniform half _FrenselPower;
	uniform fixed4 _rimColor;
	uniform half _specularPower;
	uniform fixed3 _specularColor;
	uniform sampler2D _normalMap;
	uniform half4 _normalMap_ST;
	uniform fixed _uvPanSpeedX;
	uniform fixed _uvPanSpeedY;
	uniform fixed _doubleSampleScale;
	uniform fixed _doubleSampleOffset;
	uniform half _alphaFrenselPower;
	uniform fixed _alphaFrenselBias;


	struct app2vert {
		float4 vertex 	: 	POSITION;
		fixed2 texCoord : TEXCOORD0;
		fixed4 normal : NORMAL;
		fixed4 tangent : TANGENT;

	};
	struct vert2Pixel
	{
		float4 pos 						: 	SV_POSITION;
		fixed2 uvs : TEXCOORD0;
		fixed3 normalDir : TEXCOORD1;
		fixed3 binormalDir : TEXCOORD2;
		fixed3 tangentDir : TEXCOORD3;
		half3 posWorld						:	TEXCOORD4;
		fixed3 viewDir : TEXCOORD5;
		fixed3 lighting : TEXCOORD6;
	};

	fixed lambert(fixed3 N, fixed3 L)
	{
		return saturate(dot(N, L));
	}
	fixed frensel(fixed3 V, fixed3 N, half P)
	{
		return pow(1 - saturate(dot(V,N)), P);
	}
	fixed phong(fixed3 R, fixed3 L)
	{
		return pow(saturate(dot(R, L)), _specularPower);
	}
	vert2Pixel vShader(app2vert IN)
	{
		vert2Pixel OUT;
		float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
		float4x4 WorldInverseTranspose = unity_WorldToObject;
		float4x4 World = unity_ObjectToWorld;

		OUT.pos = mul(WorldViewProjection, IN.vertex);
		OUT.uvs = IN.texCoord;

		OUT.normalDir = normalize(mul(IN.normal, WorldInverseTranspose).xyz);
		OUT.tangentDir = normalize(mul(IN.tangent, WorldInverseTranspose).xyz);
		OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir));
		OUT.posWorld = mul(World, IN.vertex).xyz;
		OUT.viewDir = normalize(_WorldSpaceCameraPos - OUT.posWorld);

		//vertex lights
		fixed3 vertexLighting = fixed3(0.0, 0.0, 0.0);
#ifdef VERTEXLIGHT_ON
		for (int index = 0; index < 4; index++)
		{
			half3 vertexToLightSource = half3(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index]) - OUT.posWorld;
			fixed attenuation = (1.0 / length(vertexToLightSource)) *.5;
			fixed3 diffuse = unity_LightColor[index].xyz * lambert(OUT.normalDir, normalize(vertexToLightSource)) * attenuation;
			vertexLighting = vertexLighting + diffuse;
		}
		vertexLighting = saturate(vertexLighting);
#endif
		OUT.lighting = vertexLighting;

		return OUT;
	}

	fixed4 pShader(vert2Pixel IN) : COLOR
	{
		//calculate the offset
		//using sin of time on one channel to give some "wavy" motion and break up the constant movement on the other channel
		//_Time  comes in from "UnityCG.cginc" as a float4. just use time.y - thats "real" time
		half time = _Time.y;
	half2 timeOffset = half2(sin(time)*_uvPanSpeedX,time*_uvPanSpeedY);
	//default uvs
	half2 normalUVs = TRANSFORM_TEX(IN.uvs, _normalMap);
	//default uvs + offset
	half2 normalUVs1 = normalUVs + timeOffset;
	//default uvs + offset scaled by parameters for 2nd sample
	half2 normalUVs2 = normalUVs * _doubleSampleScale + (timeOffset *_doubleSampleOffset);
	//sample normal map and unpack normals
	fixed4 normalD = tex2D(_normalMap, normalUVs1);
	normalD.xyz = (normalD.xyz * 2) - 1;
	//re sample normal map and unpack re sampled normals
	fixed2 norma2D = tex2D(_normalMap, normalUVs2).xy;
	norma2D = (norma2D * 2) - 1;
	//add to xy like with detail normals
	//you could potentially tweak this like we did with detail normals but i rarely see a large benefit
	normalD.xy += norma2D;

	//half3 normalDir = half3(2.0 * normalSample.xy - float2(1.0), 0.0);
	//deriving the z component
	//normalDir.z = sqrt(1.0 - dot(normalDir, normalDir));
	// alternatively you can approximate deriving the z component without sqrt like so:  
	//normalDir.z = 1.0 - 0.5 * dot(normalDir, normalDir);

	//pull the alpha out for spec before modification
	fixed3 normalDir = normalD.xyz;
	fixed specMap = normalD.w;
	normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));

	fixed3 ambientL = UNITY_LIGHTMODEL_AMBIENT.xyz;

	half3 pixelToLightSource = _WorldSpaceLightPos0.xyz - (IN.posWorld*_WorldSpaceLightPos0.w);
	fixed attenuation = lerp(1.0, 1.0 / length(pixelToLightSource), _WorldSpaceLightPos0.w);
	fixed3 lightDirection = normalize(pixelToLightSource);
	fixed diffuseL = lambert(normalDir, lightDirection);

	fixed3 rimLight = frensel(normalDir, IN.viewDir, _FrenselPower);
	rimLight *= saturate(dot(fixed3(0,1,0),normalDir)* 0.5 + 0.5)* saturate(dot(fixed3(0,1,0),-IN.viewDir) + 1.75);
	fixed3 diffuse = _LightColor0.xyz * (diffuseL + (rimLight * diffuseL))* attenuation;
	rimLight *= (1 - diffuseL)*(_rimColor.xyz *_rimColor.w);
	diffuse = saturate(IN.lighting + ambientL + diffuse + rimLight);

	fixed specularHighlight = phong(-reflect(IN.viewDir , normalDir) ,lightDirection)*attenuation;

	fixed4 outColor;
	half2 diffuseUVs = TRANSFORM_TEX(IN.uvs, _diffuseMap);
	//only need 1 sample of the diffuse
	diffuseUVs = diffuseUVs + timeOffset;
	fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
	fixed3 diffuseS = (diffuse * texSample.xyz) * _diffuseColor.xyz;
	fixed3 specular = (specularHighlight * _specularColor * specMap);
	//create an alpha based on the frensel term + a bias to keep some effect
	//basically this just makes it eaisier to see through when looking straight at it vs glancing angles
	//kinda like water...ish
	fixed rimAlpha = saturate(frensel(normalDir, IN.viewDir, _alphaFrenselPower) + _alphaFrenselBias);

	outColor = fixed4(diffuseS + specular,rimAlpha);
	return outColor;
	}

		ENDCG
	}

		//the second pass for additional lights
		Pass
	{
		Tags{ "LightMode" = "ForwardAdd" }
		Blend One One



		CGPROGRAM
#pragma vertex vShader
#pragma fragment pShader
#include "UnityCG.cginc"
#pragma target 3.0

		uniform fixed3 _diffuseColor;
	uniform sampler2D _diffuseMap;
	uniform half4 _diffuseMap_ST;
	uniform fixed4 _LightColor0;
	uniform half _specularPower;
	uniform fixed3 _specularColor;
	uniform sampler2D _normalMap;
	uniform half4 _normalMap_ST;
	uniform fixed _uvPanSpeedX;
	uniform fixed _uvPanSpeedY;
	uniform fixed _doubleSampleScale;
	uniform fixed _doubleSampleOffset;
	uniform half _alphaFrenselPower;
	uniform fixed _alphaFrenselBias;



	struct app2vert {
		float4 vertex 	: 	POSITION;
		fixed2 texCoord : TEXCOORD0;
		fixed4 normal : NORMAL;
		fixed4 tangent : TANGENT;
	};
	struct vert2Pixel
	{
		float4 pos 						: 	SV_POSITION;
		fixed2 uvs : TEXCOORD0;
		fixed3 normalDir : TEXCOORD1;
		fixed3 binormalDir : TEXCOORD2;
		fixed3 tangentDir : TEXCOORD3;
		half3 posWorld						:	TEXCOORD4;
		fixed3 viewDir : TEXCOORD5;
		fixed4 lighting : TEXCOORD6;
	};

	fixed lambert(fixed3 N, fixed3 L)
	{
		return saturate(dot(N, L));
	}
	fixed frensel(fixed3 V, fixed3 N, half P)
	{
		return pow(1 - saturate(dot(V,N)), P);
	}
	fixed phong(fixed3 R, fixed3 L)
	{
		return pow(saturate(dot(R, L)), _specularPower);
	}
	vert2Pixel vShader(app2vert IN)
	{
		vert2Pixel OUT;
		float4x4 WorldViewProjection = UNITY_MATRIX_MVP;
		float4x4 WorldInverseTranspose = unity_WorldToObject;
		float4x4 World = unity_ObjectToWorld;

		OUT.pos = mul(WorldViewProjection, IN.vertex);
		OUT.uvs = IN.texCoord;

		OUT.normalDir = normalize(mul(IN.normal, WorldInverseTranspose).xyz);
		OUT.tangentDir = normalize(mul(IN.tangent, WorldInverseTranspose).xyz);
		OUT.binormalDir = normalize(cross(OUT.normalDir, OUT.tangentDir));
		OUT.posWorld = mul(World, IN.vertex).xyz;
		OUT.viewDir = normalize(_WorldSpaceCameraPos - OUT.posWorld);
		return OUT;
	}
	fixed4 pShader(vert2Pixel IN) : COLOR
	{
		//calculate the offset
		//using sin of time on one channel to give some "wavy" motion and break up the constant movement on the other channel
		//_Time  comes in from "UnityCG.cginc" as a float4. just use time.y - thats "real" time
		half time = _Time.y;
	half2 timeOffset = half2(sin(time)*_uvPanSpeedX,time*_uvPanSpeedY);
	//default uvs
	half2 normalUVs = TRANSFORM_TEX(IN.uvs, _normalMap);
	//default uvs + offset
	half2 normalUVs1 = normalUVs + timeOffset;
	//default uvs + offset scaled by parameters for 2nd sample
	half2 normalUVs2 = normalUVs * _doubleSampleScale + (timeOffset *_doubleSampleOffset);
	//sample normal map and unpack normals
	fixed4 normalD = tex2D(_normalMap, normalUVs1);
	normalD.xyz = (normalD.xyz * 2) - 1;
	//re sample normal map and unpack re sampled normals
	fixed2 normalD2 = tex2D(_normalMap, normalUVs2).xy;
	normalD2 = (normalD2 * 2) - 1;
	//add to xy like with detail normals
	//you could potentially tweak this like we did with detail normals but i rarely see a large benefit
	normalD.xy += normalD2;

	//half3 normalDir = half3(2.0 * normalSample.xy - float2(1.0), 0.0);
	//deriving the z component
	//normalDir.z = sqrt(1.0 - dot(normalDir, normalDir));
	// alternatively you can approximate deriving the z component without sqrt like so:  
	//normalDir.z = 1.0 - 0.5 * dot(normalDir, normalDir);

	//pull the alpha out for spec before modification
	fixed3 normalDir = normalD.xyz;
	fixed specMap = normalD.w;
	normalDir = normalize((normalDir.x * IN.tangentDir) + (normalDir.y * IN.binormalDir) + (normalDir.z * IN.normalDir));

	//Fill lights
	half3 pixelToLightSource = _WorldSpaceLightPos0.xyz - (IN.posWorld*_WorldSpaceLightPos0.w);
	fixed attenuation = lerp(1.0, 1.0 / length(pixelToLightSource), _WorldSpaceLightPos0.w);
	fixed3 lightDirection = normalize(pixelToLightSource);

	half diffuseL = lambert(normalDir, lightDirection);
	fixed3 diffuseTotal = _LightColor0.xyz * diffuseL * attenuation;

	//specular highlight
	half specularHighlight = phong(-reflect(IN.viewDir , normalDir) ,lightDirection)*attenuation;

	fixed4 outColor;
	half2 diffuseUVs = TRANSFORM_TEX(IN.uvs, _diffuseMap);
	//only need 1 sample of the diffuse
	diffuseUVs = diffuseUVs + timeOffset;
	fixed4 texSample = tex2D(_diffuseMap, diffuseUVs);
	fixed3 diffuse = (diffuseTotal * texSample.xyz) * _diffuseColor.xyz;
	fixed3 specular = specularHighlight * _specularColor * specMap;
	//create an alpha based on the frensel term + a bias to keep some effect
	//basically this just makes it eaisier to see through when looking straight at it vs glancing angles
	//kinda like water...ish
	fixed rimAlpha = saturate(frensel(normalDir, IN.viewDir, _alphaFrenselPower) + _alphaFrenselBias);
	//since we have to add on this pass, we multiply the alpha by the diffuse and spec terms
	outColor = fixed4((diffuse + specular)*rimAlpha,1.0);
	return outColor;
	}

		ENDCG
	}

	}
}