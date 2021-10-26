using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class highscore : MonoBehaviour
{
    public static highscore instance;
    public static int score;
    private int currentScore = 0;
    
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        { Destroy(gameObject); };

        DontDestroyOnLoad(gameObject);

        score = PlayerPrefs.GetInt("highscore", score);
    }
   

    public void CheckScore(int currentgame)
    {
        currentScore = currentgame;
        if(currentgame> score)
        {
            score = currentgame;
            PlayerPrefs.SetInt("highscore", score);
        }
    }
    public int GetScore()
    {
        return score;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}
