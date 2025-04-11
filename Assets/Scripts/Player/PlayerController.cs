using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float gravity = 20f;
    
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private bool isGrounded;
    private Animator animator;
    private bool isDead = false;
    
    private GameManager gameManager;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        currentHealth = maxHealth;
        
        if (groundCheck == null)
        {
            Debug.LogWarning("Ground check transform not assigned to player!");
            groundCheck = transform;
        }
    }
    
    void Update()
    {
        if (isDead) return;
        
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        
        if (isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -2f; // Small negative value to keep player grounded
        }
        
        float horizontalInput = Input.GetAxis("Horizontal");
        
        moveDirection.x = horizontalInput * moveSpeed;
        
        moveDirection.z = moveSpeed;
        
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            moveDirection.y = jumpForce;
            if (animator != null)
                animator.SetTrigger("Jump");
        }
        
        moveDirection.y -= gravity * Time.deltaTime;
        
        controller.Move(moveDirection * Time.deltaTime);
        
        if (animator != null)
        {
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("HorizontalSpeed", Mathf.Abs(horizontalInput));
        }
    }
    
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        if (animator != null)
            animator.SetTrigger("Hit");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        isDead = true;
        
        if (animator != null)
            animator.SetTrigger("Die");
        
        if (gameManager != null)
            gameManager.GameOver();
    }
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacle") || hit.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
            
            moveDirection.z = -moveSpeed * 0.5f;
        }
    }
    
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public float GetDistanceTraveled()
    {
        return transform.position.z;
    }
}
