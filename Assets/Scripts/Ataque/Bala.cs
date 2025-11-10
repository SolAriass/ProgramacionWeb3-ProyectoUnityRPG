using UnityEngine;

public class Bala : MonoBehaviour
{
    public int damage = 3;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // si la bala toca un objeto con tag Enemy
        if (other.CompareTag("Enemigo"))
        {
            EnemyController enemigo = other.GetComponent<EnemyController>();
            EnemyAircraftController enemigoAereo = other.GetComponent<EnemyAircraftController>();

            if (enemigo != null)
            {
                enemigo.TakeDamage(damage);
            }else if(enemigoAereo != null)
            {
                enemigoAereo.TakeDamage(damage);
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


