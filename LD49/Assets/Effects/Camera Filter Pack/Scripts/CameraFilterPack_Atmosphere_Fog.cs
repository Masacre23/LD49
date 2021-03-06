////////////////////////////////////////////////////////////////////////////////////
// CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Atmosphere/Fog")]
public class CameraFilterPack_Atmosphere_Fog: MonoBehaviour {
	#region Variables
	public Shader SCShader;
	private float TimeX = 1.0f;
	private Vector4 ScreenResolution;
	private Material SCMaterial;
	[Range(0, 1000)]
	public float Near = 0.3f;
	[Range(0, 1000)]
	public float Far = 100f;
	[Range(0, 20)]
	public float Amount = 4f;
	[Range(2,16)]
	public int FastFilter = 4;
	[Range(0,1f)]
	public float Threshold = 0.5f;
	[Range(0,3f)]
	public float Intensity = 2.25f;
	[Range(-1,1f)]
	public float Precision = 0.56f;
	public Color GlowColor = new Color(0,0.7f,1,1);

	public static float ChangeAmount;
	public static int ChangeFastFilter;

	#endregion
	
	#region Properties
	Material material
	{
		get
		{
			if(SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;	
			}
			return SCMaterial;
		}
	}
	#endregion
	void Start () 
	{
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		ChangeAmount  	 = Amount;
		ChangeFastFilter = FastFilter;
		SCShader = Shader.Find("CameraFilterPack/Atmosphere_Fog");

		if(!SystemInfo.supportsImageEffects)
		{
			enabled = false;
			return;
		}
	}
	
	void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if(SCShader != null)
		{
			int DownScale=FastFilter;
			TimeX+=Time.deltaTime;
			if (TimeX>100)  TimeX=0;
			material.SetFloat("_Near",Near);
			material.SetFloat("_Far",Far);
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_Amount", Amount);
			material.SetFloat("_Value1", Threshold);
			material.SetFloat("_Value2", Intensity);
			material.SetFloat("_Value3", Precision);
			material.SetColor ("_GlowColor",GlowColor);
			material.SetVector("_ScreenResolution",new Vector2(Screen.width/DownScale,Screen.height/DownScale));
			
			if (FastFilter>1)
			{


			//	RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
			//	buffer.depth = 24;
			//	RenderBuffer
		//		RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
		//		buffer.filterMode=FilterMode.Trilinear;
			//	Graphics.Blit(sourceTexture, buffer, material,3);
		//		Graphics.Blit(buffer, buffer2, material,2);
		//		Graphics.Blit(buffer2, buffer, material,0);
		//		material.SetFloat("_Amount", Amount*2);
		//		Graphics.Blit(buffer, buffer2, material,2);
		//		Graphics.Blit(buffer2, buffer, material,0);

				//material.SetTexture("_MainTex3", buffer);
		//		material.SetTexture("_MainTex2", buffer);
		//		RenderTexture.ReleaseTemporary(buffer);
		//		RenderTexture.ReleaseTemporary(buffer2);
				Graphics.Blit(sourceTexture, destTexture, material,0);
			}
			else
			{
				Graphics.Blit(sourceTexture, destTexture, material,0);
			}
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);	
		}
		
		
	}
void OnValidate()
{
		ChangeAmount=Amount;
		ChangeFastFilter=FastFilter;
		
}
	// Update is called once per frame
	void Update () 
	{
		if (Application.isPlaying)
		{
			Amount = ChangeAmount;
			FastFilter = ChangeFastFilter;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying!=true)
		{
			SCShader = Shader.Find("CameraFilterPack/Atmosphere_Fog");

		}
		#endif

	}
	
	void OnDisable ()
	{
		if(SCMaterial)
		{
			DestroyImmediate(SCMaterial);	
		}
		
	}
	
	
}