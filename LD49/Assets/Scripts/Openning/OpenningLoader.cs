using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpenningLoader : MonoBehaviour
{
    public string sceneToLoad;
    public int timeBetweenTexts = 3;
    public GameObject[] texts;
    private int i;
    private float time;
    private bool openningHasFinished = false;

    private void Start() {
        StartCoroutine(LoadMainMenu());
    }

    void FixedUpdate() {
        time += Time.deltaTime;
        if(time >= timeBetweenTexts) {
            time = 0;
            if (i == texts.Length - 1)
                openningHasFinished = true;
            else {
                texts[i].SetActive(false);
                ++i;
                texts[i].SetActive(true);
            }
        }
    }

    IEnumerator LoadMainMenu() {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone) {
            //Output the current progress
            //m_Text.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f) {
                //Change the Text to show the Scene is ready
                //m_Text.text = "Press the space bar to continue";
                asyncOperation.allowSceneActivation = openningHasFinished;
            }

            yield return null;
        }
    }
}
