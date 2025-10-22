
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextScene : MonoBehaviour
{
    public string nextSceneName;
    void OnTriggerEnter2D(Collider2D other)
    {
        var mario = other.GetComponent<MarioStateController>();
        if (mario != null)
        {
            Debug.Log("Change scene to " + nextSceneName);
            SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        }
    }
}