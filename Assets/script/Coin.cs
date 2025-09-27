using UnityEngine;

public class Coin : MonoBehaviour
{
    public float riseDistance = 1f;
    public float riseTime = 0.3f;

    public void Launch()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * riseDistance;

        StartCoroutine(MoveCoin(startPos, targetPos));
    }

    private System.Collections.IEnumerator MoveCoin(Vector3 start, Vector3 end)
    {
        float elapsed = 0f;

        while (elapsed < riseTime)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / riseTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        // Return coin inside box
        elapsed = 0f;
        while (elapsed < riseTime)
        {
            transform.position = Vector3.Lerp(end, start, elapsed / riseTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Or set inactive
    }
}
