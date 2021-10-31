using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class OptionsSceneManager : MonoBehaviour
{
    public saveBotNum BotNum;
    private void Start() {
        BotNum = GameObject.FindObjectOfType<saveBotNum>();

        // set bot slider
        GameObject BotSliderOption = GameObject.Find("BotSliderOption");
        int SavedBotNum = PlayerPrefs.GetInt("GlobalBotNum", 1);
        BotSliderOption.GetComponentInChildren<TextMeshProUGUI>().text = "No. Bots - " + SavedBotNum;
        BotSliderOption.GetComponentInChildren<Slider>().value = SavedBotNum;

        // set volume slider
        GameObject VolumeSliderOption = GameObject.Find("VolumeSliderOption");
        float SavedVolume = PlayerPrefs.GetFloat("GlobalGameVolume", 0.6f);
        AudioListener.volume = SavedVolume;
        VolumeSliderOption.GetComponentInChildren<TextMeshProUGUI>().text = "Volume - " + decimal.Round((decimal)SavedBotNum, 2);
        VolumeSliderOption.GetComponentInChildren<Slider>().value = SavedVolume;

        // set diff slider
        GameObject DifficultySliderOption = GameObject.Find("DifficultySliderOption");
        float SavedDiff = PlayerPrefs.GetFloat("GlobalDifficulty", 3);
        DifficultySliderOption.GetComponentInChildren<TextMeshProUGUI>().text = "Difficulty - " + (int)SavedDiff;
        DifficultySliderOption.GetComponentInChildren<Slider>().value = SavedDiff;
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
        FindObjectOfType<AudioManager>().Play("button press");
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
