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
    float maxSpeed = 1.0f;

    float turn = 0.0f;
    [SerializeField]
    float turnAngle = 5;
    [SerializeField]
    float acceleration = 1;

    [SerializeField]
    float brake = 0.5f;
    TrackFitness currentTrack;
    float currentFitness = 0;

    [SerializeField]
    float totalFitness = 0;
    int prevFitness;

    TrackGenerator trackGen;

    UnityAction _onCrash;

    // Start is called before the first frame update
    void Start()
    {
        distances = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        rb = GetComponent<Rigidbody>();
        trackGen = FindObjectOfType<TrackGenerator>();
        _onCrash += NeuralNetMaster.Instance.CheckCrashes;
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

    void Steer()
    {
        if (forwardDistance < 5.0f)
        {
            Deccelerate();
        }

        else
            Accelerate();

        Turn();
    }

    void Move()
    {
        rb.MovePosition(transform.position + (transform.forward * speed));
        rb.MoveRotation(Quaternion.Euler(transform.rotation.x, transform.rotation.y + (Mathf.Rad2Deg * turn), transform.rotation.z));

    }

    public void Accelerate()
    {
        Debug.Log("Acc");

        speed = Mathf.Min(maxSpeed, speed + acceleration * Time.fixedDeltaTime);
    }

    public void Deccelerate()
    {
        Debug.Log("Decc");

        speed = Mathf.Max(0, speed - brake * Time.fixedDeltaTime);
    }

    public void TurnLeft()
    {
        Debug.Log("Left");
        turn -= turnAngle * Time.fixedDeltaTime;
        //rb.MoveRotation(Quaternion.Euler(transform.rotation.x, transform.rotation.y + (Mathf.Rad2Deg * turn), transform.rotation.z));

    }

    public void TurnRight()
    {
        Debug.Log("Right");

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Rail"))
        {
            crashed = true;
            speed = 0;
            totalFitness += currentFitness;
            currentFitness = 0;
            _onCrash.Invoke();
        }

        if (collision.collider.tag.Equals("Road"))
        {
            currentTrack = collision.gameObject.GetComponent<TrackFitness>();
            totalFitness += currentFitness;
            currentFitness = 0;
        }

        if (totalFitness % (float)prevFitness > 1)
        {
            prevFitness = (int)totalFitness % prevFitness;
            trackGen.RemovePreviousTrackPiece();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Road"))
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

    public void Init()
    {
        crashed = false;
        turn = 0.0f;
        speed = 1.0f;

        currentFitness = 0.0f;
        totalFitness = 0.0f;
        prevFitness = 0;
    }
}
