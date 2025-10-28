using UnityEngine;
using UnityEngine.InputSystem;

public class MovimientoJugador : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float fuerzaSalto = 30f;
    public float fuerzaRebote = 15f;
    public float longitudRaycast = 0.43f;
    public LayerMask Suelo;

    private bool enSuelo;
    private bool recibiendoDanio;
    public Rigidbody2D rb;

    public float velocidad = 100f;
    private Vector2 mover;
    private PlayerInput playerInput;
    [SerializeField] private Animator animator;

    SpriteRenderer sr;


    void Start()
    {
        // Tomamos la referencia al Player Input
        playerInput = GetComponent<PlayerInput>();

        // 🔧 Solución de la advertencia:
        // Desactiva todos los mapas de acción globales
        playerInput.actions.Disable();

        // Activa solo el mapa de acciones "Jugador"
        playerInput.actions.FindActionMap("Jugador").Enable();

        rb = GetComponent<Rigidbody2D>();

        sr = GetComponent<SpriteRenderer>();


    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desplazamiento = new Vector3(mover.x, mover.y, 0f) * velocidad * Time.deltaTime;

        animator.SetFloat("enMovimiento", mover.x * velocidad);

        if (mover.x < 0)
        {
            sr.flipX = true;
        }
        if (mover.x > 0)
        {
            sr.flipX = false;
        }

        if (!recibiendoDanio)
            transform.position += desplazamiento;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, Suelo);
        enSuelo = hit.collider != null;

        if (enSuelo && Input.GetKeyDown(KeyCode.Space) && !recibiendoDanio)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibiendoDanio);

    }
    // Este método debe tener el mismo nombre que la acción "Mover" en el Input Actions asset
    void OnMover(InputValue value)
    {
        mover = value.Get<Vector2>();
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;

            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.linearVelocity = Vector2.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }

    void FixedUpdate()
    {
        // opción A: velocidad (recomendada)
        rb.linearVelocity = new Vector2(mover.x * velocidad, rb.linearVelocity.y);

        // opción B: MovePosition (suave y sólido)
        // Vector2 delta = new Vector2(mover.x, 0f) * velocidad * Time.fixedDeltaTime;
        // rb.MovePosition(rb.position + delta);
    }

}

/*
Asegurate de que el nombre del método (OnMover) coincida exactamente con el nombre de la acción “Mover”.

Si la acción tiene otro nombre (por ejemplo “Move”), cambiá el método a OnMove o On<NameAcción>.
En versiones recientes de Unity se requiere que coincida exactamente.
*/