using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class JugadorController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Para control de saltos (doble salto)
    private int saltosRestantes = 2;
    private const int MAX_SALTOS = 2;

    public int vida = 100;


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
        if (!recibiendoDanio)
        {
            animator.SetFloat("enMovimiento", Mathf.Abs(mover.x * velocidad));

            if (mover.x < 0) sr.flipX = true;
            if (mover.x > 0) sr.flipX = false;

        }
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
        if (!recibiendoDanio)
        {
            rb.linearVelocity = new Vector2(mover.x * velocidad, rb.linearVelocity.y);
        }
        //else
        //{
        //    knockbackTimer -= Time.fixedDeltaTime;
        //    if (knockbackTimer <= 0f)
        //    {
        //        DesactivaDanio();
        //    }
        //}
    }

    // Este método debe tener el mismo nombre que la acción "Mover" en el Input Actions asset
    void OnMover(InputValue value)
    {
        if(EstaMuerto) return;
        mover = value.Get<Vector2>();
    }

    // Método que maneja la acción de saltar (debe coincidir con "Saltar" en Input Actions)
    void OnSaltar(InputValue value)
    {
        if (EstaMuerto) return; 

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
        if (recibiendoDanio) return;

        recibiendoDanio = true;
        //knockbackTimer = tiempoRebote;

        Debug.Log("Jugador recibe daño!");

        animator.SetBool("recibeDanio", recibiendoDanio);
        rb.linearVelocity = Vector2.zero;

        float sentido = (transform.position.x < direccion.x) ? -1f : 1f;
        Vector2 rebote = new Vector2(sentido * 1f, 0.6f); // inclinación de 60% hacia arriba

            if(recibiendoDanio)
            {
                RecibirDaño(cantDanio);
            } 


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

    public void RecibirDaño(int cantidad)
    {
        vida -= cantidad;
        Debug.Log("Vida del jugador: " + vida);


        if (vida <= 0)
        {
            EstaMuerto = true;

            rb.linearVelocity = Vector2.zero;    
            mover = Vector2.zero; 

            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }

            Debug.Log("Jugador muerto!");
        }
    }
}

/*
Asegurate de que el nombre del método (OnMover) coincida exactamente con el nombre de la acción "Mover".

Si la acción tiene otro nombre (por ejemplo "Move"), cambiá el método a OnMove o On<NameAcción>.
En versiones recientes de Unity se requiere que coincida exactamente.
*/