using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject loadingScreen;
    public static GameManager instance;
    private void Awake()
    {
        instance = this;

        SceneManager.LoadSceneAsync((int)SceneIndexes.TitleScene, LoadSceneMode.Additive);
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void LoadGame()
    {
        loadingScreen.gameObject.SetActive(true);

        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TitleScene));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.CarSelectionScene, LoadSceneMode.Additive));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.RaceArea01, LoadSceneMode.Additive));

    }
}
