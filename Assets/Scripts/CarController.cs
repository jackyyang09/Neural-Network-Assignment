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

    [SerializeField]
    float totalFitness = 0;

    List<GameObject> touchedTracks = new List<GameObject>();

    UnityAction _onCrash;

    // Start is called before the first frame update
    void Start()
    {
        distances = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
        rb = GetComponent<Rigidbody>();

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

    void Move()
    {
        
        rb.MovePosition(transform.position + (transform.forward * speed));
        rb.MoveRotation(Quaternion.Euler(transform.rotation.x, transform.rotation.y + (Mathf.Rad2Deg * turn), transform.rotation.z));
    }

    public void Accelerate()
    {
        speed = Mathf.Clamp(speed + acceleration * Time.fixedDeltaTime, 0.1f, maxSpeed);
    }

    public void Deccelerate()
    {
        speed = Mathf.Clamp(speed - brake * Time.fixedDeltaTime, 0.1f, maxSpeed);
    }

    public void TurnLeft()
    {
        turn -= turnAngle * Time.fixedDeltaTime;
    }

    public void TurnRight()
    {
        turn += turnAngle * Time.fixedDeltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Rail"))
        {
            crashed = true;
            speed = 0;
            _onCrash.Invoke();
        }

        if (collision.collider.tag.Equals("Road"))
        {
            if (!touchedTracks.Contains(collision.gameObject))
            {
                touchedTracks.Add(collision.gameObject);
                totalFitness += 1;
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
        return totalFitness;
    }

    public void Init()
    {
        touchedTracks.Clear();

        transform.SetPositionAndRotation(transform.parent.position, transform.parent.rotation);

        crashed = false;
        turn = 0.0f;
        speed = maxSpeed/2.0f;

        totalFitness = 0.0f;
    }
}
