using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NextPage : MonoBehaviour
{
    private Button button;
    public GameObject NextPageObject;
    public GameObject PreviusPageObject;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(NextPageButton);
        }
    }

    public void NextPageButton()
    {
        if (NextPageObject != null)
        {
            NextPageObject.SetActive(true);
        }

        if (PreviusPageObject != null)
        {
            PreviusPageObject.SetActive(false);
        }
    }
}
