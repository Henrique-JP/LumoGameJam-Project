using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class TypeTextEffect : MonoBehaviour
{
    // A velocidade em que as letras aparecerão (quanto menor, mais rápido)
    [Tooltip("A velocidade em segundos entre a exibição de cada letra. Quanto menor, mais rápido.")]
    [Range(0.01f, 1f)]
    public float typingSpeed = 0.05f;

    [TextArea(3, 10)] // Permite múltiplas linhas no inspetor
    public string fullText;
    private Text textComponent;

    private string currentDisplayedText = "";
    private Coroutine typingCoroutine;

    void Awake()
    {
        textComponent = GetComponent<Text>();
        if (textComponent == null)
        {
            Debug.LogError("Componente TextMeshProUGUI não encontrado!");
            enabled = false; // Desativa o script se não houver componente de texto
            return;
        }

        // Limpa o texto inicialmente
        textComponent.text = "";
    }

    void Start()
    {
        StartCoroutine(CoolDown(1f)); // Inicia a coroutine de cooldown para esperar antes de começar o efeito
    }

    void OnEnable()
    {
        // Inicia o efeito de digitação quando o objeto é ativado
        StartTypingEffect();
    }

    void OnDisable()
    {
        // Para a coroutine se o objeto for desativado
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
    }

    // Método público para iniciar o efeito de digitação
    public void StartTypingEffect()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine); // Para qualquer coroutine anterior
        }
        currentDisplayedText = "";
        textComponent.text = ""; // Garante que o texto esteja vazio antes de começar
        typingCoroutine = StartCoroutine(TypeWriterCoroutine());
    }

    // Coroutine que gerencia a exibição letra por letra
    IEnumerator TypeWriterCoroutine()
    {
        foreach (char letter in fullText.ToCharArray())
        {
            currentDisplayedText += letter;
            textComponent.text = currentDisplayedText;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    // Método para definir um novo texto e iniciar o efeito
    public void SetTextAndStartTyping(string newText)
    {
        fullText = newText;
        StartTypingEffect();
    }

    IEnumerator CoolDown(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        // Aqui você pode adicionar lógica para o que deve acontecer após o tempo de espera
        StartTypingEffect();
    }
}