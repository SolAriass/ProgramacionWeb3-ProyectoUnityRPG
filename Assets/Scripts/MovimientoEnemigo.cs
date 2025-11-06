using UnityEngine;

public class MovimientoEnemigo : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;

    public Rigidbody2D rb;
    private Vector2 movimiento;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic; // Evita empuje
    }

    void Update()
    {
        float distanciaAlJugador = Vector2.Distance(transform.position, player.position);
        if (distanciaAlJugador < detectionRadius)
        {
            Vector2 direccion = (player.position - transform.position).normalized;
            movimiento = new Vector2(direccion.x, 0); // Solo mueve horizontal
        }
        else
        {
            movimiento = Vector2.zero;
        }

        rb.MovePosition(rb.position + movimiento * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Atacar al jugador
            other.GetComponent<Jugador>()?.RecibirDaño(10);
        }
    }
}
