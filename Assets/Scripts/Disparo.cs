using UnityEngine;
using UnityEngine.InputSystem;

public class Disparo : MonoBehaviour
{

    [SerializeField] InputActionReference shoot;  // Player/Shoot (Button)
    [SerializeField] GameObject bulletPrefab;      // Prefab de la bala
    [SerializeField] Transform gunPoint;           // Punto de salida
    [SerializeField] float bulletSpeed = 15f;
    [SerializeField] float bulletLife = 10f;

    [SerializeField] SpriteRenderer playerSprite;  // para saber si está mirando a la izquierda
    private bool atacando;
    public Animator animator; // Animator del jugador


    void OnEnable() => shoot.action.Enable();
    void OnDisable() => shoot.action.Disable();

    void Update()
    {
        // Dispara si se presiona el botón configurado
        if (shoot.action.triggered)
        {
            Fire();
        }

        animator.SetBool("Atacando", atacando);
    }

    void Fire()
    {
        if (bulletPrefab == null || gunPoint == null) return;
        Atacando();

        // ¿Mirando a la izquierda?
        bool facingLeft = (playerSprite != null && playerSprite.flipX);
        if (playerSprite == null) facingLeft = transform.lossyScale.x < 0f;

        // Offset horizontal usando el gunPoint como referencia local,
        // pero SIN moverlo: convertimos a mundo con TransformPoint
        float baseOffsetX = Mathf.Abs(gunPoint.localPosition.x);
        if (baseOffsetX < 0.01f) baseOffsetX = 0.25f; // fallback si estaba en 0
        float localX = facingLeft ? -baseOffsetX : baseOffsetX;
        Vector3 localSpawn = new Vector3(localX, gunPoint.localPosition.y, 0f);

        // Posición final de spawn en MUNDO
        Vector3 spawnPos = transform.TransformPoint(localSpawn);

        // Dirección
        Vector2 dir = facingLeft ? Vector2.left : Vector2.right;

        // Instanciar y empujar
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = dir * bulletSpeed;

        // Ajuste visual opcional (flip de la bala)
        var s = bullet.transform.localScale;
        s.x = Mathf.Abs(s.x) * (facingLeft ? -1f : 1f);
        bullet.transform.localScale = s;

        Destroy(bullet, bulletLife);

      

    }

    public void Atacando()
    {
        atacando = true;
    }

    public void DesactivaAtaque()
    {
        atacando = false;
    }

}

