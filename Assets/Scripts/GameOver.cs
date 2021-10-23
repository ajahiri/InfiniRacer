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
        SceneManager.LoadScene("AttentionScreen");
        FindObjectOfType<AudioManager>().Stop("Game Over");
        FindObjectOfType<AudioManager>().Stop("BIIIIIIGBOOOOM");
        Debug.Log("Restarting game...");
    }
    public void LoadMenu()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        //Time.timeScale = 1f;
        FindObjectOfType<AudioManager>().Stop("Game Over");
        FindObjectOfType<AudioManager>().Stop("BIIIIIIGBOOOOM");
        SceneManager.LoadScene("TitleScene");
        Debug.Log("Loading menu...");
    }
}
