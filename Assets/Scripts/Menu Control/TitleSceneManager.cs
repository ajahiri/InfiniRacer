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
        FindObjectOfType<AudioManager>().Play("Start sound");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }
    public void SelectCar()
    {
        Debug.Log("Selecting car");
        SceneManager.LoadScene("CarSelectionScene");
  
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms

    }

    public void ViewOptions()
    {
        Debug.Log("Viewing options...");
        SceneManager.LoadScene("OptionsScene");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }
}
