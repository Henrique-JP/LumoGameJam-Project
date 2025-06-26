using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadScene : MonoBehaviour
{
    private Button button;
    public string SceneName;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(Load);
        }
    }

    public void Load()
    {
        if (!string.IsNullOrEmpty(SceneName))
        {
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            Debug.LogError("Scene name is not set.");
        }
    } 
}
