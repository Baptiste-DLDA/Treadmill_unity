using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startPanel;
    public Spawner objectSpawner;
    public NPCSequence characterSequence;
    public TreadmillsController treadmill;

    // ... (votre code Start reste le même) ...

    // Fonction appelée par le bouton
    public void StartGame()
    {
        Debug.Log("1. Le bouton a bien reçu le clic !"); // Si ce message n'apparait pas, le problème vient de l'UI.

        if (startPanel != null)
        {
            startPanel.SetActive(false);
            Debug.Log("2. Panneau désactivé.");
        }
        else Debug.LogError("ERREUR : StartPanel n'est pas assigné dans l'inspecteur du GameManager !");

        if (objectSpawner != null)
        {
            objectSpawner.enabled = true;
            Debug.Log("3. Spawner activé.");
        }
        else Debug.LogError("ERREUR : ObjectSpawner n'est pas assigné !");

        if (treadmill != null)
        {
            treadmill.enabled = true;
            Debug.Log("4. Tapis activé.");
        }

        if (characterSequence != null)
        {
            characterSequence.StartWalking();
            Debug.Log("5. Le bonhomme marche.");
        }
        else Debug.LogError("ERREUR : CharacterSequence n'est pas assigné !");
    }
}