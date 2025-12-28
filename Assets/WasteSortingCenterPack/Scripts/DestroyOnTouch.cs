using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DestroyOnTouch : MonoBehaviour
{
    [Header("Effets Sortie")]
    [Tooltip("L'effet de feu")]
    [SerializeField] private GameObject deathEffect;

    void OnTriggerEnter(Collider other)
    {
        // Si la bouteille touche la zone "destroyer" (la fin du tapis)
        if (other.gameObject.CompareTag("destroyer"))
        {
            // 1. Apparition du Feu
            if (deathEffect != null)
            {
                // On fait apparaître le feu
                GameObject vfx = Instantiate(deathEffect, transform.position, Quaternion.identity);
                // On détruit le feu après 0.5 secondes (comme demandé)
                Destroy(vfx, 0.5f);
            }

            // 2. Gestion du Score et Message
            if (IsABottle())
            {
                // On enlève 1 point
                if (ScoreManager.instance != null)
                {
                    ScoreManager.instance.AddPoints(-1);
                }

                // Afficher le message "-1" (via votre système de message existant ou console)
                Debug.Log("-1 Point ! (Objet perdu)");
                DisplayMessage("-1");
            }

            // 3. Lancer la suppression
            StartCoroutine(KillRoutine());
        }
    }

    // Cette fonction permet de vérifier si c'est une bouteille, peu importe sa couleur
    bool IsABottle()
    {
        string t = gameObject.tag;
        return t == "BouteilleVerte" || t == "BouteilleBleue" || t == "BouteilleViolette";
    }

    IEnumerator KillRoutine()
    {
        // On cache l'objet immédiatement pour simuler sa destruction
        if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = false;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = true;

        // On laisse le temps au message "-1" d'être lu (0.5 à 1 seconde)
        yield return new WaitForSeconds(0.5f);

        DisplayMessage(""); // On efface le message

        // Destruction finale
        Destroy(gameObject);
    }

    void DisplayMessage(string message)
    {
        GameObject textObject = GameObject.Find("TexteInfo"); // Assurez-vous que votre UI s'appelle bien ainsi
        if (textObject != null)
        {
            var textComponent = textObject.GetComponent<Text>();
            if (textComponent != null) textComponent.text = message;
        }
    }
}