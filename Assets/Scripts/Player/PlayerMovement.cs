using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    private Rigidbody2D rb;
    private Vector2 movement;
    private float originalSpeed;

    // --- ADICIONADO ---
    // Propriedade publica para que outros scripts saibam a ultima direcao do movimento.
    public Vector2 LastMovementDirection { get; private set; }

    public float CurrentSpeed => moveSpeed;
    public Vector2 Movement => movement;
    public event Action<Vector2> OnMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalSpeed = moveSpeed;

        // --- ADICIONADO ---
        // Define uma direcao inicial padrao para evitar que comece em (0,0).
        LastMovementDirection = Vector2.right;
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            movement = Vector2.zero;
            return;
        }
        ProcessInputs();
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            Move();
        }
    }

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

    private void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw(horizontalAxis);
        float moveY = Input.GetAxisRaw(verticalAxis);

        Vector2 input = new(moveX, moveY);
        movement = input.sqrMagnitude > 1f ? input.normalized : input;

        // --- ADICIONADO ---
        // Se o jogador estiver se movendo, atualiza a ultima direcao.
        if (movement.sqrMagnitude > 0.1f)
        {
            LastMovementDirection = movement.normalized;
        }

        OnMove?.Invoke(movement);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);
    }
}