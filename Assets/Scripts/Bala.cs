using UnityEngine;

public class Bala : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // si la bala toca un objeto con tag Enemy
        if (other.CompareTag("Enemigo"))
        {
            Enemigo enemigo = other.GetComponent<Enemigo>();
            if (enemigo != null)
            {
                enemigo.TakeDamage(damage);
            }

            Destroy(gameObject); // destruir la bala después de impactar
        }
        else if (!other.CompareTag("Player"))
        {
            // opcional: destruir bala si choca con algo que no sea el jugador
            Destroy(gameObject);
        }
    }
}


