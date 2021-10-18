using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject GameElementsUI;

    CarController carController;

    bool gameIsPaused = false;

    void Start()
    {
        carController = GameObject.FindGameObjectWithTag("Player").gameObject.transform.GetComponent<CarController>();
    }

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
        FindObjectOfType<AudioManager>().Play("button press");
        pauseMenuUI.SetActive(false);
        FindObjectOfType<AudioManager>().Play("CarEngine");
        GameElementsUI.SetActive(true);
        Time.timeScale = 1f;
        gameIsPaused = false;
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }

    public void PauseGame()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        pauseMenuUI.SetActive(true);
        FindObjectOfType<AudioManager>().Stop("CarEngine");
        GameElementsUI.SetActive(false);
        Time.timeScale = 0f;
        gameIsPaused = true;
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }

    public void LoadMenu()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
        Debug.Log("Loading menu...");
    }

    public void RestartGame()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Time.timeScale = 1f;
        SceneManager.LoadScene("RaceArea01");
        FindObjectOfType<AudioManager>().Play("CarEngine");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
        Debug.Log("Restarting game...");
    }

}
