using UnityEngine;
using UnityEngine.InputSystem;

public class Disparo : MonoBehaviour
{

    [SerializeField] InputActionReference shoot;  // Player/Shoot (Button)
    [SerializeField] GameObject bulletPrefab;      // Prefab de la bala
    [SerializeField] Transform gunPoint;           // Punto de salida
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float bulletLife = 5f;

    [SerializeField] SpriteRenderer playerSprite;  // para saber si está mirando a la izquierda

    void OnEnable() => shoot.action.Enable();
    void OnDisable() => shoot.action.Disable();

    void Update()
    {
        // Dispara si se presiona el botón configurado
        if (shoot.action.triggered)
        {
            Fire();
        }
    }

    void Fire()
    {
        if (bulletPrefab == null || gunPoint == null) return;

        // ¿Mirando a la izquierda?
        bool facingLeft = (playerSprite != null && playerSprite.flipX);
        if (playerSprite == null) facingLeft = transform.lossyScale.x < 0f;

        // Asegurar que el GunPoint esté al lado correcto
        var lp = gunPoint.localPosition;
        lp.x = Mathf.Abs(lp.x) * (facingLeft ? -1f : 1f);
        gunPoint.localPosition = lp;

        // Instanciar bala en el gunPoint, levemente adelantada
        Vector2 dir = facingLeft ? Vector2.left : Vector2.right;
        GameObject bullet = Instantiate(bulletPrefab, gunPoint.position + (Vector3)(dir * 0.05f), Quaternion.identity);

        // Dar velocidad (API 6.x)
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir * bulletSpeed;

        // Ajuste visual opcional (que la bala “mire” hacia el lado correcto)
        var s = bullet.transform.localScale;
        s.x = Mathf.Abs(s.x) * (facingLeft ? -1f : 1f);
        bullet.transform.localScale = s;

        Destroy(bullet, bulletLife);
    }


}

