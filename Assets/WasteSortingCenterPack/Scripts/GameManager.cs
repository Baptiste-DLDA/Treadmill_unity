using UnityEngine;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Réglages Difficulté (Temps en secondes)")]
    public float timeLevelGreen = 180f;
    public float timeLevelOrange = 60f;
    public float timeLevelRed = 30f;

    [Header("Réglages Etoiles (Score requis)")]
    public int scoreForOneStar = 50;
    public int scoreForTwoStars = 100;
    public int scoreForThreeStars = 300;

    [Header("Références UI")]
    public TextMeshProUGUI timerText;
    public GameObject endGamePanel;
    public TextMeshProUGUI endScoreText;
    public TextMeshProUGUI endStarsText;
    public TextMeshProUGUI endMessageText;

    [Header("Références Jeu")]
    public GameObject treadmillController;
    public GameObject npcSpawner;
    public GameObject wasteSpawner;

    private float currentTime;
    private bool isGameActive = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StopGameLogic();

        if (endGamePanel != null) endGamePanel.SetActive(true);
        if (endMessageText != null) endMessageText.text = "Choisissez un bouton pour commencer !";
        if (endScoreText != null) endScoreText.text = "";
        if (endStarsText != null) endStarsText.text = "";
    }

    private void Update()
    {
        if (isGameActive)
        {
            currentTime -= Time.deltaTime;

            if (timerText != null)
            {
                float minutes = Mathf.FloorToInt(currentTime / 60);
                float seconds = Mathf.FloorToInt(currentTime % 60);
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }

            if (currentTime <= 0)
            {
                currentTime = 0;
                EndGame();
            }
        }
    }

    public void StartGame(float duration)
    {
        if (isGameActive) return;

        currentTime = duration;
        isGameActive = true;

        // CORRECTION ICI : instance en minuscule
        if (ScoreManager.instance != null) ScoreManager.instance.ResetScore();

        if (endGamePanel != null) endGamePanel.SetActive(false);

        SetGameComponentsState(true);

        Debug.Log("Jeu lancé pour " + duration + " secondes !");
    }

    private void EndGame()
    {
        isGameActive = false;
        SetGameComponentsState(false);

        // CORRECTION ICI : instance en minuscule + accès à .score qui est maintenant public
        int currentScore = ScoreManager.instance != null ? ScoreManager.instance.score : 0;

        int stars = 0;
        if (currentScore >= scoreForThreeStars) stars = 3;
        else if (currentScore >= scoreForTwoStars) stars = 2;
        else if (currentScore >= scoreForOneStar) stars = 1;

        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);

            if (endScoreText != null) endScoreText.text = "Score Final : " + currentScore;

            if (endStarsText != null)
            {
                string starString = "";
                for (int i = 0; i < stars; i++) starString += "★ ";
                endStarsText.text = "Étoiles : " + starString;
            }

            if (endMessageText != null) endMessageText.text = "Appuyez sur un bouton couleur pour recommencer";
        }
    }

    private void SetGameComponentsState(bool state)
    {
        if (treadmillController != null)
        {
            var script = treadmillController.GetComponent<TreadmillsController>();
            if (script) script.enabled = state;
        }

        if (npcSpawner != null)
        {
            var script = npcSpawner.GetComponent<NPCSpawner>();
            if (script) script.enabled = state;
        }

        if (wasteSpawner != null)
        {
            // CORRECTION ICI : "spawner" (minuscule) au lieu de "Spawn"
            var script = wasteSpawner.GetComponent<Spawner>();
            if (script) script.enabled = state;
        }
    }

    private void StopGameLogic()
    {
        SetGameComponentsState(false);
    }
}