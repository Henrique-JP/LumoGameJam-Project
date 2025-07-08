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
    public ParticleSystem dustEffect; // Efeito de poeira, se houver

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

    private Animator anim; // Refer��ncia ao Animator, se houver
    private SoundManager soundManager; // Refer��ncia ao SoundManager, se houver

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
        anim = GetComponent<Animator>(); // Obtendo o Animator, se existir
        soundManager = GameObject.FindFirstObjectByType<SoundManager>(); // Obtendo o SoundManager, se existir
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
            if (dustEffect != null && !dustEffect.isPlaying)
            {
                dustEffect.Play(); // Inicia o efeito de poeira se estiver definido
                soundManager?.PlayerWalkSource.Play(); // Toca o som de passos do jogador, se existir
            }
        }
        else if (dustEffect != null && dustEffect.isPlaying)
        {
            dustEffect.Stop(); // Para o efeito de poeira se n��o houver movimento
            soundManager?.PlayerWalkSource.Stop(); // Para o som de passos do jogador, se existir
        }

        // Notifica outros scripts sobre o vetor de movimento atual
        OnMove?.Invoke(movement);

        //Controle das animações
        if (input.x != 0)
        {
            ResetLayers();
            anim.SetLayerWeight(1, 1);

            if (input.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
            }
            else if (input.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
        }

        //cima
        if (input.y > 0 && input.x == 0)
        {
            ResetLayers();
            anim.SetLayerWeight(2, 1);
        }
        //baixo
        else if (input.y < 0 && input.x == 0)
        {
            ResetLayers();
            anim.SetLayerWeight(0, 1);
        }
        //cima e esquerda
        else if (input.y > 0 && input.x < 0)
        {
            ResetLayers();
            anim.SetLayerWeight(3, 1);
        }
        //cima e direita
        else if (input.y > 0 && input.x > 0)
        {
            ResetLayers();
            anim.SetLayerWeight(3, 1);
        }
        //baixo e esquerda
        else if (input.y < 0 && input.x < 0)
        {
            ResetLayers();
            anim.SetLayerWeight(4, 1);
        }
        //baixo e direita
        else if (input.y < 0 && input.x > 0)
        {
            ResetLayers();
            anim.SetLayerWeight(4, 1);
        }

        if (input != Vector2.zero)
        {
            anim.SetBool("Walking", true);
        }
        else
        {
            anim.SetBool("Walking", false);
        }
    }

    private void Move()
    {
        // MovePosition � a forma correta de mover um Rigidbody, respeitando a f�sica
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);
    }

    void ResetLayers()
    {
        // Reseta os layers do Animator
        anim.SetLayerWeight(0, 0);
        anim.SetLayerWeight(1, 0);
        anim.SetLayerWeight(2, 0);
        anim.SetLayerWeight(3, 0);
        anim.SetLayerWeight(4, 0);
    }
}