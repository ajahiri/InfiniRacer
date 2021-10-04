using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    // for scoring display 
    public Text score;

    public void GameOverScreen()
    {
        gameObject.SetActive(true);
    }
}
