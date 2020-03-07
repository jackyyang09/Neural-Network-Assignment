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



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
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
        speed = Mathf.Min(maxSpeed, speed + 0.1f);
    }

    void Deccelerate()
    {
        speed = Mathf.Max(0, speed - 0.05f);
    }

    void Turn()
    {
        if (leftDiagonal > rightDiagonal)
        {
            turn -= 0.05f;
        }

        else if (rightDiagonal > leftDiagonal)
        {
            turn += 0.05f;
        }

        rb.MoveRotation(Quaternion.Euler(transform.rotation.x, transform.rotation.y + (Mathf.Rad2Deg * turn), transform.rotation.z));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Rail"))
        {
            crashed = true;
            speed = 0;
        }
    }
}
