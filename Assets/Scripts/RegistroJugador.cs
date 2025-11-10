using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistroJugador : MonoBehaviour
{
    [Header("UI Referencias")]
    public InputField inputNombre;
    public Text mensajeTexto;
    public Button botonRegistrar;

    [Header("Configuración")]
    private string apiUrl = "http://localhost:5288/api/jugador";
    private string escenaMenuPrincipal = "Inicio";

    void Start()
    {
        // Enfocar el input automáticamente
        if (inputNombre != null)
        {
            inputNombre.Select();
            inputNombre.ActivateInputField();
        }

        // Asegurar que el botón esté habilitado
        if (botonRegistrar != null)
            botonRegistrar.interactable = true;

        // Limpiar mensaje
        if (mensajeTexto != null)
            mensajeTexto.text = "";
    }

    public void OnClickRegistrar()
    {
        string nombre = inputNombre.text.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            mensajeTexto.text = "Por favor ingresá un nombre.";
            return;
        }

        if (nombre.Length < 3)
        {
            mensajeTexto.text = "El nombre debe tener al menos 3 caracteres.";
            return;
        }

        // Deshabilitar botón mientras se procesa
        if (botonRegistrar != null)
            botonRegistrar.interactable = false;

        mensajeTexto.text = "Buscando jugador...";
        StartCoroutine(BuscarOCrearJugador(nombre));
    }

    private IEnumerator BuscarOCrearJugador(string nombre)
    {
        string jsonData = $"{{\"nombreJugador\":\"{nombre}\"}}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log($"Respuesta de API: {json}");

                // Parsear respuesta
                JugadorRespuesta jugador = JsonUtility.FromJson<JugadorRespuesta>(json);

                // Guardar en sesión (solo memoria, NO PlayerPrefs)
                GameSession.Instance.IniciarSesion(
                    jugador.idJugador,
                    jugador.nombreJugador,
                    jugador.puntajeTotal,
                    jugador.nivelMaximoAlcanzado
                );

                // Mensaje de bienvenida según si es nuevo o existente
                if (jugador.puntajeTotal == 0)
                {
                    // Es un jugador nuevo (puntaje 0)
                    mensajeTexto.text = $"¡Bienvenido/a, {jugador.nombreJugador}!";
                    Debug.Log($"Nuevo jugador: {jugador.nombreJugador}");
                }
                else
                {
                    // Es un jugador existente con progreso
                    mensajeTexto.text = $"¡Hola de nuevo, {jugador.nombreJugador}! \nPuntaje: {jugador.puntajeTotal} pts";
                    Debug.Log($"Jugador existente: {jugador.nombreJugador} (Puntaje: {jugador.puntajeTotal}, Nivel: {jugador.nivelMaximoAlcanzado})");
                }

                // Esperar un momento antes de cambiar de escena
                yield return new WaitForSeconds(1.5f);

                // Ir al menú principal
                SceneManager.LoadScene(escenaMenuPrincipal);
            }
            else if (request.responseCode >= 500)
            {
                // Error 500+: Error del servidor
                mensajeTexto.text = "Error del servidor. Intentá más tarde.";
                Debug.LogError($"Error del servidor ({request.responseCode}): {request.error}");

                if (botonRegistrar != null)
                    botonRegistrar.interactable = true;
            }
        }
    }
}

[System.Serializable]
public class JugadorRespuesta
{
    public int idJugador;
    public string nombreJugador;
    public int puntajeTotal;
    public int nivelMaximoAlcanzado;
}
