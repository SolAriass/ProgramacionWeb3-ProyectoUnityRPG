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

    // Para control de saltos (doble salto)
    private int saltosRestantes = 2;
    private const int MAX_SALTOS = 2;


    void Start()
    {
        // Tomamos la referencia al Player Input
        playerInput = GetComponent<PlayerInput>();

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
       
        animator.SetFloat("enMovimiento", Mathf.Abs(mover.x * velocidad));

        if (mover.x < 0) sr.flipX = true;
        if (mover.x > 0) sr.flipX = false;


        // Raycast para detectar suelo
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, Suelo);
        enSuelo = hit.collider != null;

        // Solo resetear saltos si está en el suelo Y cayendo (o parado)
        if (enSuelo && rb.linearVelocity.y <= 0.1f)
        {
            saltosRestantes = MAX_SALTOS;
        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibiendoDanio);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(mover.x * velocidad, rb.linearVelocity.y);
    }

    // Este método debe tener el mismo nombre que la acción "Mover" en el Input Actions asset
    void OnMover(InputValue value)
    {
        mover = value.Get<Vector2>();
    }

    // Método que maneja la acción de saltar (debe coincidir con "Saltar" en Input Actions)
    void OnSaltar(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (saltosRestantes > 0 && !recibiendoDanio)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            
            saltosRestantes--;
        }
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
}

/*
Asegurate de que el nombre del método (OnMover) coincida exactamente con el nombre de la acción "Mover".

Si la acción tiene otro nombre (por ejemplo "Move"), cambiá el método a OnMove o On<NameAcción>.
En versiones recientes de Unity se requiere que coincida exactamente.
*/