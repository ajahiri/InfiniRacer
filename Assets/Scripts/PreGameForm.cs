using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PreGameForm : MonoBehaviour
{
    public string submissionName = "Anonymous";
    public float attentionRating = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attentionRating > 0)
        {
            GameObject.Find("SubmitPregameForm").GetComponent<Button>().interactable = true;
        }
    }

    public void SetSubmissionName(string newSubmissionName)
    {
        submissionName = newSubmissionName;
        Debug.Log(submissionName);
    }
    public void SetAttentionRating(float newAttentionRating)
    {
        attentionRating = newAttentionRating;
        GameObject.Find("PregameAttentionRating").GetComponent<TextMeshProUGUI>().text = "Attention Rating - " + (int)newAttentionRating;
        Debug.Log(attentionRating);
    }

    // Will save the submission details no matter what, when actual submission occurs
    // in post game screen it will check if 0 rating or not
    public void Proceed()
    {
        PlayerPrefs.SetFloat("attentionRatingPregame", attentionRating);
        PlayerPrefs.SetString("submissionName", submissionName);
        PlayerPrefs.SetFloat("startPlayTime", Time.time);
        Debug.Log("Submitting pregame form...Saving data in prefs");
        GameObject.Find("SceneManager").GetComponent<TitleSceneManager>().SelectCar();
    }
}
