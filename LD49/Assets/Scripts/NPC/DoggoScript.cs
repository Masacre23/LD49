using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoScript : MonoBehaviour
{

    GameManager gameManager;

    public GameObject bubble;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.alarmOn)
        {
            bubble.SetActive(true);
        } else
        {
            bubble.SetActive(false);
        }
    }

}
