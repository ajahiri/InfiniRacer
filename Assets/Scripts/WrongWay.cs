using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WrongWay : MonoBehaviour
{
    private GameObject checkpointHandlerObject;
    [SerializeField] GameObject trackHandlerObject;
    [SerializeField] GameObject wrongway;
    private GameObject car;
    private int index = 0;
    private Checkpoint trackRotation;
    // Start is called before the first frame update
    void Start()
    {

        car = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {   

        index = (checkpointHandlerObject.GetComponent<TrackCheckpoints>().GetNumCheckpoints()) -1;
        trackRotation = checkpointHandlerObject.GetComponent<TrackCheckpoints>().GetCheckpoint(index);
        if(Vector3.Angle(car.transform.forward, trackRotation.transform.forward) > 100){
            wrongway.GetComponent<Text>().enabled = true;
        }
        else{
            wrongway.GetComponent<Text>().enabled = false;
        }
    }
}
