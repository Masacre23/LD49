using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressButtonUI : MonoBehaviour
{
    [SerializeField] RectTransform fxHolder;
    [SerializeField] Image circleImg;

    [Range(0, 1)] public float progress = 0f;

    private void Start() {
        this.gameObject.SetActive(false);
        transform.rotation = Camera.main.transform.rotation;
    }

    void FixedUpdate() {
        circleImg.fillAmount = progress;
        fxHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, -progress * 360));
    }

    public void Show() {
        this.gameObject.SetActive(true);
        progress = 0;
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }
}
