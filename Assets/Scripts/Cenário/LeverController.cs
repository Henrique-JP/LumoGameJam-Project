using UnityEngine;
using System.Collections;
using Unity.Cinemachine; 

public class LeverController : MonoBehaviour
{
    [Header("Configurações da Porta")]
    public GameObject doorObject; // O objeto da porta que será aberta
    // public Animator doorAnimator; 
    // public string doorOpenAnimationTrigger = "OpenDoor"; // Nome do trigger no Animator

    [Header("Configurações da Câmera")]
    public CinemachineCamera playerFollowVCam; // A VCam que segue o jogador
    public CinemachineCamera doorFeedbackVCam; // A VCam que mostra a porta
    public float cameraSwitchDuration = 2.0f; // Tempo que a câmera ficará na porta

    private bool isLeverActivated = false; // Para evitar ativações múltiplas
    private bool isPlayerInTrigger = false; // Para verificar se o jogador está na área do trigger
    private GameManager gameManager; // Referência ao GameManager

    void Start()
    {
        // Inicializa o GameManager
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // reage apenas se o objeto que entrou no trigger for o jogador
        if (other.CompareTag("Player") && !isLeverActivated) 
        {
            Debug.Log("Player entrou na área da alavanca!");
            gameManager.InteractButtonImage.gameObject.SetActive(true); // Habilita o botão de interação
            isPlayerInTrigger = true; // Marca que o jogador está na área do trigger        
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Verifica se o objeto que saiu do trigger é o jogador
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player saiu da área da alavanca!");
            gameManager.InteractButtonImage.gameObject.SetActive(false); // Desabilita o botão de interação
            isPlayerInTrigger = false; // Marca que o jogador não está mais na área do trigger
        }
    }

    void Update()
    {
        // Verifica se a alavanca já foi ativada e se o jogador está na área do trigger e se está pressionando a tecla 'E'
        if (!isLeverActivated && isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            ActivateLever();
            gameObject.GetComponent<SpriteRenderer>().color = Color.green; //Placeholder para indicar que a alavanca foi ativada
            gameManager.InteractButtonImage.gameObject.SetActive(false); // Desabilita o botão de interação após ativar a alavanca
        }
    }

    void ActivateLever()
    {
        isLeverActivated = true;
        Debug.Log("Alavanca ativada!");

        // Inicia a rotina para abrir a porta e mudar a câmera
        StartCoroutine(OpenDoorAndSwitchCamera());
    }

    IEnumerator OpenDoorAndSwitchCamera()
    {
        // sistema de prioridade de câmeras serve para controlar qual câmera está ativa
        doorFeedbackVCam.Priority = 20; // Maior que a playerFollowVCam (10)
        playerFollowVCam.Priority = 10; // Mantém a prioridade normal do player follow, mas agora ela é menor

        // Espera um pequeno tempo para a câmera começar a transição antes de abrir a porta
        yield return new WaitForSeconds(0.2f); 

        // Abrir a porta
        if (doorObject != null)
        {
            // if (doorAnimator != null)
            // {
            //     doorAnimator.SetTrigger(doorOpenAnimationTrigger);
            // }

            yield return new WaitForSeconds(1);
            Destroy(doorObject); // Placeholder destrói a porta após abrir
        }

        // Espera o tempo de visualização da porta
        yield return new WaitForSeconds(cameraSwitchDuration);

        doorFeedbackVCam.Priority = 5; // Volta para prioridade menor
        playerFollowVCam.Priority = 10; // Garante que a playerFollowVCam volte a ser a principal
    }
}