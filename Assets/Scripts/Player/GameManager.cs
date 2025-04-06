using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;               // Score display
    public TextMeshProUGUI timerText;               // Timer display
    public GameObject endGamePanel;                 // Unified end game panel
    public TextMeshProUGUI endGameMessageText;      // Message display ("You Win!" / "Game Over")

    [Header("Game Settings")]
    public float timeLimit = 120f;                  // Total game time in seconds
    public int totalBoxes = 0;                      // Total boxes to collect (can be auto-counted)

    private int score = 0;
    private float remainingTime;
    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        remainingTime = timeLimit;
        UpdateScoreUI();
        UpdateTimerUI();

        if (endGamePanel) endGamePanel.SetActive(false);

        // Auto-count collectible boxes if not set manually
        if (totalBoxes <= 0)
        {
            GameObject[] boxes = GameObject.FindGameObjectsWithTag("Collectible");
            totalBoxes = boxes.Length;
        }
    }

    void Update()
    {
        if (isGameOver) return;

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameOver();
        }
        UpdateTimerUI();
    }

    public void BoxCollected()
    {
        totalBoxes--;
        AddScore(1);

        if (totalBoxes <= 0)
        {
            WinGame();
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText.text != null)
            scoreText.text = "Score: " + score;
    }

    void UpdateTimerUI()
    {
        if (timerText.text != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        ShowEndGamePanel("Game Over");
    }

    public void WinGame()
    {
        if (isGameOver) return;

        isGameOver = true;
        ShowEndGamePanel("You Win!");
    }

    void ShowEndGamePanel(string message)
    {
        if (endGamePanel != null && endGameMessageText.text != null)
        {
            endGameMessageText.text = message;
            endGamePanel.SetActive(true);
        }

        Time.timeScale = 0f; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
