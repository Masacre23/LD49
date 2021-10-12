using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : ActivableObjectScript
{
    public AudioSource audio;

    Animator animator;
    private void Start() {
        animator = transform.parent.GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        isActive = false;
        Invoke("ActivateObject", Random.Range(cooldown, cooldown + threshold));
    }

    public override void ProcessSuccess() {
        base.ProcessSuccess();
        animator.SetBool("Ringing", false);
        audio.Stop();
    }

    override protected void ActivateObject() {
        base.ActivateObject();
        animator.SetBool("Ringing", true);
        audio.Play();
    }
}
