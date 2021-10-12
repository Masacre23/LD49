using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Camera cam;
    [SerializeField] Transform finalTrans;
    

    private void Update() {
        cam.transform.position = Vector3.Lerp(cam.transform.position, finalTrans.position, 0.5f * Time.deltaTime);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, finalTrans.rotation, 0.5f * Time.deltaTime);
    }

    public void Play() {
        gameObject.SetActive(false);
        player.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
