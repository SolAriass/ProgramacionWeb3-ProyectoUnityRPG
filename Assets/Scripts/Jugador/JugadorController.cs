using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class JugadorController : MonoBehaviour
{
    public float fuerzaSalto = 30f;
    public float fuerzaRebote = 20f;
    public float longitudRaycast = 0.5f;
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

    [Header("Ground Check - Edge Collider")]
    [SerializeField] private bool usarMultiplesRaycast = true;
    [SerializeField] private float anchoDeteccion = 0.6f;
    [SerializeField] private int numeroDeRaycasts = 5;
    [SerializeField] private float offsetRaycast = 0.2f;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.actions.Disable();
        playerInput.actions.FindActionMap("Jugador").Enable();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // ═══════════════════════════════════════════════════════════
        // CONFIGURACIÓN AUTOMÁTICA ANTI-TRABADO
        // ═══════════════════════════════════════════════════════════
        ConfigurarAntiTrabado();
    }

    // ═══════════════════════════════════════════════════════════
    // Configuración para evitar trabado en bordes
    // ═══════════════════════════════════════════════════════════
    void ConfigurarAntiTrabado()
    {
        if (rb == null) return;

        // 1. Collision Detection continuo
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // 2. Interpolate para movimiento suave
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // 3. Congelar rotación
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // 4. Sin drag
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        
        // 5. Configurar el collider
        ConfigurarColliderJugador();
        
        Debug.Log("✓ Configuración anti-trabado aplicada");
    }

    void ConfigurarColliderJugador()
    {
        // Intentar obtener BoxCollider2D
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            // ═══════════════════════════════════════════════════════════
            // SOLUCIÓN: Edge Radius redondea las esquinas
            // ═══════════════════════════════════════════════════════════
            boxCollider.edgeRadius = 0.05f; // Ajustar según tu sprite (0.05 a 0.1)
            Debug.Log("✓ BoxCollider2D configurado con Edge Radius: " + boxCollider.edgeRadius);
        }
        
        // Intentar obtener CapsuleCollider2D (RECOMENDADO)
        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider != null)
        {
            capsuleCollider.direction = CapsuleDirection2D.Vertical;
            Debug.Log("✓ CapsuleCollider2D detectado (ideal para platformers)");
        }
        
        // Si no tiene ninguno, advertir
        if (boxCollider == null && capsuleCollider == null)
        {
            Debug.LogWarning("⚠ No se encontró BoxCollider2D ni CapsuleCollider2D. Agrega uno al jugador.");
        }
    }

    void Update()
    {
        animator.SetFloat("enMovimiento", Mathf.Abs(mover.x * velocidad));

        if (mover.x < 0) sr.flipX = true;
        if (mover.x > 0) sr.flipX = false;

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

    void FixedUpdate()
    {
        // Mantener velocidad Y intacta
        rb.linearVelocity = new Vector2(mover.x * velocidad, rb.linearVelocity.y);
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
        if (EstaMuerto)
            return;
        mover = value.Get<Vector2>();
    }

    void OnSaltar(InputValue value)
    {
        if (EstaMuerto)
            return;

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
          Debug.Log("Jugador recibe daño!");
          recibiendoDanio = true;

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
        Debug.Log("Vida del jugador: " + vida);


        if (vida <= 0)
        {
            EstaMuerto = true;
            animator.SetBool("EstaMuerto", true);
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