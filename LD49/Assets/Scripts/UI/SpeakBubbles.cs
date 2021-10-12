using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakBubbles : MonoBehaviour
{
    public Image firstImg;
    public Image secondImg;
    public float time;
    public float offsetLeft;
    public float offsetRight;

    float counter = 0.0f;
    Vector3 initiaLocalPos;
    [SerializeField] bool disableRotation = false;

    void Start() {
        secondImg.enabled = false;
        initiaLocalPos = gameObject.transform.localPosition;
    }
    void Update() {

        if (!disableRotation) {
            if (transform.position.x < Camera.main.transform.position.x)
                transform.position = Vector3.Lerp(transform.position, initiaLocalPos + transform.parent.position + Camera.main.transform.right * offsetRight, Time.deltaTime);
            else {
                transform.position = Vector3.Lerp(transform.position, initiaLocalPos + transform.parent.position + Camera.main.transform.right * offsetLeft, Time.deltaTime);
            }
            transform.rotation = Camera.main.transform.rotation;
        }


        counter += Time.deltaTime;
        if(counter > time) {
            counter = 0f;
            if (firstImg.enabled) {
                secondImg.enabled = true;
                firstImg.enabled = false;
            } else {
                firstImg.enabled = true;
                secondImg.enabled = false;
            }
        }
    }
}
