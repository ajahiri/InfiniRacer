using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameOver : MonoBehaviour
{
    // for scoring display 
    public Text score;

    public void RestartGame()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        //Time.timeScale = 1f;
        SceneManager.LoadScene("RaceArea01");
        FindObjectOfType<AudioManager>().Stop("Game Over");
        FindObjectOfType<AudioManager>().Play("CarEngine");
        FindObjectOfType<AudioManager>().Play("Start sound");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
        Debug.Log("Restarting game...");
    }
    public void LoadMenu()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        //Time.timeScale = 1f;
        FindObjectOfType<AudioManager>().Stop("Game Over");
        SceneManager.LoadScene("TitleScene");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
        Debug.Log("Loading menu...");
    }
}
