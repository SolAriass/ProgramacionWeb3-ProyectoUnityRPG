using UnityEngine;

public class FireballDestroyer : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Detección del Suelo (Untagged)
        // El suelo por defecto de Unity usa la etiqueta "Untagged".
        // Opcionalmente, podrías revisar por Tag o por Layer. Usaremos Tag.
        if (collision.gameObject.CompareTag("Untagged") || collision.gameObject.CompareTag("Enemigo"))
        {
            // 🚨 ¡Aquí, 'gameObject' es la bola de fuego, lo cual es correcto!
            Destroy(gameObject);
        }

        // 2. Detección del Jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Lógica de daño al jugador
            Vector2 direccionDanio = (collision.transform.position - transform.position).normalized;
            // Debes asegurarte que JugadorController está accesible y tiene RecibeDanio
            collision.gameObject.GetComponent<JugadorController>().RecibeDanio(direccionDanio, 15);

            Destroy(gameObject); // Destruye la bola después de dañar.
        }
    }

    void Start()
    {
        // Destruye la bola después de 10 segundos, si no ha tocado nada.
        Destroy(gameObject, 10f);
    }
}