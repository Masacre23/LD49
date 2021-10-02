using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class Link : MonoBehaviour {
	public string url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

	public void OpenLinkJSPlugin() {
#if !UNITY_EDITOR
		if (Application.platform == RuntimePlatform.WebGLPlayer) {
				openWindow(url);
		} else {
			Application.OpenURL(url);
		}
#endif
	}

	[DllImport("__Internal")]
	private static extern void openWindow(string url);

}