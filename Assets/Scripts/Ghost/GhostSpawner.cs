using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [Header("Configura��es do Fantasma")]
    [Tooltip("O Prefab do fantasma que este spawner ir� criar.")]
    [SerializeField] private GameObject ghostPrefab;

    [Header("Controle de Spawn")]
    [Tooltip("Marque para o spawner come�ar a funcionar automaticamente.")]
    [SerializeField] private bool startAutomatically = true;

    [Tooltip("O intervalo em segundos entre cada tentativa de spawn.")]
    [SerializeField] private float spawnInterval = 10f;

    [Header("1. Limite de Ativos na Cena")]
    [Tooltip("O n�mero m�ximo de fantasmas que podem existir na cena ao mesmo tempo. Use -1 para n�o ter limite de ativos.")]
    [SerializeField] private int maxActiveGhosts = 5;

    [Header("2. Limite Total de Spawns")]
    [Tooltip("O n�mero TOTAL de fantasmas que o spawner pode criar. Use -1 para spawns infinitos.")]
    [SerializeField] private int totalSpawnLimit = 20;

    [Header("Comportamento P�s-Spawn")]
    [Tooltip("Opcional: Ponto para onde os fantasmas ir�o imediatamente ap�s serem criados.")]
    [SerializeField] private Transform initialTargetPoint;

    private int spawnedCount = 0; // Conta quantos j� foram criados no total
    private bool isSpawning = false;

    void Start()
    {
        if (ghostPrefab == null)
        {
            Debug.LogError("O Prefab do fantasma n�o foi atribu�do no GhostSpawner!", this);
            return;
        }

        if (startAutomatically)
        {
            StartSpawning();
        }
    }

    public void StartSpawning()
    {
        if (isSpawning) return;
        if (totalSpawnLimit != -1 && spawnedCount >= totalSpawnLimit) return;

        isSpawning = true;
        InvokeRepeating(nameof(TrySpawnGhost), 1f, spawnInterval);
    }

    public void StopSpawning()
    {
        if (!isSpawning) return;
        isSpawning = false;
        CancelInvoke(nameof(TrySpawnGhost));
    }

    private void TrySpawnGhost()
    {
        // --- L�GICA COM DUPLA VERIFICA��O ---

        // 1. Verifica o limite TOTAL. Se j� atingiu, desativa o spawner para sempre.
        bool totalLimitReached = (totalSpawnLimit != -1 && spawnedCount >= totalSpawnLimit);
        if (totalLimitReached)
        {
            Debug.Log($"Limite total de {totalSpawnLimit} fantasmas atingido. Desativando spawner permanentemente.");
            StopSpawning();
            this.enabled = false; // Desativa o componente do script
            return; // Sai da fun��o
        }

        // 2. Verifica o limite de ATIVOS. Se a cena estiver cheia, apenas espera a pr�xima verifica��o.
        bool hasActiveLimit = (maxActiveGhosts != -1);
        if (hasActiveLimit)
        {
            int currentActiveGhosts = FindObjectsByType<GhostAI_Base>(FindObjectsSortMode.None).Length;
            if (currentActiveGhosts >= maxActiveGhosts)
            {
                // Cena est� cheia, n�o faz nada e espera o pr�ximo intervalo.
                return; // Sai da fun��o
            }
        }

        // Se passou por todas as verifica��es, pode criar o fantasma.
        SpawnGhost();
    }

    private void SpawnGhost()
    {
        float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
        float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
        Vector2 spawnPosition = new (randomX, randomY);

        // --- LINHA CORRIGIDA ---
        // Agora salvamos a refer�ncia do fantasma criado na vari�vel 'newGhostObject'.
        GameObject newGhostObject = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);

        spawnedCount++;

        // Agora o resto do c�digo funciona, pois a vari�vel 'newGhostObject' existe.
        GhostAI_Base ghostAI = newGhostObject.GetComponent<GhostAI_Base>();

        if (ghostAI != null && initialTargetPoint != null)
        {
            // ghostAI.SetInitialTarget(initialTargetPoint);
        }
    }

    [Header("�rea de Spawn")]
    [SerializeField] private Vector2 spawnAreaCenter = Vector2.zero;
    [SerializeField] private Vector2 spawnAreaSize = new(50f, 25f);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
        Gizmos.DrawCube(spawnAreaCenter, spawnAreaSize);
    }

    public void SpawnImmediate(int amount)
    {
        Debug.Log($"Spawnando {amount} de fantasmas");
        for (int i = 0; i < amount; i++)
        {
            bool totalLimitReached = (totalSpawnLimit != -1 && spawnedCount >= totalSpawnLimit);
            if(totalLimitReached)
            {
                Debug.LogWarning("Limite total de fantasmas atingido");
                break;
            }
            SpawnGhost();
        }
    }
}