using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class NPCBottleHit : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Tags des objets considérés comme des bouteilles")]
    public string[] bottleTags = { "BouteilleVerte", "BouteilleBleue", "BouteilleViolette" };
    public float expulsionForce = 5.0f;
    public float upwardForce = 4.0f;

    [Header("Score")]
    public int scoreReward = 50;

    [Header("Debug")]
    [Tooltip("si on arrive pas a toucher le bonhomme, on clique la dessus pour tester si ca marche bien")]
    public bool simulerImpact = false;

    private bool hasBeenHit = false;
    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private NPCSequence npcSequence;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        npcSequence = GetComponent<NPCSequence>();

        if (rb != null) rb.isKinematic = true;
    }

    void Update()
    {
        if (simulerImpact)
        {
            simulerImpact = false;
            HitByBottle();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasBeenHit) return;

        if (IsBottle(collision.gameObject))
        {
            HitByBottle();
        }
    }

    private bool IsBottle(GameObject obj)
    {
        foreach (string tag in bottleTags)
        {
            if (obj.CompareTag(tag)) return true;
        }
        return false;
    }

    [ContextMenu("Simuler Impact")]
    public void HitByBottle()
    {
        if (hasBeenHit) return;
        hasBeenHit = true;

        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddPoints(scoreReward);
        }

        if (agent != null) agent.enabled = false;
        if (npcSequence != null) npcSequence.enabled = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            Vector3 forceDirection = (-transform.forward + Vector3.up).normalized;
            rb.AddForce(forceDirection * expulsionForce + Vector3.up * upwardForce, ForceMode.Impulse);
        }

        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetTrigger("Hit");
        }

        Destroy(gameObject, 2.5f);
    }
}