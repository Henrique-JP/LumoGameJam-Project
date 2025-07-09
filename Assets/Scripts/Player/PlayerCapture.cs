using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerCapture : MonoBehaviour
{
    [Header("Teclas de Ação")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode captureKey = KeyCode.Space;
    public KeyCode readHintKey = KeyCode.F;

    [Header("Variaveis da UI")]
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;
    public Image bookIconImage;

    [Header("Configuracoes do livro")]
    public Transform bookHolder;

    [Header("Configurações da Área de Captura")]
    public LayerMask ghostLayer;
    public Vector2 captureBoxSize = new(3f, 2f);
    public float captureDistance = 1.5f;

    [Header("Referências Visuais")]
    public GameObject captureAreaVisualizer;

    private Transform _transform;
    private PlayerMovement playerMovement;
    private GhostCapture currentGhostTarget;
    private BookPickup equippedBook;
    private BookPickup availableBook;


    private void Awake()
    {
        _transform = transform;
        playerMovement = GetComponent<PlayerMovement>();

        if (captureAreaVisualizer != null)
        {
            captureAreaVisualizer.transform.localScale = new Vector3(captureBoxSize.x, captureBoxSize.y, 1);
            captureAreaVisualizer.SetActive(false);
        }

        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("!!!! A variável 'hintPanel' NÃO foi atribuída no Inspector! !!!!", this.gameObject);
        }

        if (bookIconImage != null)
        {
            bookIconImage.enabled = false;
        }

        if (GameManager.Instance != null && GameManager.Instance.InteractButtonImage != null)
            GameManager.Instance.InteractButtonImage.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleInteractionInput();
        HandleCaptureInput();
        HandleHintInput();
        CheckGhostCaptureStatus();
    }

    private void HandleHintInput()
    {
        if (Input.GetKeyDown(readHintKey))
        {
            Debug.Log("<color=cyan>-- PISTA 1: Tecla 'F' foi pressionada. --</color>");
            ToggleHintPanel();
        }
    }

    private void ToggleHintPanel()
    {
        Debug.Log("<color=cyan>-- PISTA 2: Entrou na função ToggleHintPanel. --</color>");

        if (hintPanel == null)
        {
            Debug.LogError("!!!! A função falhou porque a variável 'hintPanel' é NULA. Verifique o Inspector. !!!!");
            return;
        }

        if (hintPanel.activeSelf)
        {
            Debug.Log("<color=orange>Painel já está ativo, então será escondido.</color>");
            hintPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Painel está inativo. Verificando se o jogador tem um livro...");
            if (equippedBook != null)
            {
                Debug.Log("<color=green>-- SUCESSO: Jogador TEM um livro equipado. Mostrando o painel. --</color>");
                ShowHintPanel();
            }
            else
            {
                Debug.Log("<color=red>-- FALHA: Jogador NÃO TEM um livro equipado. Nada a fazer. --</color>");
            }
        }
    }

    private void PickupAndSwapBook()
    {
        if (availableBook == null) return;

        Debug.Log("<color=yellow>-- PISTA EXTRA: Tentando pegar o livro '" + availableBook.name + "' --</color>");

        Vector3 oldBookPosition = availableBook.transform.position;
        Quaternion oldBookRotation = availableBook.transform.rotation;
        if (equippedBook != null)
        {
            equippedBook.transform.SetParent(null);
            equippedBook.transform.position = oldBookPosition;
            equippedBook.transform.rotation = oldBookRotation;
            equippedBook.gameObject.SetActive(true);
        }
        equippedBook = availableBook;
        equippedBook.transform.SetParent(bookHolder);
        equippedBook.transform.localPosition = Vector3.zero;
        equippedBook.transform.localRotation = Quaternion.identity;
        equippedBook.gameObject.SetActive(false);
        availableBook = null;

        UpdateBookIcon();

        Debug.Log("<color=yellow>-- PISTA EXTRA: Livro pego com sucesso! 'equippedBook' agora é: " + equippedBook.name + " --</color>");

        if (GameManager.Instance != null && GameManager.Instance.InteractButtonImage != null)
            GameManager.Instance.InteractButtonImage.gameObject.SetActive(false);
    }

    private void UpdateBookIcon()
    {
        if (bookIconImage == null) return;

        if (equippedBook != null && equippedBook.bookIcon != null)
        {
            bookIconImage.sprite = equippedBook.bookIcon;
            bookIconImage.enabled = true;
        }
        else
        {
            bookIconImage.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<BookPickup>(out var book))
        {
            Debug.Log("<color=lime>-- PISTA EXTRA: Jogador entrou na área de um livro: " + other.name + " --</color>");
            availableBook = book;
            if (GameManager.Instance != null && GameManager.Instance.InteractButtonImage != null)
                GameManager.Instance.InteractButtonImage.gameObject.SetActive(true);
        }
    }
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && availableBook != null)
        {
            PickupAndSwapBook();
        }
    }
    private void HandleCaptureInput()
    {
        if (Input.GetKey(captureKey) && equippedBook != null)
        {
            ActivateCaptureArea();
            AttemptToStartCapture();
        }
        if (Input.GetKeyUp(captureKey))
        {
            DeactivateCaptureArea();
            CancelCurrentCapture();
        }
    }
    private void CheckGhostCaptureStatus() { if (currentGhostTarget != null && currentGhostTarget.IsBeingCaptured) { Vector2 captureCenter = GetCaptureCenter(); Collider2D[] results = Physics2D.OverlapBoxAll(captureCenter, captureBoxSize, 0f, ghostLayer); bool ghostStillInArea = false; foreach (var hit in results) { if (hit.GetComponent<GhostCapture>() == currentGhostTarget) { ghostStillInArea = true; break; } } if (!ghostStillInArea) { CancelCurrentCapture(); } } }
    private Vector2 GetCaptureCenter() { Vector2 lastDir = (playerMovement.LastMovementDirection.sqrMagnitude > 0.01f) ? playerMovement.LastMovementDirection : (Vector2)_transform.right; return (Vector2)_transform.position + lastDir.normalized * captureDistance; }
    private void ActivateCaptureArea() { if (captureAreaVisualizer != null) { captureAreaVisualizer.transform.position = GetCaptureCenter(); captureAreaVisualizer.SetActive(true); } }
    private void DeactivateCaptureArea() { if (captureAreaVisualizer != null) { captureAreaVisualizer.SetActive(false); } }
    private void AttemptToStartCapture() { if (currentGhostTarget != null) return; Collider2D[] hits = Physics2D.OverlapBoxAll(GetCaptureCenter(), captureBoxSize, 0f, ghostLayer); foreach (var hit in hits) { var ghost = hit.GetComponent<GhostCapture>(); if (ghost != null && !ghost.IsBeingCaptured) { if (equippedBook != null && ghost.ghostGenre == this.equippedBook.bookGenre) { currentGhostTarget = ghost; currentGhostTarget.StartCaptureProcess(); break; } } } }
    private void CancelCurrentCapture() { if (currentGhostTarget != null) { currentGhostTarget.CancelCaptureProcess(); currentGhostTarget = null; } DeactivateCaptureArea(); }
    private void ShowHintPanel() { if (equippedBook != null && hintPanel != null && hintText != null) { hintText.text = equippedBook.bookHint; hintPanel.SetActive(true); } }
    private void OnTriggerExit2D(Collider2D other) { if (other.TryGetComponent<BookPickup>(out var book) && availableBook == book) { availableBook = null; if (GameManager.Instance != null && GameManager.Instance.InteractButtonImage != null) GameManager.Instance.InteractButtonImage.gameObject.SetActive(false); } }
    private void OnDrawGizmosSelected() { Gizmos.color = Color.yellow; Vector2 center = Application.isPlaying ? GetCaptureCenter() : (Vector2)transform.position + Vector2.right * captureDistance; Gizmos.DrawWireCube(center, captureBoxSize); }
}