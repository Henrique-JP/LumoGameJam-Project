using UnityEngine;

[RequireComponent(typeof(PlayerMovement))] // Garante que temos acesso ao script de movimento
public class PlayerCapture : MonoBehaviour
{
    [Header("Configura��es da �rea de Captura")]
    public KeyCode captureKey = KeyCode.E;
    public LayerMask ghostLayer;

    // --- ALTERADO ---
    [Tooltip("O tamanho (largura e altura) da caixa de captura.")]
    public Vector2 captureBoxSize = new(3f, 2f);
    [Tooltip("A que dist�ncia do jogador a caixa de captura aparece.")]
    public float captureDistance = 1.5f;

    [Header("Refer�ncias Visuais")]
    public GameObject captureAreaVisualizer;

    private Transform _transform;
    private PlayerMovement playerMovement; // Refer�ncia para o script de movimento
    private GhostCapture currentGhostTarget;
    private Vector2 captureCenter;

    private void Awake()
    {
        _transform = transform;
        playerMovement = GetComponent<PlayerMovement>(); // Pega a refer�ncia

        if (captureAreaVisualizer != null)
        {
            // Ajusta o tamanho do visualizador para corresponder � caixa
            captureAreaVisualizer.transform.localScale = new Vector3(captureBoxSize.x, captureBoxSize.y, 1);
            captureAreaVisualizer.SetActive(false);
        }
    }

    private void Update()
    {
        // Calcula a posi��o da caixa de captura a cada frame
        captureCenter = (Vector2)_transform.position + playerMovement.LastMovementDirection * captureDistance;

        // Atualiza a posi��o do visualizador se ele estiver ativo
        if (captureAreaVisualizer != null && captureAreaVisualizer.activeSelf)
        {
            captureAreaVisualizer.transform.position = captureCenter;
        }

        if (Input.GetKeyDown(captureKey))
        {
            if (captureAreaVisualizer != null)
            {
                captureAreaVisualizer.transform.position = captureCenter; // Posi��o inicial
                captureAreaVisualizer.SetActive(true);
            }
            AttemptToStartCapture();
        }

        if (Input.GetKeyUp(captureKey))
        {
            if (captureAreaVisualizer != null)
                captureAreaVisualizer.SetActive(false);
            CancelCurrentCapture();
        }

        // Checagem de dist�ncia agora usa a nova posi��o da caixa
        if (currentGhostTarget != null)
        {
            float distanceToGhost = Vector2.Distance(captureCenter, currentGhostTarget.transform.position);
            // Uma checagem simples para ver se o fantasma saiu da �rea geral
            if (distanceToGhost > captureBoxSize.x)
            {
                CancelCurrentCapture();
            }
        }
    }

    private void AttemptToStartCapture()
    {
        if (currentGhostTarget != null) return;

        // --- ALTERADO ---
        // Usa OverlapBox em vez de OverlapCircle
        Collider2D hit = Physics2D.OverlapBox(captureCenter, captureBoxSize, 0f, ghostLayer);

        if (hit != null)
        {
            var ghost = hit.GetComponent<GhostCapture>();
            if (ghost != null && !ghost.IsBeingCaptured)
            {
                var romanceGhost = hit.GetComponent<RomanceGhost_AI>();
                if (romanceGhost != null && !romanceGhost.IsVulnerable)
                {
                    Debug.Log("Este fantasma est� se movendo r�pido demais para ser capturado!");
                    return;
                }

                currentGhostTarget = ghost;
                currentGhostTarget.StartCaptureProcess();
            }
        }
    }

    private void CancelCurrentCapture()
    {
        if (currentGhostTarget != null)
        {
            currentGhostTarget.CancelCaptureProcess();
            currentGhostTarget = null;
        }
    }

    // --- ALTERADO ---
    // Desenha uma caixa (WireCube) para debug no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Precisamos garantir que temos a refer�ncia do movimento no modo de edi��o
        PlayerMovement pm = GetComponent<PlayerMovement>();
        Vector2 lastDir = (pm != null && pm.LastMovementDirection != Vector2.zero) ? pm.LastMovementDirection : Vector2.right;
        Vector2 center = (Application.isPlaying) ? captureCenter : (Vector2)transform.position + lastDir * captureDistance;

        Gizmos.DrawWireCube(center, captureBoxSize);
    }
}