using TMPro;
using UnityEngine;
using Unity.MLAgents.Policies;

public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] float moveSpeed = 6;
    [SerializeField] int damageTick = 5; // Damage taken per hit

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
    
    public void SetMovement(Vector2 newMovement)
    {
        movement = newMovement.normalized;
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void Update()
    {
        if (dead || !GameManager.instance.IsGameRunning())
        {
            movement = Vector2.zero;
            anim.SetFloat("velocity", 0);
            return;
        }

        // Only read player Input when in Heuristic mode or no Agent present
        var bp = GetComponent<BehaviorParameters>();
        // Only fallback to manual input if BehaviorType is HeuristicOnly
        if (bp != null && bp.BehaviorType == BehaviorType.HeuristicOnly)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            movement = new Vector2(moveHorizontal, moveVertical).normalized;
        }
        // Otherwise keep movement set by Agent.SetMovement()

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
            Hit(damageTick);
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
