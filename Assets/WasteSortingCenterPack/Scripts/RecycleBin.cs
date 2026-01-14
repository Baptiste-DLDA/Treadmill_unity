using UnityEngine;

public class RecycleBin : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Le tag exact que ce panier accepte (ex: BouteilleVerte)")]
    public string validTag;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(validTag))
        {
            ScoreManager.instance.AddPoints(5);

            Destroy(other.gameObject);

            Debug.Log("Tri réussi !");
        }
        else if (other.CompareTag("BouteilleVerte") || other.CompareTag("BouteilleBleue") || other.CompareTag("BouteilleViolette"))
        {
            // Si c'est une bouteille mais la mauvaise couleur
            Debug.Log("Mauvais panier !");
        }
    }
}