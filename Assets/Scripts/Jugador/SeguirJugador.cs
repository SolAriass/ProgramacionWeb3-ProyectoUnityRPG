using UnityEngine;

public class SeguirJugador : MonoBehaviour
{

        public Transform objetivo; // El jugador
        public float suavizado = 5f; // Qué tan suave se mueve la cámara

        private Vector3 offset; // Diferencia inicial entre cámara y jugador

        void Start()
        {
            if (objetivo != null)
                offset = transform.position - objetivo.position;
        }

        void LateUpdate()
        {
            if (objetivo != null)
            {
                Vector3 posicionDeseada = objetivo.position + offset;
                transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
            }
        }
        // Update is called once per frame
        void Update()
    {
        
    }
}
