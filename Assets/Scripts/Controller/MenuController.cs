using UnityEngine;

public class MenuController : MonoBehaviour
{
    // Método para ser chamado pelo botão "Retomar"
    public void Resume()
    {
        // Acessa o GameManager e chama a função para despausar
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TogglePauseMenu();
        }
    }

    // Método para ser chamado pelo botão "Reiniciar"
    public void Restart()
    {
        // Acessa o GameManager e chama a função para reiniciar
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }

    // Método para ser chamado pelo botão "Menu Principal"
    public void MainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
    }

    // Método para ser chamado pelo botão "Sair"
    public void Quit()
    {
        // Acessa o GameManager e chama a função para sair do jogo
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
    }
}