using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DarkRoomTrigger : MonoBehaviour
{
    [Header("Configurações de Luz")]
    public Light2D globalLight; // Luz global 
    public GameObject playerLightObject; // Objeto de luz do jogador (PlayerLight)
    public Color darkAmbientColor = Color.black; // Cor ambiente para a sala escura

    private Color originalAmbientColor; // Para restaurar a cor ambiente original

    void Start()
    {
        // Desliga a luz do jogador no início 
        if (playerLightObject != null)
        {
            playerLightObject.SetActive(false);
        }

        // Salva a cor ambiente original ao iniciar a cena
        originalAmbientColor = globalLight.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Certifica-se de que é o jogador que entrou no gatilho
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador entrou na sala escura.");

            // Ativa a luz do jogador
            if (playerLightObject != null)
            {
                playerLightObject.SetActive(true);
            }

            // Escurece o ambiente global
            globalLight.color = darkAmbientColor;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Certifica que o jogador que saiu do gatilho
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador saiu da sala escura.");

            // Desativa a luz do jogador
            if (playerLightObject != null)
            {
                playerLightObject.SetActive(false);
            }

            // Restaura o ambiente global
            globalLight.color = originalAmbientColor;
        }
    }
}