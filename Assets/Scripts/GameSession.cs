using UnityEngine;

/// <summary>
/// Singleton que mantiene los datos del jugador SOLO durante esta sesión.
/// Se destruye automáticamente al cerrar el juego (no usa PlayerPrefs).
/// </summary>
public class GameSession : MonoBehaviour
{
    private static GameSession _instance;

    public static GameSession Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameSession");
                _instance = go.AddComponent<GameSession>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // Datos del jugador actual (solo en memoria)
    public int JugadorId { get; private set; }
    public string NombreJugador { get; private set; }
    public int PuntajeTotal { get; private set; }
    public int NivelMaximo { get; private set; }

    private bool _sesionActiva = false;
    public bool SesionActiva => _sesionActiva;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Inicializar sesión con datos del jugador desde la API
    /// </summary>
    public void IniciarSesion(int id, string nombre, int puntaje, int nivel)
    {
        JugadorId = id;
        NombreJugador = nombre;
        PuntajeTotal = puntaje;
        NivelMaximo = nivel;
        _sesionActiva = true;

        Debug.Log($"✅ Sesión iniciada: {nombre} (ID: {id}, Puntaje: {puntaje}, Nivel: {nivel})");
    }

    /// <summary>
    /// Actualizar puntaje después de completar un nivel
    /// </summary>
    public void ActualizarProgreso(int nuevoPuntaje, int nuevoNivel)
    {
        PuntajeTotal = nuevoPuntaje;
        NivelMaximo = nuevoNivel;
        Debug.Log($"📊 Progreso actualizado: {nuevoPuntaje} pts, Nivel máximo: {nuevoNivel}");
    }

    /// <summary>
    /// Cerrar sesión (opcional, se destruye automáticamente al cerrar el juego)
    /// </summary>
    public void CerrarSesion()
    {
        _sesionActiva = false;
        JugadorId = 0;
        NombreJugador = "";
        PuntajeTotal = 0;
        NivelMaximo = 1;
        Debug.Log("🚪 Sesión cerrada");
    }
}