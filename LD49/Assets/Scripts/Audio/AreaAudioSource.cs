using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAudioSource : MonoBehaviour
{
    public float maxVolume = 1f;
    private AudioSource audio;
    private bool entering = false;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        audio.volume = 0;
    }

    private void Update()
    {
        if (entering)
        {
            audio.volume = Mathf.Min(audio.volume + Time.deltaTime, maxVolume);
        } else
        {
            audio.volume -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            entering = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            entering = false;
        }
    }

}
