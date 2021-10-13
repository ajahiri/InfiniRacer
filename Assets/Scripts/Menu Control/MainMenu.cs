using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene("RaceArea01");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms 
    }
}
