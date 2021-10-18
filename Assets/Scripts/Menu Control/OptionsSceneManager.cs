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
        updateBotText();
    }
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
        PlayerPrefs.SetInt("GlobalBotNum", numBots);
        BotNum.setBotNum(numBots);
        updateBotText();
    }

    private void updateBotText() {
        if (BotNum)
        {
            if(BotNum.botNum == 1){
                GameObject.Find("BotText").GetComponent<TextMeshProUGUI>().SetText(BotNum.botNum.ToString() + " Bot");
            } else {
                GameObject.Find("BotText").GetComponent<TextMeshProUGUI>().SetText(BotNum.botNum.ToString() + " Bots");
            }
        }
    }
}
