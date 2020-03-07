using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;
    public float rotTime = 0.25f;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotVel = Vector3.zero;

    void Update()
    {
        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
        transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, target.eulerAngles, ref rotVel, rotTime);
    }
}