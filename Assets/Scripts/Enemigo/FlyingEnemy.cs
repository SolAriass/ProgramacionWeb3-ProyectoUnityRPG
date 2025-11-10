using UnityEngine;

public class FlyingEnemy: MonoBehaviour
{


    Rigidbody2D enemy;

    private void Start()
    {
        enemy = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        enemy.velocity = new Vector2(-1, 0);
    }

}