using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource backgroundMusicSource;

    [Header("Player UI Elements")]
    public Image InteractButtonImage; // Referência à imagem do botão de interação

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI ghostCountText;
    public TextMeshProUGUI introMessageText; // para a mensagem inicial
    public GameObject pauseMenu; // Referência ao painel do menu de pausa
    public GameObject BookInterface; // Referência à interface do livro

    [Header("Game Settings")]
    public float totalTimeInMinutes = 10f;
    public int totalGhostsToCapture = 5;

    [Header("Intro Message Settings")]
    public float introMessageDisplayTime = 3.0f;
    public float introMessageFadeOutTime = 2.0f;

    private float currentTime;
    private int capturedGhosts = 0;
    private bool gameOver = false;
    private bool isPaused = false; // Para rastrear o estado de pausa

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Garante que o menu de pausa comece desativado
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        currentTime = totalTimeInMinutes * 60f;
        UpdateTimerDisplay();
        UpdateGhostCountDisplay();

        if (introMessageText != null)
        {
            Color initialColor = introMessageText.color;
            initialColor.a = 1f;
            introMessageText.color = initialColor;
            introMessageText.gameObject.SetActive(true);
            StartCoroutine(ShowAndFadeOutIntroMessage());
        }
    }

    void Update()
    {
        if (gameOver)
        {
            return;
        }

        // Se a tecla ESC for pressionada
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Prioridade: Se a interface do livro estiver aberta, feche-a primeiro.
            if (BookInterface != null && BookInterface.activeSelf)
            {
                BookInterface.SetActive(false);
            }
            // Caso contrário, alterne o menu de pausa.
            else
            {
                TogglePauseMenu();
            }
        }
    }

    // Método público para pausar e despausar o jogo
    public void TogglePauseMenu()
    {
        isPaused = !isPaused; // Inverte o estado de pausa

        if (isPaused)
        {
            Time.timeScale = 0f; // Pausa o tempo do jogo
            pauseMenu.SetActive(true); // Ativa o menu de pausa
            if (backgroundMusicSource != null)
            {
                backgroundMusicSource.Pause();
            }
        }
        else
        {
            Time.timeScale = 1f; // Retoma o tempo do jogo
            pauseMenu.SetActive(false); // Desativa o menu de pausa
            if (backgroundMusicSource != null)
            {
                backgroundMusicSource.UnPause();
            }
        }
    }

    IEnumerator ShowAndFadeOutIntroMessage()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(introMessageDisplayTime);
        Time.timeScale = 1f;
        StartCoroutine(GameTimerCoroutine());

        float timer = introMessageFadeOutTime;
        Color currentColor = introMessageText.color;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            currentColor.a = timer / introMessageFadeOutTime;
            introMessageText.color = currentColor;
            yield return null;
        }
        introMessageText.gameObject.SetActive(false);
    }

    IEnumerator GameTimerCoroutine()
    {
        while (!gameOver && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
            yield return null;
        }

        if (currentTime <= 0 && !gameOver)
        {
            EndGame(false);
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 60f)
        {
            timerText.color = Color.red;
        }
    }

    void UpdateGhostCountDisplay()
    {
        ghostCountText.text = $"Fantasmas: {capturedGhosts}/{totalGhostsToCapture}";
    }

    public void GhostCaptured()
    {
        if (gameOver) return;
        capturedGhosts++;
        UpdateGhostCountDisplay();

        if (capturedGhosts >= totalGhostsToCapture)
        {
            EndGame(true);
        }
    }

    void EndGame(bool playerWon)
    {
        gameOver = true;
        Time.timeScale = 0f;

        if (playerWon)
        {
            Debug.Log("Você venceu! Todos os fantasmas foram capturados.");
            SceneManager.LoadScene("GameWinScene");
        }
        else
        {
            Debug.Log("Tempo esgotado! Você perdeu.");
            SceneManager.LoadScene("GameOverScene");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Jogo encerrado.");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MenuScene");
    }

    public void ToggleBookInterface()
    {
        if (BookInterface != null)
        {
            BookInterface.SetActive(!BookInterface.activeSelf);
        }
        else
        {
            Debug.LogWarning("BookInterface não está atribuído no GameManager.");
        }
    }
}
