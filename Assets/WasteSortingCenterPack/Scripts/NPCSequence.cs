using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCSequence : MonoBehaviour
{
    [Header("Destinations")]
    public Transform destinationB; // arrêt où le bonhomme fait coucou
    public Transform destinationC; // la ou il disparait

    [Header("Réglages")]
    public float rotationSpeed = 5f; // Vitesse de rotation naturelle

    private NavMeshAgent agent;
    private Animator animator;
    private bool sequenceStarted = false;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    public void StartWalking()
    {
        agent.updateRotation = true;
        GoToPoint(destinationB.position);
    }

    void Update()
    {
        bool isMoving = agent.velocity.sqrMagnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool("isWalking", isMoving);

        if (!sequenceStarted && Vector3.Distance(transform.position, destinationB.position) <= agent.stoppingDistance + 0.2f)
        {
            if (!agent.pathPending)
            {
                StartCoroutine(SequenceAtPointB());
            }
        }

        if (sequenceStarted && Vector3.Distance(transform.position, destinationC.position) <= agent.stoppingDistance + 0.5f)
        {
            Destroy(gameObject);
        }
    }

    void GoToPoint(Vector3 targetPos)
    {
        agent.isStopped = false;
        agent.SetDestination(targetPos);
    }

    IEnumerator SequenceAtPointB()
    {
        sequenceStarted = true;
        agent.isStopped = true;

        // se tourner vers orientation du point B (pour faire face au joueur)
        yield return StartCoroutine(SmoothRotate(destinationB.rotation));

        // faire coucou
        animator.SetTrigger("doWave");

        yield return new WaitForSeconds(2.5f);

        // e tourner vers le point C 
        Vector3 directionToC = (destinationC.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToC.x, 0, directionToC.z));
        yield return StartCoroutine(SmoothRotate(lookRotation));

        GoToPoint(destinationC.position);
    }

    IEnumerator SmoothRotate(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
    }
}