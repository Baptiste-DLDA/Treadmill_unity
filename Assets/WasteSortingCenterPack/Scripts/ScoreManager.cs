using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Singleton (minuscule)

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject minusOnePopupPrefab;

    public int score = 0; // PASSÉ EN PUBLIC pour que le GameManager puisse le lire

    void Awake()
    {
        instance = this;
        UpdateScoreUI();
    }

    public void AddPoints(int points, Vector3 position = default)
    {
        score += points;

        if (score < 0)
        {
            score = 0;
        }

        UpdateScoreUI();

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

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }
}