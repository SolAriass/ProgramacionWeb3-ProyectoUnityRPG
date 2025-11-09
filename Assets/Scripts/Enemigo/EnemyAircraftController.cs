using UnityEngine;

public class EnemyAircraftController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 90.0f;
    public float speedX = 25f;      // horizontal
    public float speedY = 15f;      // vertical
    
    public int health = 30; // vida inicial del enemigo
    public int pointsOnDeath = 10; // puntos al morir

    public LayerMask capaObstaculos;     // capa del suelo/obstáculos
    public float distanciaFrontal = 0.6f;   // largo del raycast frontal
    public float distanciaVertical = 0.5f; // rayos verticales (arriba/abajo) para buscar espacio

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
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position);

            if (direction.x < 0)
            {
                sr.flipX = false;
            }
            if (direction.x > 0)
            {
                sr.flipX = true;
            }

            float vx = Mathf.Sign(direction.x) * speedX;
            float vy = Mathf.Sign(direction.y) * speedY;

            // “Frente” depende del signo de X hacia donde vamos
            Vector2 frente = new Vector2(Mathf.Sign(direction.x == 0 ? 1f : direction.x), 0f);

            // Rayo frontal: ¿hay obstáculo delante?
            bool obstaculoFrente = Physics2D.Raycast(rb.position, frente, distanciaFrontal, capaObstaculos);

            if (obstaculoFrente)
            {
                // Probamos subir o bajar según espacio
                bool obstaculoArriba = Physics2D.Raycast(rb.position, Vector2.up, distanciaVertical, capaObstaculos);
                bool obstaculoAbajo = Physics2D.Raycast(rb.position, Vector2.down, distanciaVertical, capaObstaculos);

                if (!obstaculoArriba)
                {
                    vy = Mathf.Abs(speedY);    // subir
                }
                else if (!obstaculoAbajo)
                {
                    vy = -Mathf.Abs(speedY);    // bajar
                }
                else
                {
                    vx = 0f;                    // si no hay espacio, frenar en X
                }
            }

            movement = new Vector2(vx, vy);

            // enMovimiento = true;
        }

        else
        {
            movement = Vector2.zero;
            // enMovimiento = false;
        }
    }
        void FixedUpdate()
        {
            rb.linearVelocity = Vector2.zero;
            rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
        }
        
    

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibi� {damage} de da�o. Vida: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} muri�.");
        // ac� podr�as sumar puntos si ten�s un GameManager
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            
            collision.gameObject.GetComponent<JugadorController>().RecibeDanio(direccionDanio, 1);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (rb != null)
        {
            Vector2 frente = new Vector2(transform.localScale.x >= 0 ? 1f : -1f, 0f);
            Gizmos.color = Color.yellow; // frontal
            Gizmos.DrawLine(rb.position, rb.position + frente * distanciaFrontal);

            Gizmos.color = Color.green;  // arriba
            Gizmos.DrawLine(rb.position, rb.position + Vector2.up * distanciaVertical);

            Gizmos.color = Color.cyan;   // abajo
            Gizmos.DrawLine(rb.position, rb.position + Vector2.down * distanciaVertical);
        }
    }
}

