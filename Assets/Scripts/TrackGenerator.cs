using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject[] trackPieces;

    [SerializeField]
    float generationDelay;

    [SerializeField]
    float destroyDelay;
    [SerializeField]
    float initialDestroyDelay;

    Transform lastEndPoint;

    /// <summary>
    /// Begin track regeneration 
    /// </summary>
    private void OnEnable()
    {
        Init();

        //Replace this in the future by having the car call 

    }

    public void Init()
    {
        lastEndPoint = transform;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        GenerateNewTrackPiece();
        GenerateNewTrackPiece();
        GenerateNewTrackPiece();
        CancelInvoke("GenerateNewTrackPiece");
        CancelInvoke("RemovePreviousTrackPiece");

        InvokeRepeating("GenerateNewTrackPiece", 0, generationDelay);
        InvokeRepeating("RemovePreviousTrackPiece", destroyDelay + initialDestroyDelay, destroyDelay);
    }

    public void GenerateNewTrackPiece()
    {
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        if (lastEndPoint != null)
        {
            pos = lastEndPoint.position;
            forward = lastEndPoint.forward;
        }
        Transform track = Instantiate(trackPieces[Random.Range(0, trackPieces.Length)], transform).transform;
        track.transform.position = pos;
        track.transform.forward = forward;
        lastEndPoint = track.GetChild(track.childCount - 1);
    }

    /// <summary>
    /// Call this method to prevent tracks from overlapping
    /// </summary>
    public void RemovePreviousTrackPiece()
    {
        Destroy(transform.GetChild(0).gameObject);
    }

    public void TryAgain()
    {
        Transform newTrack = transform.GetChild(transform.childCount - 2);
        lastEndPoint = newTrack.GetChild(newTrack.childCount - 1);
        CancelInvoke("GenerateNewTrackPiece");
        InvokeRepeating("GenerateNewTrackPiece", 0, generationDelay);
    }
}
