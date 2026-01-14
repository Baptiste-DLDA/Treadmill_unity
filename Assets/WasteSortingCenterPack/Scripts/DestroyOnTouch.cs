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
        if (other.gameObject.CompareTag("destroyer"))
        {
            if (deathEffect != null)
            {
                GameObject vfx = Instantiate(deathEffect, transform.position, Quaternion.identity);
                Destroy(vfx, 0.5f);
            }

            if (IsABottle())
            {
                if (ScoreManager.instance != null)
                {
                    ScoreManager.instance.AddPoints(-1);
                }

                Debug.Log("-1 Point ! (Objet perdu)");
                DisplayMessage("-1");
            }

            StartCoroutine(KillRoutine());
        }
    }

    bool IsABottle()
    {
        string t = gameObject.tag;
        return t == "BouteilleVerte" || t == "BouteilleBleue" || t == "BouteilleViolette";
    }

    IEnumerator KillRoutine()
    {
        if (GetComponent<Renderer>()) GetComponent<Renderer>().enabled = false;
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        if (GetComponent<Rigidbody>()) GetComponent<Rigidbody>().isKinematic = true;

        yield return new WaitForSeconds(0.5f);

        DisplayMessage("");

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