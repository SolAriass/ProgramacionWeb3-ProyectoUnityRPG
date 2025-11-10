using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Movimiento : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float moveSpeed = 7f;

    [Header("Salto (ajusta acá)")]
    [SerializeField] float desiredJumpHeight = 2.0f;   // altura en unidades Unity
    [SerializeField] float fallMultiplier = 1.8f;      // acelera caída
    [SerializeField] float lowJumpMultiplier = 2.2f;   // salto corto si soltás la tecla

    [Header("Input System")]
    [SerializeField] InputActionReference move;  // Player/Move (Vector2)
    [SerializeField] InputActionReference jump;  // Player/Jump (Button)
    [SerializeField] InputActionReference pausa; // Player/Pausa (Button)


    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Vector2 groundCheckSize = new(0.6f, 0.1f);
    [SerializeField] LayerMask groundMask;

    [Header("Ground Check Method")]
    [SerializeField] bool useRaycast = true; // NUEVO: Opción para cambiar método

    Rigidbody2D rb;
    SpriteRenderer sr;

    float moveX;
    bool isGrounded;
    bool jumpUsed;   // <-- candado: evita saltos hasta volver a tocar piso

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable() { move.action.Enable(); jump.action.Enable(); pausa.action.Enable(); }
    void OnDisable() { move.action.Disable(); jump.action.Disable(); pausa.action.Disable(); }

    void Update()
    {
        // Movimiento lateral
        Vector2 input = move.action.ReadValue<Vector2>();
        moveX = Mathf.Clamp(input.x, -1f, 1f);
        if (moveX != 0) sr.flipX = moveX < 0;

        // SALTO: solo si está en suelo y no se usó ya
        if (jump.action.triggered && isGrounded && !jumpUsed)
        {
            float g = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
            float v = Mathf.Sqrt(2f * g * desiredJumpHeight);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, v);
            jumpUsed = true;   // bloquea nuevos saltos en el aire
        }

        if (pausa.action.triggered)
        {
            SceneManager.LoadScene("Inicio");
        }
    }

    void FixedUpdate()
    {
        // Detecci�n de suelo
        if (groundCheck)
            isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundMask);

        // si toca el suelo, se habilita de nuevo el salto
        if (isGrounded && rb.linearVelocity.y <= 0.01f)
            jumpUsed = false;

        // Velocidad horizontal
        rb.linearVelocity = new Vector2(moveX * moveSpeed, rb.linearVelocity.y);

        // Caída más rápida y salto corto
        float g = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        bool jumpHeld = jump.action.IsPressed();

        if (rb.linearVelocity.y < 0f) // cayendo
        {
            rb.linearVelocity += Vector2.down * g * (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !jumpHeld) // soltó la tecla
        {
            rb.linearVelocity += Vector2.down * g * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        
        Gizmos.color = Color.yellow;
        
        if (useRaycast)
        {
            // Dibujar raycast
            Gizmos.DrawLine(groundCheck.position, 
                groundCheck.position + Vector3.down * groundCheckSize.y);
        }
        else
        {
            // Dibujar box
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}
}