    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var playerVehicle = GameObject.FindWithTag("Player");
        if (playerVehicle)
        {
            if (Vector3.Distance(playerVehicle.transform.position, transform.position) > 100f)
            {
                Destroy(gameObject);
            }
        }
    }
}
