using TMPro;
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
    private int _monedasAcumuladas = 0;
    private int _puntosAcumulados = 0;

    public Rigidbody2D rb;
    public float velocidad = 100f;

    private Vector2 mover;
    private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI textoMonedasUI;
    [SerializeField] private TextMeshProUGUI textoPuntaje;




    SpriteRenderer sr;

    private int saltosRestantes = 2;
    private const int MAX_SALTOS = 2;

    public int vida = 100;

    [Header("Ground Check - Edge Collider")]
    [SerializeField] private bool usarMultiplesRaycast = true;
    [SerializeField] private float anchoDeteccion = 0.6f;
    [SerializeField] private int numeroDeRaycasts = 5;
    [SerializeField] private float offsetRaycast = 0.2f;

    void Start()
    {
        // Tomamos la referencia al Player Input
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Jugador").Enable();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        
        // Inicializar UI de monedas y puntaje
        textoMonedasUI.text = "0";
        textoPuntaje.text = "Puntos: 0";

        ConfigurarAntiTrabado();
    }

    void ConfigurarAntiTrabado()
    {
        if (rb == null) return;

        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        ConfigurarColliderJugador();
    }

    void ConfigurarColliderJugador()
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.edgeRadius = 0.05f;
        }

        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider != null)
        {
            capsuleCollider.direction = CapsuleDirection2D.Vertical;
        }
    }

    void Update()
    {
        if (!recibiendoDanio)
        {
            animator.SetFloat("enMovimiento", Mathf.Abs(mover.x * velocidad));

            if (mover.x < 0) sr.flipX = true;
            if (mover.x > 0) sr.flipX = false;
        }

        // Detección de suelo
        if (usarMultiplesRaycast)
        {
            enSuelo = DetectarSueloMultiple();
        }
        else
        {
            Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - offsetRaycast);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, longitudRaycast, Suelo);
            enSuelo = hit.collider != null;
        }

        if (enSuelo && rb.linearVelocity.y <= 0.1f)
        {
            saltosRestantes = MAX_SALTOS;
        }

        animator.SetBool("ensuelo", enSuelo);
        animator.SetBool("recibeDanio", recibiendoDanio);
    }


    public void acumularPuntaje(int puntos)
    {
       _puntosAcumulados += puntos;
         textoPuntaje.text = "Puntos: " + _puntosAcumulados.ToString();
    }


    public void acumularMonedas()
    {
        _monedasAcumuladas++;
        Debug.Log("¡Conseguiste una moneda!");
        textoMonedasUI.text = _monedasAcumuladas.ToString();
    }

    void FixedUpdate()
    {
        if (!recibiendoDanio)
        {
            rb.linearVelocity = new Vector2(mover.x * velocidad, rb.linearVelocity.y);
        }
    }

    private bool DetectarSueloMultiple()
    {
        for (int i = 0; i < numeroDeRaycasts; i++)
        {
            float offset = 0f;
            if (numeroDeRaycasts > 1)
            {
                offset = Mathf.Lerp(-anchoDeteccion / 2f, anchoDeteccion / 2f, (float)i / (numeroDeRaycasts - 1));
            }

            Vector2 rayOrigin = new Vector2(transform.position.x + offset, transform.position.y - offsetRaycast);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, longitudRaycast, Suelo);

            if (hit.collider != null)
            {
                return true;
            }
        }

        return false;
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
        if (usarMultiplesRaycast)
        {
            for (int i = 0; i < numeroDeRaycasts; i++)
            {
                float offset = 0f;
                if (numeroDeRaycasts > 1)
                {
                    offset = Mathf.Lerp(-anchoDeteccion / 2f, anchoDeteccion / 2f, (float)i / (numeroDeRaycasts - 1));
                }

                Vector3 rayOrigin = new Vector3(
                    transform.position.x + offset,
                    transform.position.y - offsetRaycast,
                    0f
                );

                Gizmos.color = enSuelo ? Color.green : Color.red;
                Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * longitudRaycast);
                Gizmos.DrawWireSphere(rayOrigin, 0.03f);
            }
        }
        else
        {
            Vector3 rayOrigin = new Vector3(
                transform.position.x,
                transform.position.y - offsetRaycast,
                0f
            );

            Gizmos.color = enSuelo ? Color.green : Color.red;
            Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * longitudRaycast);
            Gizmos.DrawWireSphere(rayOrigin, 0.05f);
        }
    }

    public void RecibirDaño(int cantidad)
    {
        vida -= cantidad;

        if (vida <= 0)
        {
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Debug.Log("☠️ JUGADOR MUERTO - Iniciando Game Over");
            Debug.Log($"   EstaMuerto: {EstaMuerto} → true");

            EstaMuerto = true;

            // Intentar activar animación de muerte (solo si existe el parámetro)
            if (animator != null)
            {
                foreach (var param in animator.parameters)
                {
                    if (param.name == "EstaMuerto")
                    {
                        animator.SetBool("EstaMuerto", true);
                        Debug.Log("   ✅ Animación 'EstaMuerto' activada");
                        break;
                    }
                }
            }

            // Detener movimiento
            rb.linearVelocity = Vector2.zero;
            mover = Vector2.zero;

            // Verificar GameManager
            if (GameManager.Instance == null)
            {
                Debug.LogError("   ❌ ERROR: GameManager.Instance es NULL");
                Debug.LogError("   → Verifica que haya un GameManager en la escena");
            }
            else
            {
                Debug.Log($"   ✅ GameManager encontrado: {GameManager.Instance.name}");
                Debug.Log("   🎮 Llamando a GameOver()...");
                GameManager.Instance.GameOver();
            }

            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        }
    }
}

/*
Asegurate de que el nombre del método (OnMover) coincida exactamente con el nombre de la acción "Mover".

Si la acción tiene otro nombre (por ejemplo "Move"), cambiá el método a OnMove o On<NameAcción>.
En versiones recientes de Unity se requiere que coincida exactamente.
*/