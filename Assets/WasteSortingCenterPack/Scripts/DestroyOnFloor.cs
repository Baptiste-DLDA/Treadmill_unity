using UnityEngine;

public class DestroyOnFloor : MonoBehaviour
{
    // Cette fonction se déclenche quand la bouteille touche quelque chose
    private void OnCollisionEnter(Collision collision)
    {
        // On regarde ce qu'on a touché.
        // On vérifie si l'objet touché est le sol (ou un enfant du sol)
        // Le parent doit s'appeler exactement "Floor" (ou "floor", attention à la majuscule)
        if (collision.gameObject.name == "Floor" ||
           (collision.transform.parent != null && collision.transform.parent.name == "Floor"))
        {
            // On détruit la bouteille (l'objet sur lequel ce script est placé)
            Destroy(gameObject);
        }
    }
}