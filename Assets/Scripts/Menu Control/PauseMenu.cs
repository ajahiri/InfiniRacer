using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject GameElementsUI;

    bool gameIsPaused = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        GameElementsUI.SetActive(true);
        Time.timeScale = 1f;
        gameIsPaused = false;
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        GameElementsUI.SetActive(false);
        Time.timeScale = 0f;
        gameIsPaused = true;
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
        Debug.Log("Loading menu...");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("RaceArea01");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
        Debug.Log("Restarting game...");
    }
}
