using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnBack()
    {
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Back");
    }

    public void OnLava()
    {
        SceneManager.LoadScene("Lava");
        Debug.Log("Lava");
    }

    public void OnWater()
    {
        SceneManager.LoadScene("WaterIsland");
        Debug.Log("Water");
    }

    public void OnFloat()
    {
        SceneManager.LoadScene("FloatingIsland");
        Debug.Log("Float");
    }

}
