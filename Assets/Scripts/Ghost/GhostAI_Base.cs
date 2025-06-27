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

    protected enum GhostState { Patrulha, Fuga }
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

        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            currentState = (distanceToPlayer < fleeDistance) ? GhostState.Fuga : GhostState.Patrulha;
        }

        HandleStateBehavior();
    }

    // MUDANÇA PRINCIPAL: De "abstract" para "virtual"
    // Agora este método contém a lógica de movimento padrão.
    protected virtual void HandleStateBehavior()
    {
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0)
        {
            randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            directionTimer = defaultDirectionChangeInterval;
        }

        float currentSpeed = (currentState == GhostState.Fuga) ? moveSpeed * 1.5f : moveSpeed;
        rb.linearVelocity = randomDirection * currentSpeed;
    }
}