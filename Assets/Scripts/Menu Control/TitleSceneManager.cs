using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI botsCountSliderText;
    [SerializeField] private TextMeshProUGUI gameVolumeSliderText;
    [SerializeField] private TextMeshProUGUI difficultySliderText;

    public void StartGame()
    {
        Vibrator.Vibrate(Vibration.SHORT);
        Debug.Log("Starting game...");
        SceneManager.LoadScene("RaceArea01");
    }

    public void ChangeNumberOfBots(float newCountFloat)
    {
        Vibrator.Vibrate(Vibration.SHORT);
        var newCount = (int)newCountFloat;
        Debug.Log($"Setting bot count: {newCount}");
        botsCountSliderText.text = newCount == 0 ? "None" : newCount.ToString();
        PlayerPrefs.SetFloat("BotCount", newCount);
    }

    public void ChangeAudioVolume(float newVolume)
    {
        Vibrator.Vibrate(Vibration.SHORT);
        Debug.Log($"Setting volume: {newVolume}");
        AudioListener.volume = newVolume;
        gameVolumeSliderText.text = $"{((int)newVolume)}%";
        PlayerPrefs.SetFloat("GameVolume", newVolume);
    }
}
