using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // Garante que o objeto tenha um SpriteRenderer
public class RomanceGhost_AI : GhostAI_Base
{
    [Header("Habilidade Unica: Ciclo Rapido/Atordoado")]
    [SerializeField] private float fastSpeedMultiplier = 2.5f;
    [SerializeField] private float fastDuration = 15f;
    [SerializeField] private float stunDuration = 10f;

    [Header("Sprites de Aparencia")]
    [Tooltip("O sprite normal do fantasma.")]
    public Sprite normalSprite;
    [Tooltip("O sprite para quando o fantasma estiver atordoado.")]
    public Sprite stunnedSprite;

    // --- Variaveis internas ---
    private SpriteRenderer spriteRenderer; // Referencia ao proprio SpriteRenderer
    private enum RomanceState { Fast, Stunned }
    private RomanceState romanceState = RomanceState.Fast;

    private float stateTimer;
    public bool IsVulnerable { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        // Pega a referencia ao componente SpriteRenderer no próprio objeto
        spriteRenderer = GetComponent<SpriteRenderer>();

        stateTimer = fastDuration;
        IsVulnerable = false;

        // Garante que ele comece com o sprite normal
        if (normalSprite != null)
            spriteRenderer.sprite = normalSprite;
    }

    protected override void HandleStateBehavior()
    {
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            SwitchState();
        }
        ExecuteMovement();
    }

    private void SwitchState()
    {
        if (romanceState == RomanceState.Fast)
        {
            // Entra no estado de Stun
            romanceState = RomanceState.Stunned;
            stateTimer = stunDuration;
            IsVulnerable = true;

            // TROCA para o sprite de stun
            if (stunnedSprite != null)
                spriteRenderer.sprite = stunnedSprite;
        }
        else // Estava em Stunned
        {
            // Entra no estado Rápido
            romanceState = RomanceState.Fast;
            stateTimer = fastDuration;
            IsVulnerable = false;

            // VOLTA para o sprite normal
            if (normalSprite != null)
                spriteRenderer.sprite = normalSprite;
        }
    }

    private void ExecuteMovement()
    {
        if (romanceState == RomanceState.Fast && playerTransform != null)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = fastSpeedMultiplier * moveSpeed * -directionToPlayer;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}