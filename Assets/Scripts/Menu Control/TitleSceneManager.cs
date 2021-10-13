using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI botsCountTextMesh;
    [SerializeField] private TextMeshProUGUI gameVolumeTextMesh;
    [SerializeField] private TextMeshProUGUI difficultyTextMesh;

    private int botsCount;
    private float gameVolume;
    private float difficulty;

    public void Start()
    {
        botsCount = PlayerPrefs.GetInt("BotsCount", 1);
        gameVolume = PlayerPrefs.GetFloat("GameVolume", 0.5f);
        difficulty = PlayerPrefs.GetFloat("GameDifficulty", 0.5f);

        UpdateBotCountSliderText();
        UpdateGameVolumeSliderText();
    }

    public void StartGame()
    {
        Vibrator.Vibrate(Vibration.SHORT);
        Debug.Log("Starting game...");
        SceneManager.LoadScene("RaceArea01");
    }

    public void ChangeNumberOfBots(float newCountFloat)
    {
        Vibrator.Vibrate(Vibration.SHORT);
        botsCount = (int)newCountFloat;

        Debug.Log($"Setting bot count: {botsCount}");
        UpdateBotCountSliderText();
        PlayerPrefs.SetInt("BotCount", botsCount);
    }

    public void ChangeAudioVolume(float newVolume)
    {
        Vibrator.Vibrate(Vibration.SHORT);
        gameVolume = newVolume / 100;

        Debug.Log($"Setting volume: {(int)newVolume}%");
        UpdateGameVolumeSliderText();
        AudioListener.volume = gameVolume;
        PlayerPrefs.SetFloat("GameVolume", gameVolume);
    }

    private void UpdateBotCountSliderText()
    {
        botsCountTextMesh.text = botsCount == 0 ? "None" : botsCount.ToString();
    }

    private void UpdateGameVolumeSliderText()
    {
        gameVolumeTextMesh.text = $"{gameVolume * 100}%";
    }
}
