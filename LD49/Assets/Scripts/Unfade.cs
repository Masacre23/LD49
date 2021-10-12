using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unfade : MonoBehaviour
{

    private float fadeTime = 2f;
    Image fadePanel;
    IEnumerator UnFade() {
        for (float t = fadeTime; t > 0.0f;) {
            t -= Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }
    }
    void Start()
    {
        fadePanel = gameObject.GetComponent<Image>();
        StartCoroutine(UnFade());
    }
}
