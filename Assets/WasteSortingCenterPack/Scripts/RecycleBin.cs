using UnityEngine;

public class RecycleBin : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Le tag exact que ce panier accepte (ex: BouteilleVerte)")]
    public string validTag;

    void OnTriggerEnter(Collider other)
    {
        // On vérifie si l'objet qui entre a le bon tag
        if (other.CompareTag(validTag))
        {
            // C'est gagné !
            ScoreManager.instance.AddPoints(5);

            // On détruit l'objet trié
            Destroy(other.gameObject);

            // Optionnel : Jouer un son de succès ici
            Debug.Log("Tri réussi !");
        }
        else if (other.CompareTag("BouteilleVerte") || other.CompareTag("BouteilleBleue") || other.CompareTag("BouteilleViolette"))
        {
            // Si c'est une bouteille mais la MAUVAISE couleur
            // On peut choisir de ne rien faire (elle reste dedans) ou de l'éjecter
            Debug.Log("Mauvais panier !");
        }
    }
}