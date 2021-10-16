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
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }

    public void ChangeAudioVolume(float newVolume)
    {
        Debug.Log($"Setting volume: {newVolume}");
        AudioListener.volume = newVolume;
        PlayerPrefs.SetFloat("GlobalGameVolume", newVolume);
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }

    public void ChangeDifficulty(float newDifficulty)
    {
        
        PlayerPrefs.SetFloat("GlobalDifficulty", newDifficulty);
        Vibrator.Vibrate(Vibration.SHORT);  // 100 ms
    }
}
