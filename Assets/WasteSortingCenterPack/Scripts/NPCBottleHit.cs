using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class NPCBottleHit : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Tags des objets considérés comme des bouteilles. Doit correspondre EXACTEMENT aux tags de vos prefabs.")]
    public string[] bottleTags = { "BouteilleVerte", "BouteilleBleue", "BouteilleViolette" };
    public float expulsionForce = 5.0f;
    public float upwardForce = 4.0f;

    [Header("Score")]
    public int scoreReward = 50;

    [Header("Debug")]
    [Tooltip("Cochez cette case en mode PLAY pour tester l'animation et le score sans lancer de bouteille.")]
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

        // Sécurité : On s'assure que le RB est Kinematic au début pour que le NavMesh fonctionne
        if (rb != null) rb.isKinematic = true;
    }

    void Update()
    {
        // Permet de tester via l'inspecteur en cochant la case
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

        // 1. Score
        ScoreManager scoreManager = FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddPoints(scoreReward);
        }

        // 2. Désactiver l'IA
        if (agent != null) agent.enabled = false;
        if (npcSequence != null) npcSequence.enabled = false;

        // 3. Physique (Éjection)
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // Vecteur : Vers l'arrière + Vers le haut
            Vector3 forceDirection = (-transform.forward + Vector3.up).normalized;
            rb.AddForce(forceDirection * expulsionForce + Vector3.up * upwardForce, ForceMode.Impulse);
        }

        // 4. Animation
        if (animator != null)
        {
            animator.applyRootMotion = false;
            animator.SetTrigger("Hit");
        }

        // 5. Destruction
        Destroy(gameObject, 2.5f);
    }
}