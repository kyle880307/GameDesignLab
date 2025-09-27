

using UnityEngine;

public class QB : MonoBehaviour
{
    public GameObject coinObject;        // The coin prefab
    public Transform coinSpawnPoint;     // Spawn position (above box)
    public Sprite usedBoxSprite;         // Sprite after used
    public AudioClip coinSound;
    public float bounceForce = 5f;

    public SpringJoint2D springJoint;

    public float springEnableTime = 0.25f;

    private bool used = false;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (used) return; // Already used, do nothing

        // Default to freezing all constraints to prevent any movement from collisions.
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if player hit from BELOW
            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;

            // normal.y > 0.5f means the player hit from the bottom
            if (normal.y > 0.5f)
            {
                // If the condition is met, unfreeze constraints and activate the box
                rb.constraints = RigidbodyConstraints2D.None;
                ActivateBox();
            }
        }
    }

    void ActivateBox()
    {
        used = true;

        // Play the "used" animation or bounce animation
        GetComponent<Animator>().SetTrigger("Bounce");

        // Swap to used sprite (if provided)
        if (sr != null && usedBoxSprite != null)
        {
            sr.sprite = usedBoxSprite;
        }

        // Disable the animator so it stays on the used sprite
        GetComponent<Animator>().enabled = false;

        // Spawn coin and trigger its animation
        if (coinObject != null)
        {
            GameObject coin = Instantiate(coinObject, coinSpawnPoint.position, Quaternion.identity);
            Coin coinScript = coin.GetComponent<Coin>();
            if (coinScript != null)
            {
                // This launch method should trigger an animation that goes up and comes back down
                coinScript.Launch();
            }
        }
    }
}


