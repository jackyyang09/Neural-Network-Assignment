using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAnimator : MonoBehaviour
{
    [SerializeField]
    Vector3 followOffset = new Vector3(0, -1, 0);

    [SerializeField]
    List<float> rotations;

    [SerializeField]
    int samples;

    [SerializeField]
    Vector2 turnRange = new Vector2();
    [SerializeField]
    float maxRotationDelta;

    [SerializeField]
    float turn;

    [SerializeField]
    float turnDecay;

    [SerializeField]
    float magicLerpValue = 0.1f;

    Rigidbody rb;

    Animator anim;

    Vector3 prevDir;

    [SerializeField]
    Transform car;
    CarController carC;
    [SerializeField]
    float tireSpinMultiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        carC = car.GetComponent<CarController>();
        rb = GetComponentInParent<Rigidbody>();
        anim = GetComponent<Animator>();
        rotations = new List<float>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = car.transform.position + followOffset;
        transform.rotation = Quaternion.Lerp(transform.rotation, car.rotation, magicLerpValue);

        Vector3 curDir = transform.forward;
        float curDelta = Vector3.SignedAngle(curDir, prevDir, car.up);

        //// Do some smoothing
        //rotations.Add(curDelta);
        //if (rotations.Count > samples)
        //{
        //    rotations.RemoveAt(0);
        //}
        //if (rotations.Count == samples)
        //{
        //    float average = 0;
        //    for (int i = 0; i < samples; i++)
        //    {
        //        average += rotations[i];
        //    }
        //    average /= samples;
        //    turn += average;
        //}
        turn = curDelta;

        //turn = Mathf.MoveTowards(curDelta, turn + curDelta, maxRotationDelta * Time.deltaTime);
        //
        turn = Mathf.MoveTowards(turn, 0, turnDecay * Time.deltaTime);
        //turn = Mathf.Clamp(turn, turnRange.x, turnRange.y);

        anim.SetFloat("Drift", Mathf.InverseLerp(turnRange.x, turnRange.y, turn));
        anim.SetFloat("Speed", carC.GetSpeed() * tireSpinMultiplier);

        prevDir = curDir;
    }
}