using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OptionsSceneManager : MonoBehaviour
{
    public saveBotNum BotNum;
    private void Start() {
        BotNum = GameObject.FindObjectOfType<saveBotNum>();
    }
    public void GoBackToTitleScene()
    {
        FindObjectOfType<AudioManager>().Play("button press");
        Debug.Log("Going back to title scene...");
        SceneManager.LoadScene("TitleScene");
    }

    public void ChangeAudioVolume(float newVolume)
    {
        AudioListener.volume = newVolume;
        PlayerPrefs.SetFloat("GlobalGameVolume", newVolume);
        GameObject.Find("VolumeSliderOption").GetComponentInChildren<TextMeshProUGUI>().text = "Volume - " + decimal.Round((decimal)newVolume, 2);
    }

    public void ChangeDifficulty(float newDifficulty)
    {
        PlayerPrefs.SetFloat("GlobalDifficulty", newDifficulty);
        GameObject.Find("DifficultySliderOption").GetComponentInChildren<TextMeshProUGUI>().text = "Difficulty - " + (int)newDifficulty;
    }

    public void ChangeNumBots(float newBotNum)
    {
        int numBots = (int)newBotNum;
        PlayerPrefs.SetInt("GlobalBotNum", numBots);
        BotNum.setBotNum(numBots);
        GameObject.Find("BotSliderOption").GetComponentInChildren<TextMeshProUGUI>().text = "No. Bots - " + (int)newBotNum;
    }
}
