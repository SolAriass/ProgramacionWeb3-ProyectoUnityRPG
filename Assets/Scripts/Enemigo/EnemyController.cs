using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 90.0f;
    public float speed = 30.0f;
    public int health = 30; // vida inicial del enemigo
    public int pointsOnDeath = 10; // puntos al morir
    public int cantidadDeDanioHaciaJugador = 10;
    [SerializeField] private JugadorController jugador;
    
    private GameObject victoriaGame;
    private GameObject panel;

    public Rigidbody2D rb;
    private Vector2 movement;
    // private bool enMovimiento;
    private Animator animator;
    SpriteRenderer sr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            if (direction.x < 0)
            {
                sr.flipX = false;
            }
            if (direction.x > 0)
            {
                sr.flipX = true;
            }

            movement = new Vector2(direction.x, 0);

            // enMovimiento = true;
        }

        else
        {
            movement = Vector2.zero;
            //enMovimiento = false;
        }


    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida: {health}");

        if (health <= 0)
        {
            
            Die();
        }
    }


    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
    }
    
    void Die()
    {
        Debug.Log($"{gameObject.name} murió.");
        
        // Acumular puntos al jugador
        if (jugador != null)
        {
            jugador.acumularPuntaje(pointsOnDeath);
        }

        // Verificar si es el Jefe Final
        if (gameObject.CompareTag("JefeFinal"))
        {
            victoriaGame = GameObject.Find("CanvasFinalGame");
            
            if (victoriaGame != null)
            {
                Transform panelTransform = victoriaGame.transform.Find("Panel");

                if (panelTransform != null)
                {
                    panel = panelTransform.gameObject;
                    panel.SetActive(true);
                }

                victoriaGame.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("¡Jefe Final derrotado! Mostrando pantalla de Victoria.");
            }
            else
            {
                Debug.LogError("Error: No se encontró el CanvasFinalGame. Asegúrate de que está activo o usa FindObjectOfType.");
            }
        }

        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            
            collision.gameObject.GetComponent<JugadorController>().RecibeDanio(direccionDanio, cantidadDeDanioHaciaJugador);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

