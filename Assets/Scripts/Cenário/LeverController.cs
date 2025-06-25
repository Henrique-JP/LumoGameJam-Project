using UnityEngine;
using System.Collections;
using Unity.Cinemachine; 

public class LeverController : MonoBehaviour
{
    [Header("Configurações da Porta")]
    public GameObject doorObject; // O objeto da porta que será aberta
    public Animator doorAnimator; // O Animator da porta (se houver)
    public string doorOpenAnimationTrigger = "OpenDoor"; // Nome do trigger no Animator

    [Header("Configurações da Câmera")]
    public CinemachineCamera playerFollowVCam; // A VCam que segue o jogador
    public CinemachineCamera doorFeedbackVCam; // A VCam que mostra a porta
    public float cameraSwitchDuration = 2.0f; // Tempo que a câmera ficará na porta

    private bool isLeverActivated = false; // Para evitar ativações múltiplas

    void OnTriggerEnter2D(Collider2D other)
    {
        // reage apenas se o objeto que entrou no trigger for o jogador
        if (other.CompareTag("Player") && !isLeverActivated) 
        {
            // Opcional: Aqui você pode adicionar lógica para o jogador interagir com a alavanca
            // Por exemplo, checar um input key: if (Input.GetKeyDown(KeyCode.E)) { ActivateLever(); }
            // Por simplicidade, ativaremos ao entrar no trigger.
            gameObject.GetComponent<SpriteRenderer>().color = Color.green; // Muda a cor da alavanca para indicar que foi ativada
            ActivateLever();
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
        // 1. Ativar a VCam da porta (aumenta a prioridade)
        doorFeedbackVCam.Priority = 20; // Maior que a playerFollowVCam (10)
        playerFollowVCam.Priority = 10; // Mantém a prioridade normal do player follow, mas agora ela é menor

        // Espera um pequeno tempo para a câmera começar a transição antes de abrir a porta
        // Isso dá tempo para o Cinemachine Brain começar a agir
        yield return new WaitForSeconds(0.2f); // Pequeno atraso

        // 2. Abrir a porta
        if (doorObject != null)
        {
            // Se você tiver um Animator, ative o trigger de animação
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger(doorOpenAnimationTrigger);
            }
            // Se não tiver Animator, você pode simplesmente desativar o objeto da porta
            // ou mudar o sprite para a porta aberta.
            // else { doorObject.SetActive(false); }
            yield return new WaitForSeconds(1);
            Destroy(doorObject); // Exemplo: destrói a porta após abrir
        }

        // 3. Esperar o tempo de visualização da porta
        yield return new WaitForSeconds(cameraSwitchDuration);

        // 4. Voltar para a VCam do jogador (diminui a prioridade da VCam da porta)
        doorFeedbackVCam.Priority = 5; // Volta para prioridade menor
        playerFollowVCam.Priority = 10; // Garante que a playerFollowVCam volte a ser a principal
    }
}