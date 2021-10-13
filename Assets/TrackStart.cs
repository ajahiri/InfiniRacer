    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackStart : MonoBehaviour
{
    private GameObject playerVehicle;
    private GameObject botVehicle;

    // Start is called before the first frame update
    void Start()
    {
        playerVehicle = GameObject.FindWithTag("Player");
        botVehicle = GameObject.FindWithTag("Car");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Prioritise player vehicle pos, if no player, then use bot distance for destroy
        if (playerVehicle)
        {
            if (Vector3.Distance(playerVehicle.transform.position, transform.position) > 70f)
            {
                Destroy(gameObject);
            }
        } else if (botVehicle)
        {
            if (Vector3.Distance(botVehicle.transform.position, transform.position) > 70f)
            {
                Destroy(gameObject);
            }
        }
    }
}
