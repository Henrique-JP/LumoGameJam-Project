using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    private Button quitButton;

    void Start()
    {
        quitButton = GetComponent<Button>();
        quitButton.onClick.AddListener(Quit);
    }

    void Quit()
    {
        Debug.Log("Quit Game");
        Application.Quit();
        
        // If running in the editor, stop playing
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
