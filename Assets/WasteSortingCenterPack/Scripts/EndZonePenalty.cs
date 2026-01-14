using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EndZonePenalty : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("L'effet de feu")]
    public GameObject flameEffectPrefab;

    [Tooltip("L'endroit EXACT où le feu doit apparaître")]
    public Transform fireSpawnPoint;

    [Header("Apparence du Feu")]
    public Vector3 fireScale = new Vector3(0.5f, 0.5f, 0.5f);
    [Header("UI (Optionnel)")]
    public Text infoText;

    // Sécurité anti-doublon
    private List<int> processedObjects = new List<int>();

    void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        int objectID = obj.GetInstanceID();

        if (processedObjects.Contains(objectID)) return;

        if (IsABottle(obj))
        {
            processedObjects.Add(objectID);

            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddPoints(-1, obj.transform.position);
            }

            if (infoText != null)
            {
                infoText.text = "Perdu !";
                Invoke("ClearMessage", 1.0f);
            }

            if (flameEffectPrefab != null)
            {
                Vector3 spawnPosition = transform.position;
                Quaternion spawnRotation = Quaternion.identity;

                if (fireSpawnPoint != null)
                {
                    spawnPosition = fireSpawnPoint.position;
                    spawnRotation = fireSpawnPoint.rotation;
                }

                GameObject fire = Instantiate(flameEffectPrefab, spawnPosition, spawnRotation);

                fire.transform.localScale = fireScale;

                Destroy(fire, 0.5f);
            }

            Destroy(obj);
        }
        else
        {
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