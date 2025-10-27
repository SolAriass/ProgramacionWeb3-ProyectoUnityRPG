using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public int health = 30; // vida inicial del enemigo
    public int pointsOnDeath = 10; // puntos al morir

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
}
