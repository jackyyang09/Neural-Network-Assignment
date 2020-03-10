using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    bool crashed = false;

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
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!crashed)
        {
            CastRays();
            Steer();
            Move();
        }
    }


    void CastRays()
    {
        forwardDistance = GetRayDistance(transform.forward);
        leftDistance = GetRayDistance(-transform.right);
        rightDistance = GetRayDistance(transform.right);
        leftDiagonal = GetRayDistance((transform.forward - transform.right).normalized);
        rightDiagonal = GetRayDistance((transform.forward + transform.right).normalized);
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
    }

    void Accelerate()
    {
        speed = Mathf.Min(maxSpeed, speed + acceleration * Time.fixedDeltaTime);
    }

    void Deccelerate()
    {
        speed = Mathf.Max(0, speed - brake * Time.fixedDeltaTime);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Rail"))
        {
            crashed = true;
            speed = 0;
            totalFitness += currentFitness;
            currentFitness = 0;
        }

        if (collision.gameObject.tag.Equals("Road"))
        {
            currentTrack = collision.gameObject.GetComponent<TrackFitness>();
            totalFitness += currentFitness;
            currentFitness = 0;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Road"))
        {
            if (currentTrack != null)
                currentFitness = currentTrack.GetFitness(transform);
        }
    }
}
