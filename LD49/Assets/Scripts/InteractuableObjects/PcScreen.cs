using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcScreen : ActivableObjectScript
{
    [SerializeField] Material[] goodScreens;
    [SerializeField] Material blueScreen;
    SpeakBubbles toolBubble;

    [SerializeField]MeshRenderer meshRenderer;

    void Start()
    {
        toolBubble = transform.GetComponentInChildren<SpeakBubbles>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        Material[] materials = meshRenderer.materials;
        if (Random.value >= 0.5) materials[1] = goodScreens[0];
        else materials[1] = goodScreens[1];
        meshRenderer.materials = materials;

        toolBubble.gameObject.SetActive(false);

        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        isActive = false;
        Invoke("ActivateObject", Random.Range(0, cooldown + threshold));
    }

    public void BreakComputer() {
        Material[] materials = meshRenderer.materials;
        materials[1] = blueScreen;

        meshRenderer.materials = materials;
        toolBubble.gameObject.SetActive(true);
    }

    public void RepairComputer() {
        Material[] materials = meshRenderer.materials;
        if (Random.value >= 0.5) materials[1] = goodScreens[0];
        else materials[1] = goodScreens[1];
        meshRenderer.materials = materials;
        toolBubble.gameObject.SetActive(false);
    }

    override protected void ActivateObject() {
        base.ActivateObject();
        BreakComputer();
    }

    override public void ProcessSuccess() {
        base.ProcessSuccess();
        RepairComputer();
    }
}
