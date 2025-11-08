using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
  
    public void Jugar()
    {
        SceneManager.LoadScene("Nivel-1");
    }

    public void Salir()
    {
        Application.Quit();
    }


}
