using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gameManager;
    CharacterController characterController;

    public float speed = 6.0f;
    public float maxSpeed = 6.0f;
    public float rotationSpeed = 360f;

    private Vector3 movement = Vector3.zero;

    public Transform itemPlaceholder;
    private bool isInteracting = false;

    private InteractuableScript interactuableObject = null;
    private float processTimeout = 0f;
    private float itemProcessTime = 0f;
    private ItemScript item = null;

    Animator animator;

    [SerializeField] SetPosition setPosOfPlaceHolder;

    void Start()
    {
        int index = PlayerEditor.index;
        int count = 0;
        Transform selectedChild;
        foreach(Transform child in transform.Find("Model")) {
            if (count != index) child.gameObject.SetActive(false);
            //Destroy(child.gameObject);
            else selectedChild = child;
            count++;
        }
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        setPosOfPlaceHolder.enabled = true;
        setPosOfPlaceHolder.target = transform.Find("Model").GetChild(index).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0);
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if(animator == null) {
            animator = GetComponentInChildren<Animator>();
        }
        isInteracting = Input.GetButton("Fire1");
        if (isInteracting)
        {
            ProcessItem();
        } else
        {
            animator.SetBool("Interacting", false);
            processTimeout = 0f;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (itemPlaceholder.childCount > 0)
            {
                item = null;
                Destroy(itemPlaceholder.GetChild(0).gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (animator == null) {
            animator = GetComponentInChildren<Animator>();
        }
        animator.SetBool("Grabbing", false);
        movement = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        if (movement != Vector3.zero)
        {
            transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(movement), transform.rotation, 0.45f);

            Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            movement = Input.GetAxis("Vertical") * camForward + Input.GetAxis("Horizontal") * Camera.main.transform.right;

            var itemWeight = item != null ? item.weight : 0;
            movement *= speed - itemWeight;

            if (movement.x != 0f && movement.z != 0f) movement = movement / 1.425f;

            characterController.Move(movement * Time.deltaTime);
        }

        animator.SetBool("HasObject", item != null);
        animator.SetFloat("Speed", movement.magnitude);

        //gravity
        characterController.Move(Vector3.down * 20f * Time.deltaTime);
    }


    private void ProcessItem()
    {
        if (interactuableObject != null && interactuableObject.isActive && item == null)
        {
            processTimeout += Time.deltaTime;
            //Debug.Log("PROCESSING ITEM:" + processTimeout);
            interactuableObject.buttonUI.progress = processTimeout / itemProcessTime;
            animator.SetBool("Interacting", true);
            if (processTimeout >= itemProcessTime)
            {
                animator.SetBool("Grabbing", true);
                interactuableObject.ProcessSuccess();
                InstantiateItem();
                ResetInteractuableObject();
            }
        }
    }

    private void ResetInteractuableObject()
    {
        if (interactuableObject != null) interactuableObject.buttonUI.Hide();
        interactuableObject = null;
        processTimeout = 0f;
        itemProcessTime = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactuable"))
        {
            interactuableObject = other.GetComponent<InteractuableScript>();
            if (interactuableObject.isActive)
            {
                interactuableObject.buttonUI.Show();
                itemProcessTime = interactuableObject.time;
                //Debug.Log("process item time: " + itemProcessTime);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactuable"))
        {
            ResetInteractuableObject();
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("NPC"))
        {
            if (isInteracting && item != null)
            {
                Debug.Log(other.gameObject.transform.parent.name);
                var success = other.GetComponentInParent<NPCRequestScript>().CompleteRequest(item.type);
                if (success)
                {
                    Debug.Log("ITEM " + item.type + " DELIVERED!!");
                    if (itemPlaceholder.childCount > 0)
                    {
                        gameManager.IncreaseStock(1);
                        Destroy(itemPlaceholder.GetChild(0).gameObject);
                    }
                }
                else
                {
                    Debug.Log("ITEM WRONG!");
                }
            }
        } else if (other.CompareTag("Boss"))
        {
            if (isInteracting && item != null)
            {
                Debug.Log(other.gameObject.transform.parent.name);
                var success = other.GetComponentInParent<NPCRequestScript>().CompleteRequest(item.type);
                if (success)
                {
                    Debug.Log("ITEM " + item.type + " DELIVERED!!");
                    if (itemPlaceholder.childCount > 0)
                    {
                        float increaseValue = gameManager.alarmOn ? 6 : 2;
                        gameManager.IncreaseStock(increaseValue);
                        Destroy(itemPlaceholder.GetChild(0).gameObject);
                    }
                }
                else
                {
                    Debug.Log("ITEM WRONG!");
                }
            }
        }
    }

    private void InstantiateItem()
    {
        if (interactuableObject != null && interactuableObject.item != null)
        {
            var obj = Instantiate(interactuableObject.item, itemPlaceholder);
            item = obj.GetComponent<ItemScript>();
        }
    }
}