using UnityEngine;

public class Laoding : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Load the Lab4 scene after 3 seconds
        Invoke("LoadLab4Scene", 3f);
    }

    private void LoadLab4Scene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lab4");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
