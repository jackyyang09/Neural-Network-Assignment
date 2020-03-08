using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackFitness : MonoBehaviour
{
    [SerializeField]
    Transform startPoint;

    [SerializeField]
    Transform endPoint;

    public float GetFitness(Transform carPoint)
    {
        float mag = (endPoint.position - startPoint.position).magnitude;

        float fitness = (startPoint.position - carPoint.position).magnitude;

        return (fitness / mag);
    }
}
