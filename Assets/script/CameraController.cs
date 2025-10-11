using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{

    public Transform player; // Mario's Transform
    public Transform endLimit; // GameObject that indicates end of map
    private float offset; // initial x-offset between camera and Mario
    private float startX; // smallest x-coordinate of the Camera
    private float endX; // largest x-coordinate of the camera
    private float viewportHalfWidth;

    void Start()
    {
        InitializeCamera();
        // Subscribe to scene change to reinitialize camera
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reinitialize camera when scene changes
        StartCoroutine(ReinitializeCameraAfterFrame());
    }

    IEnumerator ReinitializeCameraAfterFrame()
    {
        // Wait one frame to ensure player position is set
        yield return null;
        InitializeCamera();
    }

    void InitializeCamera()
    {
        // get coordinate of the bottomleft of the viewport
        // z doesn't matter since the camera is orthographic
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)); // the z-component is the distance of the resulting plane from the camera 
        viewportHalfWidth = Mathf.Abs(bottomLeft.x - this.transform.position.x);
        offset = this.transform.position.x - player.position.x;
        startX = this.transform.position.x;
        endX = endLimit.transform.position.x - viewportHalfWidth;
    }

    void Update()
    {
        float desiredX = player.position.x + offset;
        float desiredY = player.position.y + 2f; // follow Y as well

        // clamp X movement within startX and endX
        if (desiredX > startX && desiredX < endX)
            this.transform.position = new Vector3(desiredX, desiredY, this.transform.position.z);
        else
            this.transform.position = new Vector3(this.transform.position.x, desiredY, this.transform.position.z);
    }
}
