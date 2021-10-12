using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum NpcState { WORKING, WALKING, PANIC }

public class NPCNavScript: MonoBehaviour
{

    public Transform models;

    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 targetPos = Vector3.zero;

    private Transform player;
    private bool lookingPlayer = false;

    private NpcState currentState;

    private float normalSpeed = 2f;
    private float panicSpeed = 6f;

    // Start is called before the first frame update
    void Start()
    {

        Transform child = models.GetChild(Random.Range(0, models.childCount));
        agent = GetComponent<NavMeshAgent>();
        animator = child.GetComponent<Animator>();
        child.gameObject.SetActive(true);

        NormalityState();
        if (currentState == NpcState.WALKING)
        {
            ChangePosition();
        }
    }

    private void NormalityState()
    {
        var rand = Random.Range(0, 2);
        currentState = rand < 1 ? NpcState.WORKING : NpcState.WALKING;
        agent.speed = normalSpeed;
        animator.SetBool("Panicking", false);
    }

    public void SetPanicState(bool isPanic)
    {
        if (isPanic)
        {
            if (currentState == NpcState.WORKING) ChangePosition();
            currentState = NpcState.PANIC;
            agent.speed = panicSpeed;
            animator.SetBool("Panicking", true);
        } else
        {
            NormalityState();
        }
    }

    float GetRange()
    {
        return currentState == NpcState.PANIC ? GetPanicRange() : GetNormalRange();
    }

    float GetNormalRange()
    {
        return Random.Range(5f, 15f);
    }

    float GetPanicRange()
    {
        return Random.Range(1f, 5f);
    }

    void ChangePosition()
    {
        var walkRadius = 20f;
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        targetPos = hit.position;

        //targetPos = new Vector3(Random.Range(-8.0F, 8.0F), 0, Random.Range(-4.5F, 4.5F));
        Invoke("ChangePosition", GetRange());
    }

    void Update()
    {
        if (lookingPlayer && player != null)
        {
            Vector3 relativePos = player.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 2 * Time.deltaTime);
        } else if (targetPos != Vector3.zero)
        {
            agent.SetDestination(targetPos);
            Debug.DrawLine(transform.position, targetPos, Color.red);
        }
        
    }

    private void LateUpdate()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            lookingPlayer = true;
            agent.isStopped = true;
            targetPos = Vector3.zero;
            agent.velocity = Vector3.zero;
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        lookingPlayer = false;
        agent.isStopped = false;
    }

}
