using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    void Update() {
        if (Camera.main != null)
            this.transform.position = Camera.main.transform.position;
    }
}
