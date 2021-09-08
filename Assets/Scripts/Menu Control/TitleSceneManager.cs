using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("Starting game...");
        SceneManager.LoadScene("RaceArea01");
    }

    public void ViewOptions()
    {
        Debug.Log("Viewing options...");
        SceneManager.LoadScene("OptionsScene");
    }
}
