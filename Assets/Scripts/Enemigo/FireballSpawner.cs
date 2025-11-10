using System.Collections; // ¡Necesario para el temporizador!
using UnityEngine;

public class FireballSpawner : MonoBehaviour
{
    public GameObject fireballPrefab;

    // Controla el tiempo entre caídas
    public float tiempoEntreBolas = 3f;

    // Al inicio del juego, iniciamos el temporizador
    void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    // Corrutina: Permite esperar un tiempo sin congelar el juego
    IEnumerator SpawnCycle()
    {
        // Bucle infinito para que la generación sea constante
        while (true)
        {
            // Pausa la función por el tiempo definido
            yield return new WaitForSeconds(tiempoEntreBolas);

            // Crea la bola de fuego en la posición del objeto Generador
            Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        }
    }
}
