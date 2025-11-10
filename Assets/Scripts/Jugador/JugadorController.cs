using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class JugadorController : MonoBehaviour
{
    public float fuerzaSalto = 70f;
    public float fuerzaRebote = 70f;
    public float longitudRaycast = 0.7f;
    public LayerMask Suelo;

    private bool enSuelo;
    private bool recibiendoDanio;
    public bool EstaMuerto { get; private set; } = false;

    public Rigidbody2D rb;
    public float velocidad = 100f;

    private Vector2 mover;
    private PlayerInput playerInput;
    [SerializeField] private Animator animator;

    SpriteRenderer sr;

    private int saltosRestantes = 2;
    private const int MAX_SALTOS = 2;

    public int vida = 100;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Jugador").Enable();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!recibiendoDanio)
        {
            animator.SetFloat("enMovimiento", Mathf.Abs(mover.x * velocidad));

            if (mover.x < 0) sr.flipX = true;
            if (mover.x > 0) sr.flipX = false;
        }

        // Detectar suelo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, Suelo);
        enSuelo = hit.collider != null;

        if (enSuelo && rb.linearVelocity.y <= 0.1f)
        {
            saltosRestantes = MAX_SALTOS;
        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibiendoDanio);
    }

    void FixedUpdate()
    {
        if (!recibiendoDanio)
        {
            rb.linearVelocity = new Vector2(mover.x * velocidad, rb.linearVelocity.y);
        }
    }

    void OnMover(InputValue value)
    {
        if (EstaMuerto) return;
        mover = value.Get<Vector2>();
    }

    void OnSaltar(InputValue value)
    {
        if (EstaMuerto) return;

        if (!value.isPressed) return;

        if (saltosRestantes > 0 && !recibiendoDanio)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            saltosRestantes--;
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (recibiendoDanio) return;

        recibiendoDanio = true;

        RecibirDaño(cantDanio);
        animator.SetBool("recibeDanio", recibiendoDanio);

        // Aplicar knockback
        rb.linearVelocity = Vector2.zero;
        float sentido = (transform.position.x < direccion.x) ? -1f : 1f;
        Vector2 fuerzaKnockback = new Vector2(sentido * fuerzaRebote * 0.85f, fuerzaRebote * 0.55f);
        rb.AddForce(fuerzaKnockback, ForceMode2D.Impulse);

        Debug.Log($"💥 Jugador recibe {cantDanio} de daño | Vida: {vida}");
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    public void RecibirDaño(int cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            EstaMuerto = true;
            rb.linearVelocity = Vector2.zero;
            mover = Vector2.zero;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }

            Debug.Log("☠️ Jugador muerto - Game Over");
        }
    }
}

/*
Asegurate de que el nombre del método (OnMover) coincida exactamente con el nombre de la acción "Mover".

Si la acción tiene otro nombre (por ejemplo "Move"), cambiá el método a OnMove o On<NameAcción>.
En versiones recientes de Unity se requiere que coincida exactamente.
*/