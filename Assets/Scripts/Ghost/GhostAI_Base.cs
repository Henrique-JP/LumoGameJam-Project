using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(GhostCapture))]
public abstract class GhostAI_Base : MonoBehaviour
{
    [Header("Configurações Gerais de IA")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float fleeDistance = 7f;

    [Header("Configurações de Movimento Padrão")]
    [Tooltip("De quanto em quanto tempo o fantasma muda de direção por padrão.")]
    [SerializeField] private float defaultDirectionChangeInterval = 3f;

    // --- Variáveis internas ---
    protected Transform playerTransform;
    protected Rigidbody2D rb;
    protected GhostCapture ghostCapture;
    private float directionTimer;
    private Vector2 randomDirection;

    protected enum GhostState { Patrulha, Fuga, Saindo }
    private Transform targetExitPoint;
    protected GhostState currentState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ghostCapture = GetComponent<GhostCapture>();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("IA de Fantasma: Objeto do Jogador com a tag 'Player' nao encontrado!", this);
        }
    }
    protected virtual void Update()
    {
        if (ghostCapture != null && ghostCapture.IsBeingCaptured)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentState != GhostState.Saindo && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            currentState = (distanceToPlayer < fleeDistance) ? GhostState.Fuga : GhostState.Patrulha;
        }

        HandleStateBehavior();
    }
    // Agora este método contém a lógica de movimento padrão.
    protected virtual void HandleStateBehavior()
    {
        switch (currentState)
        {
            case GhostState.Saindo:
                HandleExitState();
                break;

            case GhostState.Fuga:
                if (playerTransform != null)
                {
                    Vector2 directionFromPlayer = (transform.position - playerTransform.position).normalized;
                    rb.linearVelocity = 1.5f * moveSpeed * directionFromPlayer;
                }
                break;

            case GhostState.Patrulha:
            default:
                directionTimer -= Time.deltaTime;
                if (directionTimer <= 0)
                {
                    randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                    directionTimer = defaultDirectionChangeInterval;
                }
                rb.linearVelocity = randomDirection * moveSpeed;
                break;
        }
    }

    // ESTE É O NOVO MÉTODO QUE CONTROLA A SAÍDA DA SALA
    private void HandleExitState()
    {
        if (targetExitPoint == null)
        {
            // Se, por algum motivo, não há ponto de saída, volta a patrulhar.
            currentState = GhostState.Patrulha;
            return;
        }

        // Move-se em direção ao ponto de saída
        Vector2 directionToExit = (targetExitPoint.position - transform.position).normalized;
        rb.linearVelocity = 1.2f * moveSpeed * directionToExit; // Um pouco mais rápido pra sair logo

        // Se chegou perto o suficiente do ponto de saída
        if (Vector2.Distance(transform.position, targetExitPoint.position) < 1.0f)
        {
            Debug.Log("Fantasma chegou ao ponto de saída. Iniciando IA normal.");
            targetExitPoint = null; // Esquece o ponto de saída
            currentState = GhostState.Patrulha; // Começa a patrulhar
        }
    }
    public void SetInitialTarget(Transform exitPoint)
    {
        this.targetExitPoint = exitPoint;
        this.currentState = GhostState.Saindo;
        Debug.Log($"Fantasma {gameObject.name} recebeu ordem para ir para o ponto de saída: {exitPoint.name}");
    }
}