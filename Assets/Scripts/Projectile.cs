using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed = 18f;

    private void FixedUpdate()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            Destroy(this.gameObject);
            enemy.Hit(15);
        }
    }
}
