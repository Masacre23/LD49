using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] float fadeTime = 2f;
    public Image fadePanel;

    public enum GameState { MAIN_MENU, RUNNING, PAUSE, FINISH }
    private GameState state = GameState.MAIN_MENU;
    //public AudioManager audioManager;


    public GameObject graphObject;
    private GraphLineScript graph;
    private int unstableObjects = 0;
    private int pendingRequest = 0;

    CameraFilterPack_Color_RGB cameraColor;
    public AudioClip[] music;
    private AudioSource cameraAudio;

    BreakableWindow[] windows;
    NPCNavScript[] npcs;
    NPCRequestScript boss;

    GameObject gameWin;
    GameObject gameOver;

    public bool alarmOn = false;

    void Start()
    {
        var obj = Instantiate(graphObject, transform);
        graph = obj.GetComponentInChildren<GraphLineScript>();

       
        cameraColor = Camera.main.GetComponent<CameraFilterPack_Color_RGB>();
        try {
            cameraAudio = GameObject.Find("Music").GetComponent<AudioSource>();
            cameraAudio.clip = music[0];
        } catch {
            //cameraAudio = Camera.main.GetComponent<AudioSource>();
            //cameraAudio.clip = music[0];
            //cameraAudio.Play();
        }

        windows = GameObject.FindObjectsOfType<BreakableWindow>();
        npcs = GameObject.FindObjectsOfType<NPCNavScript>();
        npcs[Random.Range(0, npcs.Length)].GetComponent<NPCRequestScript>().QuickStart();
        boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<NPCRequestScript>();

        gameOver = GameObject.Find("GameOver");
        gameOver.SetActive(false);
        gameWin = GameObject.Find("GameWin");
        gameWin.SetActive(false);
    }

    //public bool CanMakeRequest()
    //{

    //}

    public void IncreaseStock(float value)
    {
        graph.IncreaseStock(value);
    }

    public void GameWin() {

        if (state != GameState.FINISH)
        {
            state = GameState.FINISH;
            graph.StopGraph();
            StartCoroutine(GameWinFading());

            Debug.Log("YOU WON!!");
        }
    }

    public void GameOver()
    {
        if (state != GameState.FINISH)
        {
            state = GameState.FINISH;
            graph.StopGraph();
            StartCoroutine(GameOverFading());

            cameraAudio.clip = music[0];
            cameraAudio.Play();
            Debug.Log("GAME OVER!!");
        }
    }

    IEnumerator GameWinFading() {

        //Fade prepare scene unfade
        for (float t = 0.0f; t < fadeTime;) {
            t += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }
        Camera.main.gameObject.SetActive(false);
        gameWin.SetActive(true);
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().enabled = false;
        player.SetActive(false);
        GameObject model = player.transform.Find("Model").transform.GetChild(PlayerEditor.index).gameObject;
        PlayerEditor.index = 0;
        GameObject copy = GameObject.Instantiate(model);
        copy.transform.position = GameObject.Find("GameWinPlaceholder").transform.position;


        for (float t = fadeTime; t > 0.0f;) {
            t -= Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }

        //Wait
        for (float t = 5f; t > 0.0f;) {
            t -= Time.deltaTime;
            yield return null;
        }

        //Fade
        for (float t = 0.0f; t < fadeTime;) {
            t += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }

        SceneManager.LoadScene(1);
    }

    IEnumerator GameOverFading() {

        //Fade prepare scene unfade
        for (float t = 0.0f; t < fadeTime;) {
            t += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }
        Camera.main.gameObject.SetActive(false);
        gameOver.SetActive(true);
        GameObject player = GameObject.Find("Player");
        player.GetComponent<PlayerController>().enabled = false;
        player.SetActive(false);
        GameObject model = player.transform.Find("Model").transform.GetChild(PlayerEditor.index).gameObject;
        PlayerEditor.index = 0;
        GameObject copy = GameObject.Instantiate(model);
        copy.transform.position = GameObject.Find("GameOverPlaceholder").transform.position;

        for (float t = fadeTime; t > 0.0f;) {
            t -= Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }

        //Wait
        for (float t = 5f; t > 0.0f;) {
            t -= Time.deltaTime;
            yield return null;
        }

        //Fade
        for (float t = 0.0f; t < fadeTime;) {
            t += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }
        SceneManager.LoadScene(1);
    }

    public void SetAlarmState(bool value)
    {
        if (value)
        {
            Debug.Log("ALAAAAAARMAAA!!");
            alarmOn = true;

            StopCoroutine("AlarmLoop");
            StopCoroutine("DisableAlarm");
            StartCoroutine("EnableAlarm");

            cameraAudio.clip = music[1];
            cameraAudio.Play();

            foreach (var window in windows)
            {
                window.useCollision = true;
            }

            CancelInvoke("StartBossRequest");
            Invoke("StartBossRequest", 10f);
        }
        else
        {
            Debug.Log("ALARM DISABLED");
            alarmOn = false;

            StopCoroutine("EnableAlarm");
            StopCoroutine("AlarmLoop");
            StartCoroutine("DisableAlarm");

            cameraAudio.clip = music[0];
            cameraAudio.Play();
        }

        SetPanic(value);
    }

    private void StartBossRequest()
    {
        boss.QuickStart();
    }

    public int GetUnstableObjects()
    {
        return unstableObjects;
    }

    public void AddUnstableObject()
    {
        unstableObjects++;
    }

    public void RemoveUnstableObject()
    {
        unstableObjects--;
    }

    public int GetPendingRequest()
    {
        return pendingRequest;
    }

    public void AddPendingRequest()
    {
        pendingRequest++;
    }

    public void RemovePendingRequest()
    {
        pendingRequest--;
    }

    IEnumerator EnableAlarm()
    {
        var startColor = cameraColor.ColorRGB.g;
        for (float t = startColor; t > 0; t -= Time.deltaTime / 2f)
        {
            var min = Mathf.Max(t, 0.3f);
            cameraColor.ColorRGB = new Color(1, min, min);
            yield return null;
        }

        StartCoroutine("AlarmLoop");
    }

    private void SetPanic(bool isPanic)
    {
        if (npcs != null)
        {
            foreach (var npc in npcs)
            {
                npc.SetPanicState(isPanic);
            }
        }
    }

    IEnumerator AlarmLoop()
    {
        for (float t = 0.3f; t < 1f; t += Time.deltaTime / 2f)
        {
            cameraColor.ColorRGB = new Color(1, t, t);
            yield return null;
        }

        for (float t = 1; t > 0; t -= Time.deltaTime / 2f)
        {
            var min = Mathf.Max(t, 0.3f);
            cameraColor.ColorRGB = new Color(1, min, min);
            yield return null;
        }

        StartCoroutine("AlarmLoop");
    }

    IEnumerator DisableAlarm()
    {
        var startColor = cameraColor.ColorRGB.g;
        for (float t = startColor; t < 1f; t += Time.deltaTime / 2f)
        {
            cameraColor.ColorRGB = new Color(1, t, t);
            yield return null;
        }
    }

    IEnumerator Fading() {
        for (float t = 0.0f; t < fadeTime;) {
            t += Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }

    }
    IEnumerator UnFading() {
        for (float t = fadeTime; t > 0.0f;) {
            t -= Time.deltaTime;
            fadePanel.color = new Color(0f, 0f, 0f, t / (fadeTime));
            yield return null;
        }
    }
}
