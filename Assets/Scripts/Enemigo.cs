using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public int health = 30; // vida inicial del enemigo
    public int pointsOnDeath = 10; // puntos al morir

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
        Debug.Log($"{gameObject.name} murió.");
        // acá podrías sumar puntos si tenés un GameManager
        Destroy(gameObject);
    }
}
