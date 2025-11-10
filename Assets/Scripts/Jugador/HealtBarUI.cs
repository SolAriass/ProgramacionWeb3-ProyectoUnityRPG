using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    // Arrastra aquí el objeto HealthBarFill (el que tiene la imagen de Rick)
    public Image fillImage;

    // Arrastra aquí el GameObject de tu personaje (con JugadorController)
    public JugadorController playerController;

    private float maxHealth = 100f; // Usamos el valor confirmado de 100

    void Start()
    {
        if (playerController != null)
        {
            // Aunque lo definimos como 100f, lo leemos del script del jugador para asegurar
            maxHealth = playerController.vida;
        }
    }

    void Update()
    {
        if (playerController != null && fillImage != null)
        {
            // Calcula la proporción (Ej: 50 vida / 100 max = 0.5)
            float healthRatio = (float)playerController.vida / maxHealth;

            // Actualiza el Fill Amount.
            // Esto hará que Rick se "despínte" a medida que healthRatio disminuye.
            fillImage.fillAmount = healthRatio;
        }
    }
}