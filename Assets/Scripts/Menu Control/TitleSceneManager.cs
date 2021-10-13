using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    public void StartGame()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("Starting game...");
        SceneManager.LoadScene("RaceArea01");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }
    public void SelectCar()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("Selecting car");
        SceneManager.LoadScene("CarSelectionScene");
  
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms

    }

    public void ViewOptions()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("Viewing options...");
        SceneManager.LoadScene("OptionsScene");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }
}
