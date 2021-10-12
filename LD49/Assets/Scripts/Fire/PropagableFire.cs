using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagableFire : MonoBehaviour
{
    public GameObject fireObject = null;
    private float fireTime = 10f;

    public void StartFire()
    {
        fireObject.SetActive(true);
        CancelInvoke("StopFire");
        Invoke("StopFire", fireTime);
    }

    private void StopFire()
    {
        fireObject.SetActive(false);
    }
}
