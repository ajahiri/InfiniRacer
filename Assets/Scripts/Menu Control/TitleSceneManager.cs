using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void PregameSubmission()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        SceneManager.LoadScene("AttentionScreen");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }

    public void PostgameSubmission()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        SceneManager.LoadScene("PostAttentionScreen");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }
    [SerializeField] GameObject LoadScreen;



    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    bool gameIsLoading = false;
    public void LoadGame()
    {
        LoadScreen.gameObject.SetActive(true);

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.CarSelectionScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.RaceArea01));

        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("loading game...");
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }







}
