using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractuableScript : MonoBehaviour
{
    public GameObject item = null;
    public bool isActive = true;
    public float time = 3f;
    public PressButtonUI buttonUI;

    public virtual void ProcessSuccess()
    {
        Debug.Log("PROCESS SUCCESS!!");
        //isActive = false;
    }
    
}
