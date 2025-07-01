using System;
using UnityEngine;

// Garante que o componente Rigidbody2D sempre existir� neste GameObject.
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    //======================================================================
    //  CAMPOS SERIALIZADOS (Vis�veis no Inspector)
    //======================================================================
    [Header("Configura��es de Movimento")]
    [Tooltip("A velocidade base de movimento do jogador.")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Configura��es de Input")]
    [Tooltip("Nome do eixo de input horizontal (Project Settings -> Input Manager).")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [Tooltip("Nome do eixo de input vertical (Project Settings -> Input Manager).")]
    [SerializeField] private string verticalAxis = "Vertical";

    //======================================================================
    //  PROPRIEDADES P�BLICAS (Acesso para outros scripts)
    //======================================================================
    public float CurrentSpeed => moveSpeed;
    public Vector2 CurrentMovement => movement; // Renomeado para clareza
    public Vector2 LastMovementDirection { get; private set; }

    //======================================================================
    //  EVENTOS P�BLICOS
    //======================================================================
    public event Action<Vector2> OnMove;

    //======================================================================
    //  CAMPOS PRIVADOS (L�gica interna)
    //======================================================================
    private Rigidbody2D rb;
    private Vector2 movement;
    private float originalSpeed;

    //======================================================================
    //  M�TODOS DA UNITY
    //======================================================================
    private void Awake()
    {
        // � uma boa pr�tica obter componentes no Awake()
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Armazenar valores iniciais no Start()
        originalSpeed = moveSpeed;
        LastMovementDirection = Vector2.down; // Define uma dire��o inicial padr�o
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
        // A movimenta��o f�sica deve ocorrer no FixedUpdate
        Move();
    }

    //======================================================================
    //  M�TODOS P�BLICOS
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
    //  M�TODOS PRIVADOS (L�gica Interna)
    //======================================================================
    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw(horizontalAxis);
        float moveY = Input.GetAxisRaw(verticalAxis);

        Vector2 input = new(moveX, moveY);

        // Normaliza o vetor de input para evitar movimento diagonal mais r�pido
        movement = input.sqrMagnitude > 1f ? input.normalized : input;

        // Se o jogador estiver se movendo, atualiza a �ltima dire��o registrada
        if (movement.sqrMagnitude > 0.01f) // Usar um limiar pequeno � mais seguro que 0.1f
        {
            LastMovementDirection = movement.normalized;
        }

        // Notifica outros scripts sobre o vetor de movimento atual
        OnMove?.Invoke(movement);
    }

    private void Move()
    {
        // MovePosition � a forma correta de mover um Rigidbody, respeitando a f�sica
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);
    }
}