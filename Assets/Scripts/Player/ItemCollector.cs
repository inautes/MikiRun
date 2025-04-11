using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private AudioManager audioManager;
    private GameManager gameManager;
    
    void Start()
    {
        audioManager = AudioManager.Instance;
        gameManager = GameManager.Instance;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            if (audioManager != null)
                audioManager.PlayCollectSound();
            
            if (gameManager != null)
                gameManager.AddScore(10);
            
            Destroy(other.gameObject);
        }
    }
}
