using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerEditor: MonoBehaviour
{
    [SerializeField] GameObject[] models;
    [SerializeField] Tutorial tutorial;
    [SerializeField] Image fadePanel;
    public static int index = 0;
    GameObject player;
    GameObject tutorialInfo;
    [SerializeField] float fadeTime = 2f;

    IEnumerator UnFade() {
        for (float t = fadeTime; t > 0.0f;) {
            t -= Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }
    }

    IEnumerator Fade() {
        for (float t = 0.0f; t < fadeTime;) {
            t += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void Start() {
        player = GameObject.Find("Player");
        if (player != null) player.SetActive(false);
        tutorial.enabled = false;

        tutorialInfo = GameObject.Find("TutorialInfo");
        tutorialInfo.SetActive(false);

        StartCoroutine(UnFade());
    }

    public void next() {
        models[index].SetActive(false);
        if (index == models.Length - 1) {
            index = 0;
        } else index++;

        models[index].SetActive(true);
    }

    public void previous() {
        models[index].SetActive(false);
        if (index == 0) {
            index = models.Length - 1;
        } else index--;

        models[index].SetActive(true);
    }

    bool accepted = false;
    public void accept() {
        if (accepted) {
            StartCoroutine(Fade());
        } else {
            tutorialInfo.SetActive(true);
            accepted = true;
            tutorial.enabled = true;
        }
    }
}
