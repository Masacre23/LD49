using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivableObjectScript : InteractuableScript
{
    public GameObject fire = null;
    protected GameManager gameManager;

    public float cooldown = 60f;
    public float threshold = 300f;

    private float fireTime = 20f;
    private float activateCount = 0;

    private void Update()
    {
        if (isActive)
        {
            activateCount += Time.deltaTime;
            if (activateCount >= fireTime && fire != null)
            {
                StartFire();
            }
        }
        
    }

    protected virtual void ActivateObject()
    {
        if (!isActive)
        {
            Debug.Log(transform.parent.name + " is enabled.");
            isActive = true;
            gameManager.AddUnstableObject();
        }
    }

    override public void ProcessSuccess()
    {
        base.ProcessSuccess();
        isActive = false;
        if (fire != null) fire.SetActive(false);

        gameManager.RemoveUnstableObject();
        gameManager.IncreaseStock(1);
        Invoke("ActivateObject", Random.Range(cooldown, cooldown + threshold));
    }

    private void StartFire()
    {
        fire.SetActive(true);

        Invoke("CheckSpherecast", 5f);
    }

    private void CheckSpherecast()
    {
        if (isActive)
        {
            var collisions = Physics.OverlapSphere(transform.position, 5f);

            foreach (var hit in collisions)
            {
                if (hit.CompareTag("Flammable"))
                {
                    hit.gameObject.GetComponent<PropagableFire>().StartFire();
                }
            }
        }
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position + Vector3.forward, 5f);
    //}
}
