using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using UnityEngine;

public class InteractableReading : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject ReadingIcon;
    private bool isPlayerInTrigger = false;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInTrigger)
            {
                // Verifica se o ícone de leitura já está ativo
                if (ReadingIcon.activeSelf)
                {
                    // Se o ícone já está ativo, desativa-o
                    ReadingIcon.SetActive(false);
                }
                else
                {
                    // Se o ícone não está ativo, ativa-o
                    ReadingIcon.SetActive(true);
                }
            }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            gameManager.InteractButtonImage.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            gameManager.InteractButtonImage.gameObject.SetActive(false);
            ReadingIcon.SetActive(false);
        }
    }
}
