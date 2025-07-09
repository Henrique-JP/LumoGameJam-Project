using UnityEngine;
using System.Collections; // Necessário para usar Coroutines
using System.Collections.Generic;
using System.Linq; // Para usar o .OrderBy e .ThenBy

public class GhostAI_Base : MonoBehaviour
{
    // --- Configurações Básicas ---
    [Header("Configurações do Fantasma Base")]
    public float moveSpeed = 3f; // Velocidade de movimento do fantasma
    public float evadeSpeedMultiplier = 1.5f; // Multiplicador de velocidade ao fugir
    public float detectionRadius = 5f; // Raio para detectar o jogador
    public LayerMask playerLayer; // Layer do jogador para detecção

    // --- Waypoints ---
    [Header("Waypoints")]
    private Coroutine patrolWaitCoroutine; // Variavel para armazenar a Coroutine
    public List<Transform> waypoints; // Lista de waypoints na cena
    protected int currentWaypointIndex; // Alterado para protected
    [Tooltip("Tempo de atraso em segundos ao chegar em um waypoint antes de escolher o próximo.")]
    public float waypointDecisionDelay = 0.5f; // Tempo de atraso na decisão do waypoint
    private bool isWaitingAtWaypoint = false; // Flag para indicar se está esperando


    // --- Referências ---
    protected Transform playerTransform; // Alterado para protected
    protected Rigidbody2D rb; // Alterado para protected
    public Animator anim; // Referência ao Animator, se houver
    public SoundManager soundManager; // Referência ao SoundManager, se houver
    private SpriteRenderer spriteRenderer; // Adicionado para controle do flipX

    // --- Máquina de Estados ---
    public enum GhostState { Patrol, Evade }
    public GhostState currentState = GhostState.Patrol;

    // --- Evasão de Obstáculos ---
    [Header("Evasão de Obstáculos")]
    public float obstacleDetectionDistance = 1.5f; // Distância para detectar obstáculos
    public LayerMask obstacleLayer; // Layer dos obstáculos (paredes, objetos do cenário)

    // Awake agora é protected virtual para ser chamado pela classe filha
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Inicializa o Animator
        spriteRenderer = GetComponent<SpriteRenderer>(); // Inicializa o SpriteRenderer
        soundManager = Object.FindAnyObjectByType<SoundManager>(); // Encontra o SoundManager na cena

        
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;// Encontra o jogador pela tag
        } 

        if (playerTransform == null)
        {
            Debug.LogError("Jogador não encontrado! Certifique-se de que o jogador tem a tag 'Player'.");
        }

        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogError("Nenhum waypoint atribuído. Atribua os waypoints no Inspector.");
        }
        else
        {
            ChooseRandomWaypoint(); // Começa em um waypoint aleatório
        }
    }

    void Update()
    {
        // Verifica a distância do jogador a cada frame
        CheckPlayerDistance();

        // Novo método virtual para encapsular o comportamento do estado
        HandleStateBehavior();
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = Vector2.zero;

        // Só se move se não estiver esperando no waypoint
        if (currentState == GhostState.Patrol && !isWaitingAtWaypoint)
        {
            Vector2 directionToWaypoint = (waypoints[currentWaypointIndex].position - transform.position).normalized;
            targetVelocity = directionToWaypoint * moveSpeed;
        }
        else if (currentState == GhostState.Evade)
        {
            Vector2 directionFromPlayer = (transform.position - playerTransform.position).normalized;
            targetVelocity = directionFromPlayer * (moveSpeed * evadeSpeedMultiplier);
        }
        // Se estiver esperando ou em Patrulha mas esperando, a velocidade é zero
        else if (currentState == GhostState.Patrol && isWaitingAtWaypoint)
        {
            targetVelocity = Vector2.zero;
        }

        targetVelocity = AvoidObstacles(targetVelocity);
        rb.linearVelocity = targetVelocity; 

        //Controle das animações - mover para uma função separada para clareza e reusabilidade
        UpdateAnimations(targetVelocity);
    }

    protected virtual void HandleStateBehavior()
    {
        switch (currentState)
        {
            case GhostState.Patrol:
                PatrolBehavior();
                break;
            case GhostState.Evade:
                EvadeBehavior();
                break;
        }
    }

    protected virtual void CheckPlayerDistance()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (currentState == GhostState.Patrol && distanceToPlayer <= detectionRadius)
        {
            currentState = GhostState.Evade;
            // Interrompe qualquer espera de waypoint se for para fugir
            if (isWaitingAtWaypoint)
            {
                if (patrolWaitCoroutine != null)
                {
                    StopCoroutine(patrolWaitCoroutine);
                }
                isWaitingAtWaypoint = false;
            }
            Debug.Log("Fantasma detectou o jogador e está fugindo!");
        }
        else if (currentState == GhostState.Evade && distanceToPlayer > detectionRadius * 1.5f)
        {
            currentState = GhostState.Patrol;
            ChooseRandomWaypoint(); // Escolhe um novo waypoint para patrulhar
            Debug.Log("Fantasma conseguiu fugir e voltou à patrulha!");
        }
    }

    protected virtual void PatrolBehavior()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        // Só verifica a chegada ao waypoint se não estiver esperando
        if (!isWaitingAtWaypoint && Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.5f)
        {
            patrolWaitCoroutine = StartCoroutine(WaitForNextWaypoint());
        }
    }

    protected virtual void EvadeBehavior()
    {
        // Comportamento de fuga tratado em FixedUpdate e CheckPlayerDistance
    }

    // Coroutine para o atraso no waypoint
    protected IEnumerator WaitForNextWaypoint()
    {
        isWaitingAtWaypoint = true;
        // Se quiser que o fantasma pare completamente durante o delay
        rb.linearVelocity = Vector2.zero; 

        // Adicione aqui lógica para animação de "parado" 

        yield return new WaitForSeconds(waypointDecisionDelay);

        isWaitingAtWaypoint = false;
        ChooseRandomWaypoint();
        // A velocidade será retomada no FixedUpdate quando isWaitingAtWaypoint for false
    }

    protected void ChooseRandomWaypoint()
    {
        int newIndex = currentWaypointIndex;
        while (newIndex == currentWaypointIndex && waypoints.Count > 1)
        {
            newIndex = Random.Range(0, waypoints.Count);
        }
        currentWaypointIndex = newIndex;
        Debug.Log("Patrulhando para o waypoint: " + waypoints[currentWaypointIndex].name);
    }

    protected Vector2 AvoidObstacles(Vector2 desiredVelocity)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, desiredVelocity.normalized, obstacleDetectionDistance, obstacleLayer);

        if (hit.collider != null)
        {
            Debug.DrawRay(transform.position, desiredVelocity.normalized * obstacleDetectionDistance, Color.red);

            Vector2 avoidanceDirection;

            Vector2 right = Quaternion.Euler(0, 0, -90) * desiredVelocity.normalized;
            Vector2 left = Quaternion.Euler(0, 0, 90) * desiredVelocity.normalized;

            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, right, obstacleDetectionDistance, obstacleLayer);
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, left, obstacleDetectionDistance, obstacleLayer);

            if (hitRight.collider == null)
            {
                avoidanceDirection = right;
            }   
            else if (hitLeft.collider == null)
            {
                avoidanceDirection = left;
            }
            else
            {
                avoidanceDirection = ((Vector2)transform.position - hit.point).normalized;
            }

            return (desiredVelocity.normalized + avoidanceDirection.normalized).normalized * desiredVelocity.magnitude;
        }

        Debug.DrawRay(transform.position, desiredVelocity.normalized * obstacleDetectionDistance, Color.green);
        return desiredVelocity;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (waypoints != null && waypoints.Count > 0 && currentWaypointIndex < waypoints.Count)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, waypoints[currentWaypointIndex].position);
        }
    }

    // Função para atualizar animações
    protected void UpdateAnimations(Vector2 currentVelocity)
    {
        if (anim == null) return; // Garante que há um Animator

        // Animação de movimento horizontal
        if (currentVelocity.x != 0)
        {
            ResetLayers();
            anim.SetLayerWeight(1, 1); // Exemplo: layer para movimento lateral

            if (currentVelocity.x > 0)
            {
                spriteRenderer.flipX = false; // Usa spriteRenderer diretamente
            }
            else if (currentVelocity.x < 0)
            {
                spriteRenderer.flipX = true; // Usa spriteRenderer diretamente
            }
        }
        // Animação de movimento para cima
        else if (currentVelocity.y > 0) 
        {
            ResetLayers();
            anim.SetLayerWeight(2, 1); // layer para movimento para cima
        }
        // Animação de movimento para baixo
        else if (currentVelocity.y < 0) 
        {
            ResetLayers();
            anim.SetLayerWeight(0, 1); // layer para movimento para baixo
        }
        else // Fantasma parado
        {
            ResetLayers(); // Se estiver parado, pode querer ativar uma animação de "idle" padrão ou manter uma direção
            // Exemplo: se ele estava parado e o último movimento foi para baixo, mantenha a animação para baixo
        }
    }

    protected void ResetLayers()
    {
        if (anim == null) return;
        // Reseta os layers do Animator para 0
        for (int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }
    }
}