using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // Garante que o objeto tenha um SpriteRenderer
public class RomanceGhost_AI : GhostAI_Base
{
    [Header("Habilidade Unica: Ciclo Rapido/Atordoado")]
    [SerializeField] private float fastSpeedMultiplier = 2.5f;
    [SerializeField] private float fastDuration = 15f;
    [SerializeField] private float stunDuration = 10f;

    [Header("Sprites de Aparencia")]
    // [Tooltip("O sprite normal do fantasma.")]
    // public Sprite normalSprite;
    [Tooltip("O sprite para quando o fantasma estiver atordoado.")]
    public Sprite stunnedSprite;

    // --- Variaveis internas ---
    private SpriteRenderer spriteRenderer; // Referencia ao proprio SpriteRenderer
    private enum RomanceState { Fast, Stunned }
    private RomanceState romanceState = RomanceState.Fast;

    private float stateTimer;
    public bool IsVulnerable { get; private set; } // Propriedade para ser acessada externamente

    protected override void Awake()
    {
        base.Awake(); // Chama o Awake da base para inicializar rb, playerTransform, etc.
        
        // Pega a referencia ao componente SpriteRenderer no próprio objeto
        spriteRenderer = GetComponent<SpriteRenderer>();

        stateTimer = fastDuration; // Inicia no estado rápido
        IsVulnerable = false; // Começa invulnerável

        // Garante que ele comece com o sprite normal
        // if (normalSprite != null)
        //     spriteRenderer.sprite = normalSprite;
    }

    void FixedUpdate()
    {
        // A lógica de evasão de obstáculos ainda pode ser útil, mesmo para perseguir
        // ou ficar parado. Se o fantasma se move, ele deve evitar paredes.
        // Se ele não se move, a evasão não terá efeito.
        
        Vector2 targetVelocity = Vector2.zero;

        if (romanceState == RomanceState.Fast)
        {
            // Para perseguir: ((Vector2)playerTransform.position - transform.position).normalized
            // Para fugir (como o original): -((Vector2)playerTransform.position - transform.position).normalized
            if (playerTransform != null)
            {
                Vector2 directionToPlayer = -((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
                targetVelocity = directionToPlayer * (moveSpeed * fastSpeedMultiplier);
            }
        }
        else // romanceState == RomanceState.Stunned
        {
            // No estado "Stunned", o fantasma fica parado.
            targetVelocity = Vector2.zero;
        }

        // Aplica a evasão de obstáculos, se houver movimento
        rb.linearVelocity = AvoidObstacles(targetVelocity);
    }

    protected override void HandleStateBehavior()
    {
        // Sem base.HandleStateBehavior() aqui porque
        // a lógica de Patrulha/Fuga da base não se aplica ao RomanceGhost_AI,
        // que tem seus próprios estados de Fast/Stunned.
        
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SwitchState();
        }
    }

    protected override void CheckPlayerDistance()
    {
        // O RomanceGhost_AI não transita entre Patrol e Evade com base na proximidade do jogador.
        // Sua transição de estado é puramente baseada no timer de Fast/Stunned.
        // Portanto, a lógica de CheckPlayerDistance da base é ignorada.
    }


    private void SwitchState()
    {
        if (romanceState == RomanceState.Fast)
        {
            // Entra no estado de Stunned
            romanceState = RomanceState.Stunned;
            stateTimer = stunDuration;
            IsVulnerable = true;
            Debug.Log("RomanceGhost_AI: Entrando no estado ATORDOADO.");

            // TROCA para o sprite de stun
            if (stunnedSprite != null)
            {
                // precisa ser feito de outra forma, pois os sprites de movimento no update sobrescrevem o sprite do atordoado
                // spriteRenderer.sprite = stunnedSprite;
            }
                
        }
        else // Estava em Stunned
        {
            // Entra no estado Rápido
            romanceState = RomanceState.Fast;
            stateTimer = fastDuration;
            IsVulnerable = false;
            Debug.Log("RomanceGhost_AI: Entrando no estado RÁPIDO.");

            // VOLTA para o sprite normal
            // if (normalSprite != null)
            //     spriteRenderer.sprite = normalSprite;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected(); // Desenha as gizmos de detecção do jogador e waypoint da base

        // Adicionar gizmos específicas do RomanceGhost_AI aqui, se houver necessidade
        // Por exemplo, para visualizar algo relacionado ao seu estado ou habilidades.
        if (romanceState == RomanceState.Stunned)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f); // Exemplo: um círculo vermelho para indicar atordoado
        }
    }
}