# 🎮 Rick Adventure API

API REST para gestionar progreso de jugadores en un juego Unity de 3 niveles.

---

## 📖 ¿Qué hace esta API?

Esta API es el **backend** de un juego de Unity. Solo se encarga de:
1. ✅ Registrar jugadores
2. ✅ Guardar resultados de partidas
3. ✅ Mostrar ranking (top 10)

**⚠️ La lógica del juego (niveles, enemigos, combate) está en Unity, NO en la API.**

---

## 🔄 Flujo Unity ↔ API (Paso a Paso)

### **1️⃣ Inicio del Juego (Crear Jugador)**
```
Unity: Usuario ingresa su nombre → "Rick"
Unity: POST /api/jugador {"nombreJugador": "Rick"}
API:   Responde con {idJugador: 5, puntajeTotal: 0, nivelMaximoAlcanzado: 1}
Unity: Guarda idJugador=5 en PlayerPrefs para usarlo después
```

### **2️⃣ Jugando (Todo en Unity)**
```
Unity: El jugador juega el nivel 1 (todo pasa en Unity)
       - Mata enemigos, suma puntos, completa o pierde
       - Los enemigos, mapas, lógica están hardcodeados en Unity
```

### **3️⃣ Termina el Nivel (Guardar Partida)**
```
Unity: POST /api/partida {idJugador: 5, idNivel: 1, puntaje: 350, completado: true}
API:   - Guarda la partida en la BD
       - Suma 350 al puntaje total del jugador
       - Si completado=true → Desbloquea nivel 2
       - Responde con {exito: true, nuevoPuntajeTotal: 350, nivelDesbloqueado: 2}
Unity: Muestra mensaje "¡Nivel 2 desbloqueado!" y habilita el siguiente nivel
```

### **4️⃣ Ver Ranking (Opcional)**
```
Unity: GET /api/jugador/ranking
API:   Devuelve top 10 jugadores ordenados por puntaje
Unity: Muestra la tabla de ranking en pantalla
```

---

## 🚀 Endpoints (Solo 3)

| Método | Endpoint | Descripción | Cuándo llamarlo |
|--------|----------|-------------|----------------|
| `POST` | `/api/jugador` | Crear jugador | Al iniciar el juego |
| `POST` | `/api/partida` | Guardar resultado | Al terminar cada nivel |
| `GET` | `/api/jugador/ranking` | Ver top 10 | En pantalla de ranking |

---

## 📦 Request/Response (Ejemplos)

### **POST /api/jugador** (Crear Jugador)
**Request:**
```json
POST http://localhost:5000/api/jugador
Content-Type: application/json

{
  "nombreJugador": "Rick"
}
```
**Response:**
```json
{
  "idJugador": 5,
  "nombreJugador": "Rick",
  "puntajeTotal": 0,
  "nivelMaximoAlcanzado": 1
}
```
**⚠️ Importante:** Guarda `idJugador` en Unity (PlayerPrefs) para usarlo en las siguientes llamadas.

---

### **POST /api/partida** (Guardar Partida)
**Request:**
```json
POST http://localhost:5000/api/partida
Content-Type: application/json

{
  "idJugador": 5,
  "idNivel": 1,
  "puntaje": 350,
  "completado": true
}
```
**Response:**
```json
{
  "exito": true,
  "mensaje": "¡Nivel 1 completado! Nivel 2 desbloqueado.",
  "nuevoPuntajeTotal": 350,
  "nivelDesbloqueado": 2
}
```

**Si el jugador NO completa el nivel:**
```json
{
  "idJugador": 5,
  "idNivel": 2,
  "puntaje": 150,
  "completado": false
}
```
Response:
```json
{
  "exito": true,
  "mensaje": "Partida guardada. Sigue intentando el nivel 2.",
  "nuevoPuntajeTotal": 500,
  "nivelDesbloqueado": 2  // No avanza porque no completó
}
```

---

### **GET /api/jugador/ranking** (Top 10)
**Request:**
```json
GET http://localhost:5000/api/jugador/ranking
```
**Response:**
```json
[
  {
    "posicion": 1,
    "nombreJugador": "Rick",
    "puntajeTotal": 1250,
    "nivelMaximoAlcanzado": 3
  },
  {
    "posicion": 2,
    "nombreJugador": "Morty",
    "puntajeTotal": 890,
    "nivelMaximoAlcanzado": 2
  },
  {
    "posicion": 3,
    "nombreJugador": "Summer",
    "puntajeTotal": 650,
    "nivelMaximoAlcanzado": 1
  }
]
```

---

## 🎯 Integración con Unity (Código C# para Unity)

### **1. Crear Jugador (Script en Unity)**
```csharp
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class JugadorRequest
{
    public string nombreJugador;
}

[System.Serializable]
public class JugadorResponse
{
    public int idJugador;
    public string nombreJugador;
    public int puntajeTotal;
    public int nivelMaximoAlcanzado;
}

public class APIManager : MonoBehaviour
{
    private const string API_URL = "http://localhost:5000/api";
    
    public IEnumerator CrearJugador(string nombre)
    {
        JugadorRequest request = new JugadorRequest { nombreJugador = nombre };
        string jsonData = JsonUtility.ToJson(request);
        
        using (UnityWebRequest www = UnityWebRequest.Post($"{API_URL}/jugador", jsonData, "application/json"))
        {
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                JugadorResponse response = JsonUtility.FromJson<JugadorResponse>(www.downloadHandler.text);
                
                // Guardar idJugador en PlayerPrefs
                PlayerPrefs.SetInt("idJugador", response.idJugador);
                PlayerPrefs.SetString("nombreJugador", response.nombreJugador);
                PlayerPrefs.Save();
                
                Debug.Log($"Jugador creado: {response.nombreJugador} (ID: {response.idJugador})");
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
            }
        }
    }
}
```

### **2. Guardar Partida (Al terminar nivel)**
```csharp
[System.Serializable]
public class PartidaRequest
{
    public int idJugador;
    public int idNivel;
    public int puntaje;
    public bool completado;
}

[System.Serializable]
public class PartidaResponse
{
    public bool exito;
    public string mensaje;
    public int nuevoPuntajeTotal;
    public int nivelDesbloqueado;
}

public IEnumerator GuardarPartida(int idNivel, int puntaje, bool completado)
{
    int idJugador = PlayerPrefs.GetInt("idJugador");
    
    PartidaRequest request = new PartidaRequest
    {
        idJugador = idJugador,
        idNivel = idNivel,
        puntaje = puntaje,
        completado = completado
    };
    
    string jsonData = JsonUtility.ToJson(request);
    
    using (UnityWebRequest www = UnityWebRequest.Post($"{API_URL}/partida", jsonData, "application/json"))
    {
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success)
        {
            PartidaResponse response = JsonUtility.FromJson<PartidaResponse>(www.downloadHandler.text);
            
            Debug.Log(response.mensaje);
            Debug.Log($"Puntaje total: {response.nuevoPuntajeTotal}");
            Debug.Log($"Nivel desbloqueado: {response.nivelDesbloqueado}");
            
            // Actualizar UI en Unity
            // MostrarMensaje(response.mensaje);
            // ActualizarPuntaje(response.nuevoPuntajeTotal);
        }
        else
        {
            Debug.LogError($"Error: {www.error}");
        }
    }
}
```

### **3. Obtener Ranking**
```csharp
[System.Serializable]
public class RankingResponse
{
    public int posicion;
    public string nombreJugador;
    public int puntajeTotal;
    public int nivelMaximoAlcanzado;
}

[System.Serializable]
public class RankingListWrapper
{
    public RankingResponse[] items;
}

public IEnumerator ObtenerRanking()
{
    using (UnityWebRequest www = UnityWebRequest.Get($"{API_URL}/jugador/ranking"))
    {
        yield return www.SendWebRequest();
        
        if (www.result == UnityWebRequest.Result.Success)
        {
            // Unity JsonUtility no soporta arrays directamente, usar wrapper
            string json = "{\"items\":" + www.downloadHandler.text + "}";
            RankingListWrapper wrapper = JsonUtility.FromJson<RankingListWrapper>(json);
            
            foreach (var jugador in wrapper.items)
            {
                Debug.Log($"{jugador.posicion}. {jugador.nombreJugador} - {jugador.puntajeTotal} pts");
            }
            
            // Mostrar ranking en UI
            // MostrarRanking(wrapper.items);
        }
        else
        {
            Debug.LogError($"Error: {www.error}");
        }
    }
}
```

### **4. Ejemplo de uso en Unity**
```csharp
public class GameManager : MonoBehaviour
{
    private APIManager apiManager;
    
    void Start()
    {
        apiManager = GetComponent<APIManager>();
    }
    
    // Llamar al iniciar el juego
    public void IniciarJuego(string nombreJugador)
    {
        StartCoroutine(apiManager.CrearJugador(nombreJugador));
    }
    
    // Llamar al terminar un nivel
    public void NivelCompletado(int nivel, int puntaje, bool completado)
    {
        StartCoroutine(apiManager.GuardarPartida(nivel, puntaje, completado));
    }
    
    // Llamar al abrir pantalla de ranking
    public void MostrarRanking()
    {
        StartCoroutine(apiManager.ObtenerRanking());
    }
}
```

---

## ⚙️ Configuración e Instalación

### **Configurar Base de Datos**

** Ejecutar el script de creación e insert de tablas (archivo 01-CrearBD.sql)**

### **Configurar Connection String**

Editar `appsettings.json` en el proyecto `RickAdventureAPI`:
```json
{
  "ConnectionStrings": {
    "RickAdventureDB": "Server=.\\SQLEXPRESS;Database=RickAdventureGame;Trusted_Connection=True;TrustServerCertificate=true"
  }
}
```
El nombre de SQLEXPRESS puede variar según tu instalación.

### **4. Ejecutar la API**

## 🧪 Probar con Postman

---

## 📝 Notas Importantes

- ✅ **Unity NO necesita consultar niveles ni enemigos** - están hardcodeados en el juego
- ✅ **La API solo persiste progreso** - no maneja lógica de gameplay
- ✅ **Cada partida se guarda** - incluso si el jugador pierde (para estadísticas)
- ✅ **El puntaje SIEMPRE se acumula** - aunque no complete el nivel
- ✅ **Solo se desbloquea el siguiente nivel si `completado: true`**
- ⚠️ **CORS habilitado para Unity** - `AllowAnyOrigin()` (cambiar en producción)