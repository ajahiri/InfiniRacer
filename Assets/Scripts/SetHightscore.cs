using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetHightscore : MonoBehaviour
{
    // Start is called before the first frame update
    int score = 0;
    int scoreUpdate;
    public TextMeshProUGUI text;

    void Start()
    {
        score = GameObject.Find("HighScore").GetComponent<highscore>().GetScore();
    }

    // Update is called once per frame
    void Update()
    {
        
        text.text = "High Score: " + score;
        checkscore();
    }
    public void checkscore()
    {
        scoreUpdate = GameObject.Find("HighScore").GetComponent<highscore>().GetScore();
        if (scoreUpdate> score)
        {
            score = scoreUpdate;
        }
    }


}
