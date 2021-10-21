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
    }
    public void SelectCar()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("Selecting car");
        SceneManager.LoadScene("CarSelectionScene");
    }

    public void ViewOptions()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("Viewing options...");
        SceneManager.LoadScene("OptionsScene");
    }

    public void PregameSubmission()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        SceneManager.LoadScene("AttentionScreen");
    }

    public void PostgameSubmission()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        SceneManager.LoadScene("PostAttentionScreen");
    }


    [SerializeField] GameObject LoadScreen;
    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void LoadGame()
    {
        LoadScreen.gameObject.SetActive(true);

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.CarSelectionScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.RaceArea01));

        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("loading game...");
    }

}
