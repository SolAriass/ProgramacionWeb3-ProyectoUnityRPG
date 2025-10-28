using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // arrastra el jugador en el Inspector
    public Vector3 offset;   // distancia de la cámara respecto al jugador
    public float smoothSpeed = 0.125f; // velocidad de suavizado

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

