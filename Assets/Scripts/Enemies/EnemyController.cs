using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public bool moveHorizontally = false;
    public float horizontalDistance = 2f;
    public float horizontalSpeed = 1f;
    
    [Header("Attack Settings")]
    public bool canAttack = false;
    public float attackRange = 1.5f;
    public int attackDamage = 1;
    public float attackCooldown = 2f;
    
    private Transform playerTransform;
    private Vector3 startPosition;
    private float horizontalOffset = 0f;
    private float lastAttackTime = 0f;
    private Animator animator;
    private GameManager gameManager;
    
    void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
        startPosition = transform.position;
        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
    }
    
    void Update()
    {
        if (playerTransform == null || gameManager == null) return;
        
        float gameSpeed = gameManager.GetCurrentGameSpeed();
        transform.Translate(Vector3.back * gameSpeed * Time.deltaTime);
        
        if (moveHorizontally)
        {
            horizontalOffset = Mathf.Sin(Time.time * horizontalSpeed) * horizontalDistance;
            Vector3 newPosition = startPosition;
            newPosition.x += horizontalOffset;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        
        if (canAttack && playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
        
        if (animator != null)
        {
            animator.SetFloat("MoveSpeed", moveSpeed);
        }
        
        if (transform.position.z < playerTransform.position.z - 20f)
        {
            Destroy(gameObject);
        }
    }
    
    void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackRange)
        {
            PlayerController player = playerTransform.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
        
        lastAttackTime = Time.time;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(attackDamage);
            }
        }
    }
}
