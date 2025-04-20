using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuDataPasser : MonoBehaviour
{
    public string playerName;
        
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void StartGame()
    {
        // Load the game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}