using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    [Header("Configurações da Porta")]
    public GameObject []doorObjects; // O objeto da porta que será aberta
    public Collider2D doorCollider; // O collider da porta que será desativado
    public Sprite LeverDesableSprite;
    public Sprite DoorDesableSprite;
    public GameObject GhostRomance;

    [Header("Configurações da Câmera")]
    public CinemachineCamera playerFollowVCam; // A VCam que segue o jogador
    public CinemachineCamera doorFeedbackVCam; // A VCam que mostra a porta
    public float cameraSwitchDuration = 2.0f; // Tempo que a câmera ficará na porta

    private bool isLeverActivated = false; // Para evitar ativações múltiplas
    private bool isPlayerInTrigger = false; // Para verificar se o jogador está na área do trigger
    private GameManager gameManager; // Referência ao GameManager
    private DarkRoomTrigger darkRoomTrigger;

    //------------Adicionado-------------
    // [Header("Ações do Spawner")]
    // [Tooltip("O spawner de fantasmas que esta alavanca vai controlar.")]
    // public GhostSpawner ghostSpawnerToActivate;

    // [Tooltip("Quantos fantasmas devem ser criados instantaneamente ao ativar a alavanca.")]
    // public int initialGhostsToSpawn = 3;
    void Start()
    {
        // Inicializa o GameManager
        gameManager = Object.FindFirstObjectByType<GameManager>();
        darkRoomTrigger = Object.FindFirstObjectByType<DarkRoomTrigger>();
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
            gameObject.GetComponent<SpriteRenderer>().sprite = LeverDesableSprite;
            ActivateLever();
            //gameObject.GetComponent<SpriteRenderer>().color = Color.green; //Placeholder para indicar que a alavanca foi ativada
            gameManager.InteractButtonImage.gameObject.SetActive(false); // Desabilita o botão de interação após ativar a alavanca
        }
    }

    void ActivateLever()
    {
        isLeverActivated = true;
        Debug.Log("Alavanca ativada!");

        //---TRECHO ADICIONADO PARA O SPAWNER DE FANTASMAS---
        // if (ghostSpawnerToActivate != null)
        // {
        //     //Manda criar os fantasmas iniciais imediatamente
        //     ghostSpawnerToActivate.SpawnImmediate(initialGhostsToSpawn);

        //     //Depois, inicia o spawn continuo
        //     ghostSpawnerToActivate.StartSpawning();
        // }

        // Inicia a rotina para abrir a porta e mudar a câmera
        StartCoroutine(OpenDoorAndSwitchCamera());
    }

    IEnumerator OpenDoorAndSwitchCamera()
    {
        // sistema de prioridade de câmeras serve para controlar qual câmera está ativa
        doorFeedbackVCam.Priority = 20; // Maior que a playerFollowVCam (10)
        playerFollowVCam.Priority = 10; // Mantém a prioridade normal do player follow, mas agora ela é menor

        darkRoomTrigger.globalLight.color = darkRoomTrigger.originalAmbientColor; //restaura a luz original
        // Espera um pequeno tempo para a câmera começar a transição antes de abrir a porta
        yield return new WaitForSeconds(0.2f);

        // Abrir a porta
        if (doorObjects != null && doorObjects.Length > 0)
        {

            // if (doorAnimator != null)
            // {
            //     doorAnimator.SetTrigger(doorOpenAnimationTrigger);
            // }
            yield return new WaitForSeconds(1);

            // Passa por cada porta na lista e a destrói
            foreach (GameObject door in doorObjects)
            {
                if (door != null) // Boa prática verificar se a porta não é nula
                {
                    //Destroy(door);
                    doorCollider.enabled = false; // Desativa o collider da porta
                    doorObjects[0].GetComponent<SpriteRenderer>().sprite = DoorDesableSprite; // Altera o sprite da porta para indicar que foi desativada
                    doorObjects[1].GetComponent<SpriteRenderer>().sprite = DoorDesableSprite;
                    GhostRomance.GetComponent<RomanceGhost_AI>().enabled = true; // Ativa o script do fantasma RomanceGhost_AI
                }
            }
        }
        // Espera o tempo de visualização da porta
        yield return new WaitForSeconds(cameraSwitchDuration);

        doorFeedbackVCam.Priority = 5; // Volta para prioridade menor
        playerFollowVCam.Priority = 10; // Garante que a playerFollowVCam volte a ser a principal
        darkRoomTrigger.globalLight.color = darkRoomTrigger.darkAmbientColor; //restaura a escuridão
    }
}