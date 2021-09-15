using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speedoForEngine : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        FindObjectOfType<AudioManager>().Play("CarEngine");

    }

    // Update is called once per frame
    void Update()
    {
        speed = rb.velocity.magnitude;
        FindObjectOfType<AudioManager>().ChangePitch("CarEngine", speed);

    }
}
