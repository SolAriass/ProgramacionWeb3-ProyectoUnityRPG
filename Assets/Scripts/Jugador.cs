using UnityEngine;

public class Jugador : MonoBehaviour
{
    public int vida = 100;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void RecibirDa�o(int cantidad)
    {
        vida -= cantidad;
        Debug.Log("Vida del jugador: " + vida);

        // Activar animaci�n de recibir golpe
        anim.SetTrigger("Hit");

        if (vida <= 0)
        {
            Debug.Log("Jugador muerto!");
        }
    }
}
