using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 90.0f;
    public float speed = 30.0f;
    public int health = 30; // vida inicial del enemigo
    public int pointsOnDeath = 10; // puntos al morir
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

        // 1. 🛑 EJECUTAR LA LÓGICA DE JUEGO ANTES DE DESTRUIR EL OBJETO
        if (gameObject.CompareTag("JefeFinal"))
        {
            // El GameObject.Find() debe buscar el Canvas por su nombre.
            // Nota: Si el CanvasFinalGame está desactivado, GameObject.Find() NO LO ENCUENTRA.
            // Es mejor hacerlo estático o buscar un objeto padre activo.

            // Asumiendo que CanvasFinalGame está ACTIVO al inicio, pero con su PANEL DESACTIVADO, o:

            // Opción Segura (Si CanvasFinalGame está siempre activo en la jerarquía):
            // Usar FindObjectOfType si el CanvasFinalGame es una variable estática del GameManager

            // Si usamos GameObject.Find, el Canvas DEBE ESTAR ACTIVO para ser encontrado:
            victoriaGame = GameObject.Find("CanvasFinalGame");
            Transform panelTransform = victoriaGame.transform.Find("Panel");

            if (panelTransform != null)
            {
                panel = panelTransform.gameObject;

                // ✅ Activamos el panel (o podés poner false para ocultarlo)
                panel.SetActive(true);
            }

            if (victoriaGame != null)
            {
                victoriaGame.SetActive(true); // Activa el Canvas Final
                Time.timeScale = 0f;          // Pausa el juego
                Debug.Log("¡Jefe Final derrotado! Mostrando pantalla de Victoria.");
            }
            else
            {
                Debug.LogError("Error: No se encontró el CanvasFinalGame. Asegúrate de que está activo o usa FindObjectOfType.");
            }
        }

        // 2. DESTRUIR EL OBJETO AL FINAL
        // acá podrías sumar puntos si tenés un GameManager
        Destroy(gameObject);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            
            collision.gameObject.GetComponent<JugadorController>().RecibeDanio(direccionDanio, 10);
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

