using UnityEngine;
using TMPro; // Nécessaire pour toucher à la couleur du texte

public class FloatingText : MonoBehaviour
{
    [Header("Paramètres")]
    [Tooltip("Vitesse de montée")]
    public float moveSpeed = 1.5f;

    [Tooltip("Durée de vie en secondes")]
    public float lifeTime = 1.0f;

    private TextMeshPro textMesh;
    private Color textColor;
    private float timer;

    void Start()
    {
        // On récupère le composant texte pour faire un fondu (optionnel)
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null) textColor = textMesh.color;

        // On détruit l'objet automatiquement après 'lifeTime' secondes
        Destroy(gameObject, lifeTime);

        // Optionnel : On fait regarder le texte vers le joueur (la caméra)
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }

    void Update()
    {
        // 1. Faire monter le texte
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 2. (Optionnel) Faire disparaître le texte progressivement (Fade out)
        if (textMesh != null)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / lifeTime);
            textMesh.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
        }
    }
}