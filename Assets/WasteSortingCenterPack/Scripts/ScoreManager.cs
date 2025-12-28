using UnityEngine;
using TMPro; // Nécessaire pour le texte

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton pour accès facile

    [Header("UI")]
    public TextMeshProUGUI scoreText; // Glissez votre texte ici
    public GameObject minusOnePopupPrefab; // Optionnel : Prefab texte "-1" flottant

    private int score = 0;

    void Awake()
    {
        instance = this;
        UpdateScoreUI();
    }

    public void AddPoints(int points, Vector3 position = default)
    {
        // On ajoute les points
        score += points;

        // CORRECTION : Si le score est inférieur à 0, on le remet à 0
        if (score < 0)
        {
            score = 0;
        }

        UpdateScoreUI();

        // Si on perd des points (et que points est négatif), on affiche le popup
        // On vérifie aussi que position n'est pas (0,0,0) pour éviter de le faire apparaitre au centre du monde
        if (points < 0 && minusOnePopupPrefab != null && position != Vector3.zero)
        {
            Instantiate(minusOnePopupPrefab, position, Quaternion.identity);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score : " + score;
    }
}