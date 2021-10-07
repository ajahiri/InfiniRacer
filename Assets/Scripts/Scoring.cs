using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scoring : MonoBehaviour
{
    private GameObject playerObject;
    private int score = 0;

    private TrackCheckpoints trackCheckpointsScript;
    private Text placementText;

    // Start is called before the first frame update
    void Start()
    {
        trackCheckpointsScript = GameObject.FindObjectOfType<TrackCheckpoints>();
        placementText = transform.GetChild(0).GetComponent<Text>();
        Debug.Log(trackCheckpointsScript);
        playerObject = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(AddScore());   
    }

    private void FixedUpdate()
    {
        bool isFirstPlace = trackCheckpointsScript.isFirstPlace(playerObject.transform);
        placementText.text = isFirstPlace ? "1st" : "2nd";
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
                gameObject.GetComponent<Text>().text = "Score: " + score;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
