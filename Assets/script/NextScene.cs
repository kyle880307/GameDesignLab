
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NextScene : MonoBehaviour
{
    public string nextSceneName;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Change scene to " + nextSceneName);
            SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        }
    }
    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}