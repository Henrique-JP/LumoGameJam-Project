using UnityEngine;

public class FantasyGhost_AI : GhostAI_Base
{
    [Header("Habilidade Única: Teleporte")]
    [SerializeField] private float teleportInterval = 30f;
    [SerializeField] private Vector2 teleportAreaCenter = Vector2.zero;
    [SerializeField] private Vector2 teleportAreaSize = new (40f, 20f);

    private float teleportTimer;

    protected override void Awake()
    {
        base.Awake(); // Chama o Awake da base
        teleportTimer = teleportInterval;
    }

    // Sobrescrevemos o comportamento para ADICIONAR a lógica de teleporte
    protected override void HandleStateBehavior()
    {
        // 1. EXECUTA O COMPORTAMENTO PADRÃO (movimento aleatório) DA CLASSE BASE
        base.HandleStateBehavior();

        // 2. ADICIONA A HABILIDADE ÚNICA
        teleportTimer -= Time.deltaTime;
        if (teleportTimer <= 0)
        {
            Teleport();
            teleportTimer = teleportInterval;
        }
    }

    private void Teleport()
    {
        float randomX = Random.Range(teleportAreaCenter.x - teleportAreaSize.x / 2, teleportAreaCenter.x + teleportAreaSize.x / 2);
        float randomY = Random.Range(teleportAreaCenter.y - teleportAreaSize.y / 2, teleportAreaCenter.y + teleportAreaSize.y / 2);
        transform.position = new Vector2(randomX, randomY);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0f, 1f, 0.5f);
        Gizmos.DrawCube(teleportAreaCenter, teleportAreaSize);
    }
}