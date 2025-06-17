using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] float moveSpeed = 6;
    
    Animator anim;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    
    int maxHealth = 100;
    int currentHealth;
    bool dead = false;
    
    float moveHorizontal, moveVertical;
    Vector2 movement;
    
    int facingDirection = 1; // 1 = right, -1 = left
    
    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        currentHealth = maxHealth;
        healthText.text = maxHealth.ToString();
    }

    private void Update()
    {
        if (dead || !GameManager.instance.IsGameRunning())
        {
            movement = Vector2.zero;
            anim.SetFloat("velocity", 0);
            return;
        }
        
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        
        movement = new Vector2(moveHorizontal, moveVertical).normalized;
        
        anim.SetFloat("velocity", movement.magnitude);

        if (movement.x != 0)
        {
            facingDirection = movement.x > 0 ? 1 : -1;
        }
        
        transform.localScale = new Vector2(facingDirection, 1);
    }

    private void FixedUpdate()
    {
        if (!GameManager.instance.IsGameRunning())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        rb.linearVelocity = movement * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
        {
            Hit(20);
        }
    }

    void Hit(int damage)
    {
        anim.SetTrigger("hit");
        currentHealth -= damage;
        healthText.text = Mathf.Clamp(currentHealth, 0, maxHealth).ToString();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        dead = true;
        GameManager.instance.GameOver();
        rb.simulated = false; // Disable physics interaction
        GunManager.Instance.DisableAllGuns(); // Disable all guns
        gameObject.SetActive(false); // Completely deactivate the player object
    }
}
