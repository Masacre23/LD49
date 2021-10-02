using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnEnableSelectButton : MonoBehaviour {

    [SerializeField] Button button;

    private void OnEnable()
    {
        StartCoroutine(SelectDelayed());
    }

    IEnumerator SelectDelayed()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }
}
