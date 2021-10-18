using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scoring : MonoBehaviour
{
    private GameObject playerObject;
    public int score = 0;

    private TrackCheckpoints trackCheckpointsScript;
    private Text placementText;

    // Start is called before the first frame update
    void Start()
    {
        trackCheckpointsScript = GameObject.Find("CheckpointHandler").GetComponent<TrackCheckpoints>();
        placementText = transform.GetChild(0).GetComponent<Text>();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(AddScore());   
    }

    private void FixedUpdate()
    {
        int placeIndex = trackCheckpointsScript.GetPlace(playerObject.transform);
        string placeText = "1st";
        switch (placeIndex)
        {
            case 0:
                placeText = "1st";
                break;
            case 1:
                placeText = "2nd";
                break;
            case 2:
                placeText = "3rd";
                break;
            default:
                placeText = "1st";
                break;
        }
        if (placeIndex > 2)
        {
            placeText = $"{placeIndex + 1}th";
        }
        placementText.text = placeText;
        gameObject.GetComponent<Text>().text = "Score: " + score;
        FindObjectOfType<highscore>().CheckScore(score);
    }

    IEnumerator AddScore()
    {
        for (; ; )
        {
            // Add score every second if player is in first place
            bool isFirstPlace = trackCheckpointsScript.isFirstPlace(playerObject.transform);
            if (isFirstPlace)
            {
                score += 100;
            }
            yield return new WaitForSeconds(1f);
        }
    }
    public void CoinPickup()
    {
        score += 50;
    }
}
