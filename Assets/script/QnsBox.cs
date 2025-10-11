using UnityEngine;
using System.Collections;

public class QnsBox : MonoBehaviour
{
    public GameObject coinObject;
    public Transform coinSpawnPoint;
    public AudioSource coinAudioSource;
    public Sprite usedBoxSprite;
    private Sprite originalBoxSprite; 
    private SpriteRenderer sr;
    private Coin coinScript;

    public float bounceHeight = 1f; // how high the box bounces
    public float bounceDuration = 0.2f; // total up and down duration

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            originalBoxSprite = sr.sprite;
        }
        ResetBox();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && coinScript != null)
        {
            var normal = collision.contacts[0].normal;
            if (normal.y > 0)
            {
                if (coinScript != null)
                {
                    coinScript.Launch();
                    if (coinAudioSource != null) coinAudioSource.Play();

                    StartCoroutine(BounceAndChangeSprite());
                    coinScript = null; // prevent multiple uses
                }
            }

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

        if (coinScript != null)
        {
            Destroy(coinScript.gameObject);
        }
        GameObject coin = Instantiate(coinObject, coinSpawnPoint.position, Quaternion.identity);
        coinScript = coin.GetComponent<Coin>();
    }
}
