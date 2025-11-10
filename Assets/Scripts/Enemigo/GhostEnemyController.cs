using UnityEngine;

public class GhostEnemyController : MonoBehaviour
{
    // --- Variables de Patrulla ---
    [Header("Patrulla y Movimiento")]
    public float moveSpeed = 3f;           // Velocidad de movimiento del enemigo
    public float patrolDistance = 5f;      // Distancia a patrullar a cada lado del punto inicial
    private Vector2 startPosition;         // Posición inicial para calcular los límites
    private bool movingRight = true;       // Controla la dirección actual

    // --- Variables de Daño ---
    [Header("Ataque y Daño")]
    public int damageAmount = 1;           // Cantidad de daño a infligir al jugador
    public float damageCooldown = 1f;      // Tiempo entre cada vez que puede hacer daño
    private float lastDamageTime;          // Control del temporizador
    public string playerTag = "Player";    // Tag para identificar al jugador

    // --- Variables Comunes del Enemigo (Vida, Gráficos) ---
    [Header("Vida y Componentes")]
    public int health = 30;                // Vida inicial del enemigo
    public int pointsOnDeath = 10;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    // private Animator animator; // Si no lo usas para la patrulla, déjalo comentado

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        // animator = GetComponent<Animator>();

        // Guardar la posición inicial al empezar el juego
        startPosition = rb.position;
        // Inicializar el cooldown para que pueda atacar inmediatamente
        lastDamageTime = -damageCooldown;
    }

    void FixedUpdate()
    {
        HandlePatrolMovement();
    }

    /// <summary>
    /// Maneja el movimiento de izquierda a derecha.
    /// </summary>
    void HandlePatrolMovement()
    {
        Vector2 targetPosition;

        if (movingRight)
        {
            // Mover hacia el límite derecho
            targetPosition = new Vector2(startPosition.x + patrolDistance, rb.position.y);
            // Voltear sprite
            sr.flipX = true;
        }
        else
        {
            // Mover hacia el límite izquierdo
            targetPosition = new Vector2(startPosition.x - patrolDistance, rb.position.y);
            // Voltear sprite
            sr.flipX = false;
        }

        // Calcular la nueva posición un poco más cerca del objetivo
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // Comprobar si ha llegado al límite para cambiar de dirección
        if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            movingRight = !movingRight; // Invertir la dirección
        }
    }

    // --- Lógica de Daño al JUGADOR (Necesitas un Collider2D con Is Trigger activado) ---

    /// <summary>
    /// Se llama cuando el Collider (marcado como Trigger) entra o se mantiene en contacto.
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        // 1. Verificar el tag y el cooldown
        if (other.CompareTag(playerTag) && Time.time >= lastDamageTime + damageCooldown)
        {
            // 2. Intentar obtener el controlador del jugador
            JugadorController jugador = other.gameObject.GetComponent<JugadorController>();

            if (jugador != null)
            {
                // 3. Calcular la dirección del daño (necesaria para el knockback del jugador)
                Vector2 direccionDanio = (other.transform.position.x > transform.position.x) ? Vector2.right : Vector2.left;

                // 4. Llamar al método de daño del jugador
                jugador.RecibeDanio(direccionDanio, damageAmount);

                Debug.Log($"{gameObject.name} infligió {damageAmount} de daño a {other.gameObject.name}.");

                // 5. Reiniciar el temporizador
                lastDamageTime = Time.time;
            }
        }
    }

    // --- Lógica de Daño y Muerte del ENEMIGO (Reutilizada de tu script anterior) ---

    /// <summary>
    /// Método público para que el jugador (u otros elementos) inflijan daño.
    /// </summary>
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} murió. Puntos ganados: {pointsOnDeath}");
        // Aquí podrías sumar puntos si tienes un GameManager
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Muestra la distancia de patrulla en el editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, startPosition + Vector2.right * patrolDistance);
        Gizmos.DrawLine(startPosition, startPosition + Vector2.left * patrolDistance);
    }
}
