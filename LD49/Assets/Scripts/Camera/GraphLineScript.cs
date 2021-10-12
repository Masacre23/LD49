using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphLineScript : MonoBehaviour
{

    public Text text;
    GameManager gameManager;

    private LineRenderer line;
    private float lineScale = 0.50f;

    private float startValue = 50f;
    private float stock;
    private int pos = 5;

    private float variance = 5f;
    private float multiplier = 20;
    private float cooldown = 5f;

    private float increase = 0f;

    private bool alarmStateEnabled = false;
    private float alarmValue = -1f;
    private float loseValue = -15f;

    private bool win = false;
    private float winCondition = 300f;
    private float winTimeCounter = 0f;

    private float startDate = 61200f;

    private float unstableObjectModifier = 0.2f;
    private float pendingRequestModifier = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        stock = startValue + multiplier * variance;

        line = GetComponent<LineRenderer>();
        line.positionCount = pos + 1;
        line.SetPosition(0, Vector3.zero);
        line.SetPosition(1, new Vector3(1f, 2f, 0f));
        line.SetPosition(2, new Vector3(2f, 4f, 0f));
        line.SetPosition(3, new Vector3(3f, 2f, 0f));
        line.SetPosition(4, new Vector3(4f, 6f, 0f));
        line.SetPosition(5, new Vector3(5f, 8f, 0f));
        InvokeRepeating("UpdateGraph", cooldown, cooldown);
    }

    private void Update()
    {
        if (win) return;

        //Debug.Log(winTimeCounter);
        if (stock > loseValue)
        {
            if (winTimeCounter >= winCondition)
            {
                gameManager.GameWin();
                win = true;
            }
        }

        winTimeCounter += Time.deltaTime;
        text.text = FormatTime(winTimeCounter);
    }

    public void StopGraph()
    {
        CancelInvoke("InvokeRepeating");
    }

    private void UpdateGraph()
    {
        var unstableObjects = gameManager.GetUnstableObjects();
        var pedingRequests = gameManager.GetPendingRequest();
        Debug.Log(pedingRequests);

        pos++;

        var unstableMod = Mathf.Max(1, unstableObjects * unstableObjectModifier);
        var pendingRequestMod = Mathf.Max(1, pedingRequests * pendingRequestModifier);

        stock -= UnityEngine.Random.Range(-variance * multiplier/2f, (variance * multiplier) * unstableMod * pendingRequestMod);
        stock += increase;

        increase = 0;
        line.positionCount = pos + 1;

        var newValue = stock / startValue;
        line.SetPosition(pos, new Vector3(pos, newValue, 0));
        if (pos > 30) transform.Translate(Vector3.left * lineScale);

        if (newValue <= loseValue) {
            gameManager.GameOver();
        } else if (newValue <= alarmValue) {
            if (!alarmStateEnabled) gameManager.SetAlarmState(true);
        } else {
            if (alarmStateEnabled) gameManager.SetAlarmState(false);
        }

        alarmStateEnabled = newValue <= alarmValue;
    }

    public void IncreaseStock(float value)
    {
        increase = UnityEngine.Random.Range(multiplier, variance * multiplier * value);
    }

    public string FormatTime(float time) {
        TimeSpan timespan = TimeSpan.FromSeconds(time + 60 * 8);

        string str = timespan.ToString(@"mm\:ss");

        return str;
    }
}
