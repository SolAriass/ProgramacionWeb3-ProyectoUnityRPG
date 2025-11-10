using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistroJugador : MonoBehaviour
{
    public InputField inputNombre;
    public Text mensajeTexto;
    private string apiUrl = "http://localhost:5288/api/jugador";

    public void OnClickRegistrar()
    {
        string nombre = inputNombre.text.Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            mensajeTexto.text = "Por favor ingresá un nombre.";
            return;
        }
        StartCoroutine(CrearJugador(nombre));
        SceneManager.LoadScene("Inicio");
    }

    private IEnumerator CrearJugador(string nombre)
    {
        // ✅ CAMBIO 1: Usar "NombreJugador" en lugar de "nombre"
        string jsonData = $"{{\"NombreJugador\":\"{nombre}\"}}";
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
                Debug.Log("Jugador creado: " + json);

                // Parseamos el JSON
                JugadorRespuesta jugador = JsonUtility.FromJson<JugadorRespuesta>(json);

                // Guardamos info local
                PlayerPrefs.SetInt("JugadorId", jugador.idJugador);
                PlayerPrefs.SetString("JugadorNombre", jugador.nombreJugador);
                PlayerPrefs.SetInt("PuntajeTotal", jugador.puntajeTotal);
                PlayerPrefs.SetInt("NivelMaximo", jugador.nivelMaximoAlcanzado);
                PlayerPrefs.Save();

                mensajeTexto.text = "¡Bienvenido/a, " + jugador.nombreJugador + "!";

                // Ejemplo: cargar siguiente escena
                // SceneManager.LoadScene("Nivel1");
            }
            else
            {
                mensajeTexto.text = "Error al registrar jugador.";
                Debug.LogError($"Error {request.responseCode}: {request.error}");

                // ✅ Esto te ayudará a ver qué está pasando
                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError($"Respuesta del servidor: {request.downloadHandler.text}");
                }
            }
        }
    }
}

// ✅ CAMBIO 2: Actualizar la clase para que coincida con tu JugadorResponse
[System.Serializable]
public class JugadorRespuesta
{
    public int idJugador;           // Era "id", ahora "idJugador"
    public string nombreJugador;
    public int puntajeTotal;
    public int nivelMaximoAlcanzado;
}
