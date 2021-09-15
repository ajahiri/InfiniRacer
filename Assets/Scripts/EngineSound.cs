using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour
{
    public Rigidbody vehicleRigidBody;
    private float pitchFromCar;
    public float minPitch = 0.4f;
    public float maxPitch = 1.2f;
    public float maxSpeed = 30;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        vehicleRigidBody = GetComponent<Rigidbody>();
        audioSource.pitch = minPitch;
    }

    // Update is called once per frame
    void Update()
    {
        pitchFromCar = vehicleRigidBody.velocity.magnitude;
        audioSource.pitch = (pitchFromCar / maxSpeed) + minPitch;
    }
}
