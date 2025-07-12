using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RomanceGhost_AI : GhostAI_Base
{
    [Header("Habilidade Unica: Ciclo Rapido/Atordoado")]
    [SerializeField] private float fastSpeedMultiplier = 2.5f;
    [SerializeField] private float fastDuration = 15f;
    [SerializeField] private float stunDuration = 10f;

    [Header("Sprites de Aparencia")]
    [Tooltip("O sprite para quando o fantasma estiver atordoado.")]
    public Sprite stunnedSprite;

    private SpriteRenderer spriteRenderer;
    // <<< ADICIONADO o estado 'Dormant' >>>
    private enum RomanceState { Dormant, Exiting, Fast, Stunned }
    private RomanceState romanceState;
    private Transform exitWaypoint;

    private float stateTimer;
    public bool IsVulnerable { get; private set; }

    // <<< ALTERADO: Agora ele começa no estado 'Dormant' >>>
    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
        romanceState = RomanceState.Dormant; // Começa inativo
        IsVulnerable = false; // Não é vulnerável enquanto dorme
    }

    public void StartExitSequence(Transform exitPoint)
    {
        // Só executa se estiver inativo, para evitar ser chamado múltiplas vezes
        if (romanceState == RomanceState.Dormant)
        {
            Debug.Log("RomanceGhost_AI: Recebeu ordem de saída! Acordando...");
            exitWaypoint = exitPoint;
            romanceState = RomanceState.Exiting;
        }
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = Vector2.zero;

        // <<< LÓGICA DO ESTADO 'Dormant' >>>
        if (romanceState == RomanceState.Dormant)
        {
            targetVelocity = Vector2.zero; // Garante que ele fique parado
        }
        else if (romanceState == RomanceState.Exiting)
        {
            if (exitWaypoint != null)
            {
                Vector2 directionToExit = ((Vector2)exitWaypoint.position - (Vector2)transform.position).normalized;
                targetVelocity = directionToExit * (moveSpeed * fastSpeedMultiplier);

                if (Vector2.Distance(transform.position, exitWaypoint.position) < 0.5f)
                {
                    Debug.Log("RomanceGhost_AI: Saída concluída. Iniciando ciclo de patrulha.");
                    romanceState = RomanceState.Fast;
                    stateTimer = fastDuration;
                    ChooseRandomWaypoint();
                }
            }
        }
        else if (romanceState == RomanceState.Fast)
        {
            if (waypoints != null && waypoints.Count > 0)
            {
                Vector2 directionToWaypoint = (waypoints[currentWaypointIndex].position - transform.position).normalized;
                targetVelocity = directionToWaypoint * (moveSpeed * fastSpeedMultiplier);
            }
        }
        else // romanceState == RomanceState.Stunned
        {
            targetVelocity = Vector2.zero;
        }

        rb.linearVelocity = AvoidObstacles(targetVelocity);
    }

    protected override void HandleStateBehavior()
    {
        // <<< ALTERADO: Não faz nada se estiver inativo >>>
        if (romanceState == RomanceState.Dormant || romanceState == RomanceState.Exiting) return;

        if (romanceState == RomanceState.Fast)
        {
            if (waypoints != null && waypoints.Count > 0 && Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.5f)
            {
                ChooseRandomWaypoint();
            }
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SwitchState();
        }
    }

    // O resto do script permanece igual...
    protected override void CheckPlayerDistance() { }
    private void SwitchState()
    {
        if (romanceState == RomanceState.Fast)
        {
            romanceState = RomanceState.Stunned;
            stateTimer = stunDuration;
            IsVulnerable = true;
            Debug.Log("RomanceGhost_AI: Entrando no estado ATORDOADO.");
        }
        else
        {
            romanceState = RomanceState.Fast;
            stateTimer = fastDuration;
            IsVulnerable = false;
            Debug.Log("RomanceGhost_AI: Entrando no estado RÁPIDO.");
            ChooseRandomWaypoint();
        }
    }
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (romanceState == RomanceState.Stunned)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}