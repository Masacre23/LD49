using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCRequestScript : MonoBehaviour
{
    GameManager gameManager;

    public ItemType[] requests;
    public Image imagePlaceholder;
    private Sprite[] imageItems;

    private float threshold = 120f;
    private float cooldown = 120f;

    //public bool canMakeRequest = false;
    private ItemType request;
    private bool completed = false;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        imageItems = Resources.LoadAll<Sprite>("Items");
    }

    void Start()
    {
        if (requests.Length > 0)
        {
            StartCoroutine("StartRequest");
        }
    }

    public void QuickStart()
    {
        if (requests.Length > 0)
        {
            StopCoroutine("StartRequest");
            ActivateRequest();
        }
    }

    private void ChatBubbleSetActive(bool active)
    {
        imagePlaceholder.transform.parent.gameObject.SetActive(active);
    }

    IEnumerator StartRequest()
    {
        ChatBubbleSetActive(false);
        yield return new WaitForSeconds(Random.Range(0, threshold + cooldown));
        //yield return new WaitUntil(() => canMakeRequest);

        ActivateRequest();

        yield return new WaitUntil( () => completed);

        StartCoroutine("StartRequest");
    }

    private void ActivateRequest()
    {
        completed = false;
        var index = Random.Range(0, requests.Length);
        request = requests[index];
        imagePlaceholder.sprite = imageItems[index];

        ChatBubbleSetActive(true);
        gameManager.AddPendingRequest();
        //Debug.Log("I am NPC " + gameObject.name + " and I need: " + request.ToString());
    }


    public bool CompleteRequest(ItemType type)
    {
        if (request == type && !completed)
        {
            ChatBubbleSetActive(false);
            gameManager.RemovePendingRequest();
            completed = true;
            return true;
        } else
        {
            return false;
        }
    }
}
