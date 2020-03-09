using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimator : MonoBehaviour
{
    [SerializeField]
    List<float> rotations;

    [SerializeField]
    int samples;

    [SerializeField]
    [Range(-3f, 3f)]
    float turn;

    Rigidbody rb;

    Animator anim;

    Vector3 prevDir;

    Transform car;

    // Start is called before the first frame update
    void Start()
    {
        car = transform.parent;
        rb = GetComponentInParent<Rigidbody>();
        anim = GetComponent<Animator>();
        rotations = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 curDir = car.forward;
        float curDelta = Vector3.SignedAngle(curDir, prevDir, car.up);

        turn += curDelta;
        turn = Mathf.Clamp(turn, -3, 3);

        // Do some smoothing
        rotations.Add(curDelta);
        if (rotations.Count > samples)
        {
            rotations.RemoveAt(0);
        }
        if (rotations.Count == samples)
        {
            float average = 0;
            for (int i = 0; i < samples; i++)
            {
                average += rotations[i];
            }
            average /= samples;
            turn = average;
            anim.SetFloat("Drift", Mathf.InverseLerp(-3, 3, turn));
        }

        prevDir = curDir;
    }
}
