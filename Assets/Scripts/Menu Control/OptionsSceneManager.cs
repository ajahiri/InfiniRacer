using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsSceneManager : MonoBehaviour
{
    public void GoBackToTitleScene()
    {
        Debug.Log("Going back to title scene...");
        SceneManager.LoadScene("TitleScene");
    }
}
