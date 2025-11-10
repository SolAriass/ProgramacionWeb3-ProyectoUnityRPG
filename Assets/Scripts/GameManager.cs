using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referencias UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Button reiniciarButton;
    public Button menuButton;

    private bool gameOverActivo = false;

    void Awake()
    {
        // Singleton mejorado con validación
        if (Instance == null)
        {
            Instance = this;
            Debug.Log($"✅ GameManager inicializado: {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"⚠️ GameManager duplicado detectado en {gameObject.name}. Destruyendo...");
            Destroy(gameObject);
            return;
        }

        // Validar referencias
        ValidarReferencias();
    }

    void ValidarReferencias()
    {
        if (gameOverPanel == null)
        {
            Debug.LogError("❌ gameOverPanel NO asignado en el Inspector del GameManager");
        }
        else
        {
            Debug.Log($"✅ gameOverPanel asignado: {gameOverPanel.name}");
        }

        if (gameOverText == null)
        {
            Debug.LogWarning("⚠️ gameOverText NO asignado (opcional)");
        }

        if (reiniciarButton == null)
        {
            Debug.LogWarning("⚠️ reiniciarButton NO asignado (opcional)");
        }

        if (menuButton == null)
        {
            Debug.LogWarning("⚠️ menuButton NO asignado (opcional)");
        }
    }

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("🔒 gameOverPanel desactivado al iniciar");
        }

        if (reiniciarButton != null)
        {
            reiniciarButton.onClick.AddListener(ReiniciarEscena);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(IrAlMenu);
        }
    }

    void Update()
    {
        if (gameOverActivo)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReiniciarEscena();
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.M))
            {
                IrAlMenu();
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("🎮 GameOver() LLAMADO");
        Debug.Log($"   gameOverActivo: {gameOverActivo}");

        if (gameOverActivo)
        {
            Debug.LogWarning("   ⚠️ GameOver ya estaba activo. Ignorando llamada duplicada.");
            Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            return;
        }

        gameOverActivo = true;
        Debug.Log("   ✅ gameOverActivo = true");

        // Pausar el juego (opcional)
        Time.timeScale = 1f;
        Debug.Log("   ⏸️ Time.timeScale = 0 (juego pausado)");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log($"   ✅ gameOverPanel activado: {gameOverPanel.name}");
        }
        else
        {
            Debug.LogError("   ❌ ERROR: gameOverPanel es NULL. No se puede mostrar el panel.");
        }

        if (gameOverText != null)
        {
            gameOverText.text = "¡Has Perdido!\nPresiona 'R' para reiniciar\nPresiona 'M' para ir al menú";
            Debug.Log("   ✅ Texto de Game Over actualizado");
        }

        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }

    public void ReiniciarEscena()
    {
        Debug.Log("🔄 Reiniciando escena...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        Debug.Log("🏠 Volviendo al menú...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Inicio");
    }
}
