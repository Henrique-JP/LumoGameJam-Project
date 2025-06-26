using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    //---- Variaveis do movimento encapsuladas ----
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";

    //---- Variaveis Internas ----
    private Rigidbody2D rb;
    private Vector2 movement;
    private float originalSpeed;

    //---- Propriedades Publicas ----
    public float CurrentSpeed => moveSpeed;
    public Vector2 Movement => movement;

    //---- Evento para outros scripts ----
    public event Action<Vector2> OnMove;

    //----Metodos da Unity ----
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Obtém o componente Rigidbody2D
        originalSpeed = moveSpeed; // Armazena a velocidade original
    }

    void Update()
    {

        if (Time.timeScale == 0f) // Verifica se o jogo está pausado
        {
            movement = Vector2.zero;
            return;
        }
        ProcessInputs();
    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero) // Verifica se há movimento
        {
            Move();
        }
    }

    //---- Metodos publicos (modificadores seguros) ----
    public void ApplySpeedModifier(float newSpeed)
    {

        if (newSpeed >= 0)   //Garante que a velocidade nao seja negativa
        {
            moveSpeed = newSpeed;
        }
    }

    public void ResetSpeed()
    {
        moveSpeed = originalSpeed; //Restaura a velocidade original
    }
    //---- Metodos privados (logica interna) ----
    private void ProcessInputs() // Processa as entradas de movimento
    {
        float moveX = Input.GetAxisRaw(horizontalAxis);
        float moveY = Input.GetAxisRaw(verticalAxis);

        Vector2 input = new(moveX, moveY);
        movement = input.sqrMagnitude > 1f ? input.normalized : input;

        OnMove?.Invoke(movement); //Dispara o evento de movimento
    }

    private void Move() // Move o jogador com base na entrada
    {
        rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement);
    }
}
