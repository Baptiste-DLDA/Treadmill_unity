using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Nécessaire pour la liste de sécurité

public class EndZonePenalty : MonoBehaviour
{
    [Header("Effets")]
    public GameObject flameEffectPrefab;

    [Header("UI (Optionnel)")]
    public Text infoText; // Ou TextMeshProUGUI si tu as changé

    // Sécurité anti-doublon : on garde en mémoire les objets qu'on est en train de détruire
    private List<int> processedObjects = new List<int>();

    void OnTriggerEnter(Collider other)
    {
        // 1. On récupère l'objet racine (au cas où on touche juste le bouchon)
        GameObject obj = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        int objectID = obj.GetInstanceID();

        // 2. SÉCURITÉ : Si on a déjà traité cet objet, on arrête tout de suite !
        if (processedObjects.Contains(objectID)) return;

        if (IsABottle(obj))
        {
            // On ajoute l'objet à la liste des traités pour ne pas le recompter
            processedObjects.Add(objectID);

            // 3. Pénalité de Score (on passe la position pour le texte flottant)
            if (ScoreManager.instance != null)
            {
                // On passe la position de l'objet pour que le -1 apparaisse dessus
                ScoreManager.instance.AddPoints(-1, obj.transform.position);
            }

            // 4. Message Text (Optionnel)
            if (infoText != null)
            {
                infoText.text = "Perdu !";
                Invoke("ClearMessage", 1.0f);
            }

            // 5. Feu
            if (flameEffectPrefab != null)
            {
                GameObject fire = Instantiate(flameEffectPrefab, transform.position, Quaternion.identity);
                Destroy(fire, 0.5f);
            }

            // 6. Destruction
            Destroy(obj);

            // Nettoyage de la liste de sécurité (pas strictement nécessaire car l'ID meurt avec l'objet, mais propre)
            // On le laisse dans la liste le temps qu'il soit détruit par Unity
        }
        else
        {
            // Si c'est un autre débris, on le détruit juste
            Destroy(obj);
        }
    }

    void ClearMessage()
    {
        if (infoText != null) infoText.text = "";
    }

    bool IsABottle(GameObject obj)
    {
        return obj.CompareTag("BouteilleVerte") ||
               obj.CompareTag("BouteilleBleue") ||
               obj.CompareTag("BouteilleViolette");
    }
}