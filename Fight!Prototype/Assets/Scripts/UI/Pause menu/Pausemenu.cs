using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausemenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnReturn()
    {
        Debug.Log("Return");
        SceneManager.LoadScene("MainMenu");
    }
}
