using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[System.Serializable]
public class RankingItem
{
    public int posicion;
    public string nombreJugador;
    public int puntajeTotal;
    public int nivelMaximoAlcanzado;
}

public class RankingManager : MonoBehaviour
{
    [Header("Configuración API")]
    private const string API_URL = "http://localhost:5288/api";

    [Header("Referencias UI")]
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject rankingItemPrefab;
    [SerializeField] private TextMeshProUGUI mensajeError;

    [Header("Colores Rick & Morty")]
    [SerializeField] private Color colorPrimero = new Color(1f, 0.84f, 0f, 1f);
    [SerializeField] private Color colorSegundo = new Color(0.75f, 0.75f, 0.75f, 1f);
    [SerializeField] private Color colorTercero = new Color(0.8f, 0.5f, 0.2f, 1f);
    [SerializeField] private Color colorNormal = new Color(1f, 1f, 1f, 1f);

    void Start()
    {
        StartCoroutine(ObtenerRanking());
    }

    IEnumerator ObtenerRanking()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{API_URL}/jugador/ranking"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;
                Debug.Log($"✅ Ranking recibido: {json}");

                string jsonWrapper = "{\"items\":" + json + "}";
                RankingListWrapper wrapper = JsonUtility.FromJson<RankingListWrapper>(jsonWrapper);

                if (wrapper.items != null && wrapper.items.Length > 0)
                {
                    MostrarRanking(wrapper.items);
                }
                else
                {
                    MostrarError("No hay jugadores en el ranking todavía.");
                }
            }
            else
            {
                Debug.LogError($"❌ Error al obtener ranking: {www.error}");
                MostrarError("Error al cargar el ranking. Verifica que la API esté activa.");
            }
        }
    }

    void MostrarRanking(RankingItem[] items)
    {
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (RankingItem item in items)
        {
            GameObject fila = Instantiate(rankingItemPrefab, contentContainer);

            TextMeshProUGUI textPosicion = fila.transform.Find("TextPosicion/Texto").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI textNombre = fila.transform.Find("TextNombre/Texto").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI textPuntaje = fila.transform.Find("TextPuntaje/Texto").GetComponent<TextMeshProUGUI>();

            if (textPosicion == null || textNombre == null || textPuntaje == null)
            {
                Debug.LogError("⚠️ No se encontraron todos los componentes TextMeshProUGUI en el prefab.");
                Destroy(fila);
                continue;
            }

            textPosicion.text = $"#{item.posicion}";
            textNombre.text = item.nombreJugador;
            textPuntaje.text = $"{item.puntajeTotal} pts";

            Color colorPosicion = colorNormal;
            switch (item.posicion)
            {
                case 1:
                    colorPosicion = colorPrimero;
                    break;
                case 2:
                    colorPosicion = colorSegundo;
                    break;
                case 3:
                    colorPosicion = colorTercero;
                    break;
            }

            textPosicion.color = colorPosicion;
            textNombre.color = colorPosicion;
        }

        Debug.Log($"✅ Ranking cargado: {items.Length} jugadores");
    }

    void MostrarError(string mensaje)
    {
        if (mensajeError != null)
        {
            mensajeError.text = mensaje;
            mensajeError.gameObject.SetActive(true);
        }
        Debug.LogWarning($"⚠️ {mensaje}");
    }

    // Método para el botón "Volver"
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("Inicio");
    }
}

[System.Serializable]
public class RankingListWrapper
{
    public RankingItem[] items;
}
