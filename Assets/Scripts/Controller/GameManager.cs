using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player UI Elements")]
    public Image InteractButtonImage; // Referência à imagem do botão de interação

    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI ghostCountText;
    public TextMeshProUGUI introMessageText; // para a mensagem inicial
    public GameObject pauseMenu; // Referência ao menu de pausa

    [Header("Game Settings")]
    public float totalTimeInMinutes = 10f;
    public int totalGhostsToCapture = 5;

    [Header("Intro Message Settings")] 
    public float introMessageDisplayTime = 3.0f; // Tempo que a mensagem fica totalmente visível
    public float introMessageFadeOutTime = 2.0f; // Tempo para a mensagem sumir gradualmente

    private float currentTime;
    private int capturedGhosts = 0;
    private bool gameOver = false;

    public static GameManager Instance { get; private set; } // singleton para acessar o GameManager de outros scripts

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
        currentTime = totalTimeInMinutes * 60f;
        UpdateTimerDisplay();
        UpdateGhostCountDisplay();

        // Garante que a mensagem inicial esteja visível no começo
        if (introMessageText != null)
        {
            Color initialColor = introMessageText.color;
            initialColor.a = 1f; // Totalmente visível
            introMessageText.color = initialColor;
            introMessageText.gameObject.SetActive(true); // Garante que o objeto está ativo

            // Inicia a coroutine para exibir e depois fazer a mensagem sumir
            StartCoroutine(ShowAndFadeOutIntroMessage());
        }
    }

    void Update()
    {
        if (gameOver)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) 
        {
            // Verifica se o jogo está pausado e alterna entre pausar e retomar
            if (Time.timeScale == 1f)
            {
                Time.timeScale = 0f; // Pausa o jogo
                pauseMenu.SetActive(true); // Ativa o menu de pausa
            }
            else
            {
                Time.timeScale = 1f; // Retoma o jogo
                pauseMenu.SetActive(false); // Desativa o menu de pausa
            }
        }
    }

    // Método para iniciar o cronômetro do jogo
    private void RunGameTimer()
    {
        // Se o jogo não acabou e a mensagem já sumiu, continue o cronômetro
        if (!gameOver)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                currentTime = 0;
                UpdateTimerDisplay();
                EndGame(false);
            }
        }
    }

    IEnumerator ShowAndFadeOutIntroMessage()
    {
        // Garante que o cronômetro do jogo não está ativo enquanto a mensagem é exibida
        Time.timeScale = 0f; // Pausa o jogo enquanto a mensagem está na tela

        // Tempo em que a mensagem fica totalmente visível
        yield return new WaitForSecondsRealtime(introMessageDisplayTime); // Realtime para não ser afetado pelo Time.timeScale = 0f

        // Reinicia o tempo do jogo
        Time.timeScale = 1f;

        // Inicia a contagem regressiva do jogo no Update a partir de agora
        StartCoroutine(GameTimerCoroutine());

        // Faz a mensagem sumir gradualmente
        float timer = introMessageFadeOutTime;
        Color currentColor = introMessageText.color;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            currentColor.a = timer / introMessageFadeOutTime; // Diminui o alpha
            introMessageText.color = currentColor;
            yield return null;
        }

        introMessageText.gameObject.SetActive(false); // Desativa o objeto de texto ao final
    }

    //Coroutine para gerenciar o cronômetro do jogo
    IEnumerator GameTimerCoroutine()
    {
        while (!gameOver && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
            yield return null; // Espera pelo próximo frame
        }

        if (currentTime <= 0 && !gameOver) // Garante que a derrota por tempo só ocorre uma vez
        {
            EndGame(false);
        }
    }


    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (currentTime <= 60f)//mudando a cor do texto do cronômetro para vermelho quando o tempo estiver abaixo de 1 minuto
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
        Time.timeScale = 0f; // Pausa o jogo ao final

        if (playerWon)
        {
            Debug.Log("Você venceu! Todos os fantasmas foram capturados.");
            // Lógica de vitória
            SceneManager.LoadScene("GameWinScene"); // Carrega a cena de vitória
        }
        else
        {
            Debug.Log("Tempo esgotado! Você perdeu.");
            // Lógica de derrota
            SceneManager.LoadScene("GameOverScene"); // Carrega a cena de Game Over
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
