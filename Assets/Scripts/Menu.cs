using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
  
    public void Jugar()
    {
        SceneManager.LoadScene("Escena1");
    }

    public void Salir()
    {
        Application.Quit();
    }


}
