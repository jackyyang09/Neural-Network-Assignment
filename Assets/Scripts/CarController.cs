using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    bool crashed = false;

    [SerializeField]
    float[] distances;

    [SerializeField]
    float forwardDistance = 0;

    [SerializeField]
    float leftDistance = 0;

    [SerializeField]
    float rightDistance = 0;

    [SerializeField]
    float leftDiagonal = 0;

    [SerializeField]
    float rightDiagonal = 0;

    [SerializeField]
    float speed = 0.5f;

    [SerializeField]
    float maxSpeed = 0.5f;

    [SerializeField]
    float turn = 0.0f;

    [SerializeField]
    float turnAngle = 5;
    [SerializeField]
    float acceleration = 1;

    [SerializeField]
    float brake = 0.5f;
    TrackFitness currentTrack;
    TrackFitness prevTrack;
    float currentFitness = 0;

    /// <summary>
    /// Checks for a positive change in fitness after this many seconds
    /// </summary>
    [SerializeField]
    float periodicFitnessCheck = 5;

    [SerializeField]
    float totalFitness = 0;
    float savedFitness;

    List<GameObject> touchedTracks = new List<GameObject>();

    TrackGenerator trackGen;

    UnityAction _onCrash;

    // Start is called before the first frame update
    void Start()
    {
        distances = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        rb = GetComponent<Rigidbody>();
        trackGen = FindObjectOfType<TrackGenerator>();
        _onCrash += NeuralNetMaster.Instance.CheckCrashes;

        InvokeRepeating("FitnessCheckup", periodicFitnessCheck, periodicFitnessCheck);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!crashed)
        {
            CastRays();
            Move();
        }
    }

    void CastRays()
    {
        distances[0] = GetRayDistance(transform.forward);
        distances[1] = GetRayDistance(-transform.right);
        distances[2] = GetRayDistance(transform.right);
        distances[3] = GetRayDistance((transform.forward - transform.right).normalized);
        distances[4] = GetRayDistance((transform.forward + transform.right).normalized);

        float max = -50;

        for (int i = 0; i < distances.Length; i++)
        {
            if (distances[i] > max)
                max = distances[i];
        }

        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = UtilMath.Lmap(distances[i], 0.0f, max, 0.0f, 1.0f);
        }
    }

    public float GetDistance(int i)
    {
        return distances[i];
    }

    float GetRayDistance(Vector3 dir)
    {
        RaycastHit hit;

        //Cast Forward ray
        if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, ~LayerMask.NameToLayer("Track")))
        {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.red, 0.1f);
            return hit.distance;
        }

        else
            return Mathf.Infinity;
    }

    void Move()
    {
        
        rb.MovePosition(transform.position + (transform.forward * speed));
        rb.MoveRotation(Quaternion.Euler(transform.rotation.x, transform.rotation.y + (Mathf.Rad2Deg * turn), transform.rotation.z));
    }

    public void Accelerate()
    {
        //Debug.Log("Acc");

        speed = Mathf.Clamp(speed + acceleration * Time.fixedDeltaTime, 0.1f, maxSpeed);
        //speed = Mathf.Min(maxSpeed, speed + acceleration * Time.fixedDeltaTime);
    }

    public void Deccelerate()
    {
        //Debug.Log("Decc");

        speed = Mathf.Clamp(speed - brake * Time.fixedDeltaTime, 0.1f, maxSpeed);

        //speed = Mathf.Max(0, speed - brake * Time.fixedDeltaTime);
    }

    public void TurnLeft()
    {
        //Debug.Log("Left");
        turn -= turnAngle * Time.fixedDeltaTime;
        //rb.MoveRotation(Quaternion.Euler(transform.rotation.x, transform.rotation.y + (Mathf.Rad2Deg * turn), transform.rotation.z));

    }

    public void TurnRight()
    {
        //Debug.Log("Right");

        turn += turnAngle * Time.fixedDeltaTime;
        //rb.MoveRotation(Quaternion.Euler(transform.rotation.x, transform.rotation.y + (Mathf.Rad2Deg * turn), transform.rotation.z));

    }

    void Turn()
    {
        if (leftDiagonal > rightDiagonal)
        {
            turn -= turnAngle * Time.fixedDeltaTime;
        }

        else if (rightDiagonal > leftDiagonal)
        {
            turn += turnAngle * Time.fixedDeltaTime;
        }

        rb.MoveRotation(Quaternion.Euler(transform.rotation.x, transform.rotation.y + (Mathf.Rad2Deg * turn), transform.rotation.z));
    }

    /// <summary>
    /// Periodically checks for good progress, failing this benchmark will end the car's current run immediately
    /// </summary>
    public void FitnessCheckup()
    {
        float fit = totalFitness + currentFitness;
        if (fit - savedFitness < 0.3f)
        {
            KYS();
        }
        else
        {
            savedFitness = fit;
        }
    }

    public void KYS()
    {
        currentTrack = null;
        crashed = true;
        speed = 0;
        //totalFitness += currentFitness;
        currentFitness = 0;
        _onCrash.Invoke();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Rail"))
        {
            KYS();
        }

        if (collision.collider.CompareTag("Road"))
        {
            if (!touchedTracks.Contains(collision.gameObject))
            {
                touchedTracks.Add(collision.gameObject);
                // Round to an int will act as a countermeasure if the car drives backwards
                totalFitness = Mathf.RoundToInt(totalFitness + currentFitness);
            }
            currentTrack = collision.gameObject.GetComponentInChildren<TrackFitness>();
            currentFitness = 0;
        }

        //if (totalFitness % (float)prevFitness > 1)
        //{
        //    prevFitness = (int)totalFitness % prevFitness;
        //    trackGen.RemovePreviousTrackPiece();
        //}
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Road"))
        {
            if (currentTrack != null)
            {
                currentFitness = currentTrack.GetFitness(transform);
            }
        }
    }

    public float GetSpeed()
    {
        return speed;
    }

    public bool GetCrashed()
    {
        return crashed;
    }

    public float GetFitness()
    {
        return totalFitness + currentFitness;
    }

    public void Init()
    {
        touchedTracks.Clear();

        transform.SetPositionAndRotation(transform.parent.position, transform.parent.rotation);

        crashed = false;
        turn = 0.0f;
        speed = maxSpeed/2.0f;

        currentFitness = 0.0f;
        totalFitness = 0.0f;
        savedFitness = 0;
    }
}
