using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionsSceneManager : MonoBehaviour
{
    public void GoBackToTitleScene()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("Going back to title scene...");
        SceneManager.LoadScene("TitleScene");
    }

    public void ChangeAudioVolume(float newVolume)
    {
        Debug.Log($"Setting volume: {newVolume}");
        AudioListener.volume = newVolume;
        PlayerPrefs.SetFloat("GlobalGameVolume", newVolume);
    }

    public void ChangeDifficulty(float newDifficulty)
    {
        
        PlayerPrefs.SetFloat("GlobalDifficulty", newDifficulty);
    }

    public void ChangeNumBots(float newBotNum)
    {
        int numBots = (int)newBotNum;
        Debug.Log(numBots);
        PlayerPrefs.SetInt("GlobalBotNum", numBots);
        GameObject.Find("BotNum").GetComponent<saveBotNum>().setBotNum(numBots);
    }
}
