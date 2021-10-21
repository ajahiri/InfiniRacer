using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class CloudFunctionsUI : MonoBehaviour
{
    // Unique session ID for identifying this particular play session
    private string sessionID;

    // Just a token that helps avoid any random API reqs being considered valid (in no way secure but is enough for this application)
    static string unityGameToken = "ni8cbHAOOfSokk6t5AF9pJH8mKFd1fN8";

    private float userPlayTime;

    private int currentUserScore;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("PlayerScoreText").GetComponent<TextMeshProUGUI>().text = $"Your Score: {currentUserScore}";
        Debug.Log(PlayerPrefs.GetFloat("attentionRatingPregame"));
        Debug.Log(PlayerPrefs.GetString("submissionName"));
        Debug.Log(PlayerPrefs.GetFloat("startPlayTime"));

        userPlayTime = Time.time - PlayerPrefs.GetFloat("startPlayTime");

        // Get user's current score from dontDestroyOnLoad
        currentUserScore = GameObject.Find("HighScore").GetComponent<highscore>().GetScore();

        // Session ID ensures that users can only submit once per session
        sessionID = System.Guid.NewGuid().ToString();

        if (PlayerPrefs.GetFloat("attentionRatingPregame") == 0)
        {
            GameObject.Find("PostgameAttentionRating").gameObject.SetActive(false);
        }

        StartCoroutine(getLeaderBoard());
    }

    // Define the class for JSON deserialization (High Scores)
    private class SingleScore
    {
        public string _id;
        public string name;
        public int score;

        public static SingleScore CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<SingleScore>(jsonString);
        }
    }
    private class MultiScores
    {
        public string message;
        public SingleScore[] topFiveScores;
    }
    // End class definition for Score Deserialisation

    private MultiScores topFiveScores;
    private string scoreSubmissionName;

    // Get and deserialise leaderboard scores
    private IEnumerator getLeaderBoard()
    {
        Debug.Log("Getting leaderboard scores");
        using (UnityWebRequest www = UnityWebRequest.Get("https://australia-southeast1-infiniracer.cloudfunctions.net/GetTopScores"))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                GameObject.Find("Leaderboard").GetComponent<TextMeshProUGUI>().text = "Leaderboard\nCould not get leaderboard scores...";
            }
            else
            {
                topFiveScores = JsonConvert.DeserializeObject<MultiScores>(www.downloadHandler.text);
                UpdateLeaderboardText(topFiveScores);
            }
        }
    }

    private void UpdateLeaderboardText(MultiScores topScores)
    {
        string newLeaderboardText = "Leaderboard\n";
        foreach (SingleScore score in topScores.topFiveScores)
        {
            newLeaderboardText += $"{score.name}: {score.score}\n";
        }
        GameObject.Find("Leaderboard").GetComponent<TextMeshProUGUI>().text = newLeaderboardText;
    }

    public void SubmitScore()
    {
        var inputText = GameObject.Find("ScoreSubmitInput").GetComponent<TMP_InputField>().text;
        scoreSubmissionName = inputText.Length > 0 ? inputText : "Anonymous";
        Debug.Log(scoreSubmissionName);
        StartCoroutine(SubmitScoreRequest(scoreSubmissionName, currentUserScore));
    }

    // User score class for serialisation
    private class UserScore 
    {
        public string name;
        public int score;
        public string token = unityGameToken;
        public string sessionID;

        public UserScore(string a, int b, string c)
        {
            name = a;
            score = b;
            sessionID = c;
        }
    }
    private IEnumerator SubmitScoreRequest(string submissionName, int score)
    {
        Debug.Log("Submitting user score");
        GameObject.Find("APIStatus").GetComponent<TextMeshProUGUI>().text = "Submitting user score...";
        UserScore newScore = new UserScore(submissionName, score, sessionID);
        string addScoreJSON = JsonConvert.SerializeObject(newScore);
        Debug.Log(addScoreJSON);
        using (UnityWebRequest www = UnityWebRequest.Put("https://australia-southeast1-infiniracer.cloudfunctions.net/AddNewScore", addScoreJSON))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                GameObject.Find("APIStatus").GetComponent<TextMeshProUGUI>().text = "Error sending score, no resubmits allowed...";
            }
            else
            {
                GameObject.Find("APIStatus").GetComponent<TextMeshProUGUI>().text = "Successfully submitted score...";
                StartCoroutine(getLeaderBoard()); // Update leaderboard in case user had a higher score
            }
        }

        yield return new WaitForSeconds(2f);

        GameObject.Find("APIStatus").GetComponent<TextMeshProUGUI>().text = "";
    }
    public void UpdateAttentionRating()
    {
        var attentionRatingObj = GameObject.Find("PostgameAttentionRating");
        var slider = attentionRatingObj.GetComponentInChildren<Slider>();
        attentionRatingObj.GetComponent<TextMeshProUGUI>().text = "Post Game Attention Rating - " + slider.value;
    }

    class AttentionSession
    {
        public string name;
        public int attentionBefore;
        public int attentionAfter;
        public float score;
        public float playTime;
        public string token = unityGameToken;
        public string sessionID;

        public AttentionSession(string a, int b, int c, int d, float e, string f)
        {
            name = a;
            attentionBefore = b;
            attentionAfter = c;
            score = d;
            playTime = e;
            sessionID = f;
        }
    }

    /* Backend requirements for attention session:
        const name = req.body.name || 'Anonymous';
        const attentionBefore = req.body.attentionBefore;
        const attentionAfter = req.body.attentionAfter;
        const playTime = req.body.playTime;
        const score = req.body.score || 0;
        const token = req.body.token || "";
        const sessionID = req.body.sessionID || null;
     */

    public void SubmitAttentionSession()
    {
        StartCoroutine(SubmitAttentionSessionRequest());
    }

    private IEnumerator SubmitAttentionSessionRequest()
    {
        var attentionRatingObj = GameObject.Find("PostgameAttentionRating");
        var slider = attentionRatingObj.GetComponentInChildren<Slider>();
        int newAttentionRating = (int)slider.value;
        Debug.Log(newAttentionRating);

        var name = PlayerPrefs.GetString("submissionName");
        var attentionBefore = (int)PlayerPrefs.GetFloat("attentionRatingPregame");
        
        GameObject.Find("APIStatus").GetComponent<TextMeshProUGUI>().text = "Submitting user attention session...";
        
        AttentionSession newAttentionSession = new AttentionSession(name, attentionBefore, newAttentionRating, currentUserScore, userPlayTime, sessionID);
        string addAttentionJSON = JsonConvert.SerializeObject(newAttentionSession);
        
        Debug.Log(addAttentionJSON);
        
        using (UnityWebRequest www = UnityWebRequest.Put("https://australia-southeast1-infiniracer.cloudfunctions.net/CreateAttentionSession", addAttentionJSON))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                GameObject.Find("APIStatus").GetComponent<TextMeshProUGUI>().text = "Error sending attention session, no resubmits allowed...";
            }
            else
            {
                GameObject.Find("APIStatus").GetComponent<TextMeshProUGUI>().text = "Successfully submitted attention session...";
                StartCoroutine(getLeaderBoard()); // Update leaderboard in case user had a higher score
            }
        }

        yield return new WaitForSeconds(2f);

        GameObject.Find("APIStatus").GetComponent<TextMeshProUGUI>().text = "";
    }
}
