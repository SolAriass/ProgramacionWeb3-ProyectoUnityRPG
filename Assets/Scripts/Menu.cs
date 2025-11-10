using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("UI Opcional")]
    [SerializeField] private TextMeshProUGUI textoNombreJugador;
    [SerializeField] private TextMeshProUGUI textoPuntaje;

    void Start()
    {
        VerificarYMostrarSesion();
    }

    void VerificarYMostrarSesion()
    {
        // Verificar que haya una sesión activa
        if (!GameSession.Instance.SesionActiva)
        {
            Debug.LogWarning("⚠️ No hay sesión activa. Redirigiendo a registro...");
            SceneManager.LoadScene("RegistroJugador");
            return;
        }

        // Mostrar información del jugador
        if (textoNombreJugador != null)
        {
            textoNombreJugador.text = $"Jugador: {GameSession.Instance.NombreJugador}";
        }

        if (textoPuntaje != null)
        {
            textoPuntaje.text = $"Puntaje: {GameSession.Instance.PuntajeTotal} pts\nNivel Máximo: {GameSession.Instance.NivelMaximo}";
        }

        Debug.Log($"📱 Menú cargado para: {GameSession.Instance.NombreJugador}");
    }

    public void Jugar()
    {
        // Verificar sesión antes de jugar
        if (!GameSession.Instance.SesionActiva)
        {
            Debug.LogWarning("⚠️ No hay sesión activa. Redirigiendo a registro...");
            SceneManager.LoadScene("RegistroJugador");
            return;
        }

        Debug.Log($"🎮 Iniciando juego para: {GameSession.Instance.NombreJugador}");
        SceneManager.LoadScene("Escena1");
    }

    public void Ranking()
    {
        SceneManager.LoadScene("RankingScene");
    }

    public void Salir()
    {
        Debug.Log("👋 Cerrando juego...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
