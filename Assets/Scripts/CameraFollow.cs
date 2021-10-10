using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 positionalOffset;
    [SerializeField] private Quaternion directionalOffset;
    private Transform target;
    [SerializeField] private float translateSpeed;
    [SerializeField] private float rotationSpeed;
    private Vector3 velocity = Vector3.zero;

    public float smoothTime = 1F;


    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        // Use a directionalOffset so camera rotation can be adjusted in editor
        var direction = target.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime) * directionalOffset;

        // Add a positional offset to the camera as we don't want to be inside the car
        var targetPosition = target.TransformPoint(positionalOffset);
        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
