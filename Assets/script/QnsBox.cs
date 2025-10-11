using UnityEngine;
using System.Collections;

public class QnsBox : MonoBehaviour
{
    [Header("Game Constants")]
    public GameConstants gameConstants;

    public GameObject coinObject;
    public GameObject starmanObject; // Add starman prefab reference
    public Transform coinSpawnPoint;
    public AudioSource coinAudioSource;
    public Sprite usedBoxSprite;
    private Sprite originalBoxSprite; 
    private SpriteRenderer sr;
    private Coin coinScript;
    private Starman starmanScript; // Add starman reference
    private bool hasBeenUsed = false; // Track if box has been hit

    private float bounceHeight;
    private float bounceDuration;

    void Start()
    {
        // Initialize values from GameConstants
        if (gameConstants != null)
        {
            bounceHeight = gameConstants.boxBounceHeight;
            bounceDuration = gameConstants.boxBounceDuration;
        }
        else
        {
            Debug.LogWarning("GameConstants not assigned to QnsBox!");
            // Fallback values
            bounceHeight = 1f;
            bounceDuration = 0.2f;
        }

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalBoxSprite = sr.sprite;
        }
        ResetBox();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasBeenUsed)
        {
            var normal = collision.contacts[0].normal;
            if (normal.y > 0) // Hit from below
            {
                Debug.Log("QnsBox hit from below by player!");
                // Spawn item when hit
                SpawnItem();
                hasBeenUsed = true; // Mark as used
                
                if (coinAudioSource != null) coinAudioSource.Play();
                StartCoroutine(BounceAndChangeSprite());
            }
        } 
    }

    private void SpawnItem()
    {
        Debug.Log("SpawnItem() called");
        Debug.Log($"coinObject: {(coinObject != null ? coinObject.name : "null")}");
        Debug.Log($"starmanObject: {(starmanObject != null ? starmanObject.name : "null")}");
        
        // Prioritize coin over starman if both are assigned
        if (coinObject != null)
        {
            Debug.Log("Spawning coin...");
            GameObject coin = Instantiate(coinObject, coinSpawnPoint.position, Quaternion.identity);
            coinScript = coin.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.Launch();
                Debug.Log("Coin launched successfully!");
            }
            else
            {
                Debug.LogError("Coin script not found on instantiated coin!");
            }
        }
        else if (starmanObject != null)
        {
            Debug.Log("Spawning starman...");
            GameObject starman = Instantiate(starmanObject, coinSpawnPoint.position, Quaternion.identity);
            starmanScript = starman.GetComponent<Starman>();
            if (starmanScript != null)
            {
                starmanScript.Launch();
                Debug.Log("Starman launched successfully!");
            }
            else
            {
                Debug.LogError("Starman script not found on instantiated starman!");
            }
        }
        else
        {
            Debug.LogWarning("No coin or starman object assigned to QnsBox!");
        }
    }

    private IEnumerator BounceAndChangeSprite()
    {
        Vector3 startPos = transform.position;
        Vector3 peakPos = startPos + Vector3.up * bounceHeight;

        float halfDuration = bounceDuration / 2f;
        float timer = 0f;

        // Move up
        while (timer < halfDuration)
        {
            transform.position = Vector3.Lerp(startPos, peakPos, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = peakPos;

        // Move down
        timer = 0f;
        while (timer < halfDuration)
        {
            transform.position = Vector3.Lerp(peakPos, startPos, timer / halfDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos;

        // Change sprite after bounce only
        if (sr != null && usedBoxSprite != null)
        {
            GetComponent<Animator>().enabled = false;
            sr.sprite = usedBoxSprite;
        }

    }

    public void ResetBox()
    {
        GetComponent<Animator>().enabled = true;

        if (sr != null && originalBoxSprite != null)
        {
            sr.sprite = originalBoxSprite;
        }

        // Clean up existing coin
        if (coinScript != null)
        {
            Destroy(coinScript.gameObject);
            coinScript = null;
        }

        // Clean up existing starman
        if (starmanScript != null)
        {
            Destroy(starmanScript.gameObject);
            starmanScript = null;
        }

        // Reset the used state
        hasBeenUsed = false;

        // Don't spawn anything at start - items will be spawned when box is hit
    }
}
