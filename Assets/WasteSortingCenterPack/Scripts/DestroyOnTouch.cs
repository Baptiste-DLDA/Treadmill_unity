using System.Collections; // <--- INDISPENSABLE pour les Coroutines
using UnityEngine;
using UnityEngine.UI; // Si tu utilises du Text Legacy

public class DestroyOnTouch : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("destroyer"))
        {
            // Au lieu de détruire tout de suite, on lance la séquence de fin
            StartCoroutine(KillRoutine());
        }
    }

    IEnumerator KillRoutine()
    {
        // 1. Si c'est une bouteille, on affiche le message
        bool isBottle = gameObject.CompareTag("bottle");

        if (isBottle)
        {
            DisplayMessage("Cet objet est à trier !");
        }

        // 2. MODE FANTÔME : On cache l'objet pour donner l'impression qu'il est détruit
        // On coupe le visuel
        if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = false;
        // On coupe la physique pour qu'il ne cogne plus rien
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        // On le fige pour qu'il ne tombe pas à l'infini (optionnel mais propre)
        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = true;

        // 3. Si c'est une bouteille, on attend 1 seconde
        if (isBottle)
        {
            yield return new WaitForSeconds(1.5f); // Pause de 1 sec
            DisplayMessage(""); // On efface le message
        }

        // 4. Destruction réelle de l'objet
        Destroy(gameObject);
    }

    void DisplayMessage(string message)
    {
        GameObject textObject = GameObject.Find("TexteInfo");
        if (textObject != null)
        {
            var textComponent = textObject.GetComponent<Text>();
            if (textComponent != null) textComponent.text = message;
        }
    }
}