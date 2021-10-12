using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleAnimation : MonoBehaviour
{
    [SerializeField] RectTransform fxHolder;
    [SerializeField] Image circleImg;

    [SerializeField] [Range(0, 1)] public float progress = 0f;

    void Update()
    {
        circleImg.fillAmount = progress;
        fxHolder.rotation = Quaternion.Euler(new Vector3(0f, 0f, -progress * 360));
    }
}
