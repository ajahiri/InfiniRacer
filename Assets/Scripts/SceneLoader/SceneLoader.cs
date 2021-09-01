using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void StartScreen()
    {
        SceneManager.LoadScene("StartScreen");
    }
   public void Unpause() {
         GetComponent<Button>().onClick.AddListener(TogglePause);
     }
     public void TogglePause() {
         Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;        
     }
    public void OptionScreen()
    {
        SceneManager.LoadScene("OptionScreen");
    }
    public void PauseScreen()
    {
        SceneManager.LoadScene("PauseScreen");
    }
    public void BackToGame()
    {
        SceneManager.LoadScene("RaceArea01");
    }
}

