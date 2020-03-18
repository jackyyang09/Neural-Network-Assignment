using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackFitness : MonoBehaviour
{
    [SerializeField]
    Transform startPoint;

    [SerializeField]
    Transform endPoint;

    [SerializeField]
    bool reversed;

    public float GetFitness(Transform carPoint)
    {
        //float mag = (endPoint.position - startPoint.position).magnitude;
        //
        //float fitness = (startPoint.position - carPoint.position).magnitude;
        //
        //return (fitness / mag);
        var pointOnLine = Vector3.Project(carPoint.position, (startPoint.position - endPoint.position).normalized);

        if (reversed)
        return InverseLerp(endPoint.position, startPoint.position, pointOnLine);
        else
        return InverseLerp(startPoint.position, endPoint.position, pointOnLine);
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }
}
