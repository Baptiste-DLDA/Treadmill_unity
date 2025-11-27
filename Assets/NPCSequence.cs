using UnityEngine;
using UnityEngine.AI; // Indispensable pour le NavMesh
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class NPCSequence : MonoBehaviour
{
    [Header("Destinations")]
    public Transform destinationB; // Premier arrêt
    public Transform destinationC; // Destination finale

    [Header("Réglages")]
    public float rotationSpeed = 5f; // Vitesse de rotation "naturelle"

    private NavMeshAgent agent;
    private Animator animator;
    private bool sequenceStarted = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Désactiver la rotation automatique du NavMesh pour la gérer nous-même si besoin, 
        // mais ici on la laisse pour la marche, on la coupera pour le "coucou"
        agent.updateRotation = true;

        // Commencer la marche vers B
        GoToPoint(destinationB.position);
    }

    void Update()
    {
        // 1. Gestion de l'animation (Déjà présent)
        bool isMoving = agent.velocity.sqrMagnitude > 0.1f && agent.remainingDistance > agent.stoppingDistance;
        animator.SetBool("isWalking", isMoving);

        // 2. Arrivée au Point B (Déjà présent)
        if (!sequenceStarted && Vector3.Distance(transform.position, destinationB.position) <= agent.stoppingDistance + 0.2f)
        {
            if (!agent.pathPending)
            {
                StartCoroutine(SequenceAtPointB());
            }
        }

        // 3. NOUVEAU : Arrivée au Point C (Fin de vie)
        // On vérifie "sequenceStarted" pour être sûr qu'il a déjà fait son étape B
        if (sequenceStarted && Vector3.Distance(transform.position, destinationC.position) <= agent.stoppingDistance + 0.5f)
        {
            Destroy(gameObject); // Le personnage disparaît de la scène
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
        agent.isStopped = true; // On arrête l'agent

        // 1. Se tourner vers l'orientation du point B (pour faire face au joueur par exemple)
        // On utilise la rotation de l'objet "DestinationB"
        yield return StartCoroutine(SmoothRotate(destinationB.rotation));

        // 2. Faire coucou
        animator.SetTrigger("doWave");

        // On attend la fin de l'animation (environ 2-3 secondes selon l'anim)
        yield return new WaitForSeconds(2.5f);

        // 3. Se tourner vers le point C (Optionnel, mais plus naturel avant de marcher)
        Vector3 directionToC = (destinationC.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToC.x, 0, directionToC.z));
        yield return StartCoroutine(SmoothRotate(lookRotation));

        // 4. Repartir vers C
        GoToPoint(destinationC.position);
    }

    // Coroutine pour faire une rotation fluide (look naturel)
    IEnumerator SmoothRotate(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null; // Attendre la frame suivante
        }
    }
}