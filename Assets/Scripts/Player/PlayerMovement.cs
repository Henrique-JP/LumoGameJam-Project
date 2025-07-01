using System;
using UnityEngine;

// Garante que o componente Rigidbody2D sempre existirá neste GameObject.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    //======================================================================
    //  CAMPOS SERIALIZADOS (Visíveis no Inspector)
    //======================================================================
    [Header("Configurações de Movimento")]
    [Tooltip("A velocidade base de movimento do jogador.")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Configurações de Input")]
    [Tooltip("Nome do eixo de input horizontal (Project Settings -> Input Manager).")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [Tooltip("Nome do eixo de input vertical (Project Settings -> Input Manager).")]
    [SerializeField] private string verticalAxis = "Vertical";

    //======================================================================
    //  PROPRIEDADES PÚBLICAS (Acesso para outros scripts)
    //======================================================================
    public float CurrentSpeed => moveSpeed;
    public Vector2 CurrentMovement => movement; // Renomeado para clareza
    public Vector2 LastMovementDirection { get; private set; }

    //======================================================================
    //  EVENTOS PÚBLICOS
    //======================================================================
    public event Action<Vector2> OnMove;

    //======================================================================
    //  CAMPOS PRIVADOS (Lógica interna)
    //======================================================================
    private Rigidbody2D rb;
    private Vector2 movement;
    private float originalSpeed;

    //======================================================================
    //  MÉTODOS DA UNITY
    //======================================================================
    private void Awake()
    {
        // É uma boa prática obter componentes no Awake()
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Armazenar valores iniciais no Start()
        originalSpeed = moveSpeed;
        LastMovementDirection = Vector2.down; // Define uma direção inicial padrão
    }

    private void Update()
    {
        // Pausa o processamento de input se o jogo estiver pausado
        if (Time.timeScale == 0f)
        {
            movement = Vector2.zero;
            return;
        }
        ProcessInputs();
    }

    private void FixedUpdate()
    {
        // A movimentação física deve ocorrer no FixedUpdate
        Move();
    }

    //======================================================================
    //  MÉTODOS PÚBLICOS
    //======================================================================
    public void ApplySpeedModifier(float newSpeed)
    {
        if (newSpeed >= 0)
        {
            moveSpeed = newSpeed;
        }
    }

    public void ResetSpeed()
    {
        moveSpeed = originalSpeed;
    }

    //======================================================================
    //  MÉTODOS PRIVADOS (Lógica Interna)
    //======================================================================
    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw(horizontalAxis);
        float moveY = Input.GetAxisRaw(verticalAxis);

        Vector2 input = new(moveX, moveY);

        // Normaliza o vetor de input para evitar movimento diagonal mais rápido
        movement = input.sqrMagnitude > 1f ? input.normalized : input;

        // Se o jogador estiver se movendo, atualiza a última direção registrada
        if (movement.sqrMagnitude > 0.01f) // Usar um limiar pequeno é mais seguro que 0.1f
        {
            LastMovementDirection = movement.normalized;
        }

        // Notifica outros scripts sobre o vetor de movimento atual
        OnMove?.Invoke(movement);
    }

    private void Move()
    {
        // MovePosition é a forma correta de mover um Rigidbody, respeitando a física
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);
    }
}