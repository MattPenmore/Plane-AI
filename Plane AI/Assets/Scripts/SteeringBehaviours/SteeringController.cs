using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringController : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField]
    ObstacleAvoidance avoidance;
    [SerializeField]
    PlanePathfinding seek;

    [SerializeField]
    float seekRatio;
    [SerializeField]
    float avoidanceRatio;
    public float maxVelocity = 300;
    public float minVelocity = 50;
    private float minSpeed = 0;

    public float maxAcceleration = 50;

    public float turnSpeed = 20f;

    public Vector3 avoidanceForce;
    public Vector3 seekForce;
    public Vector3 newDirection;

    public float curSpeed;
    public float angle;

    [SerializeField]
    float cutOffAngle;

    [SerializeField]
    GameObject body;

    [SerializeField]
    float bodyRotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
            avoidanceForce = avoidance.avoidanceForce;
            seekForce = seek.seekForce;

        if(avoidanceForce.magnitude < maxAcceleration / 2)
        {
            avoidanceRatio = 0;
        }
        else
        {
            avoidanceRatio = avoidanceForce.magnitude / maxAcceleration;
        }

        seekRatio = 1 - avoidanceRatio;

        newDirection = rb.velocity + avoidanceForce * avoidanceRatio + seekForce * seekRatio;
        angle = Vector3.Angle(newDirection, transform.TransformDirection(Vector3.forward));

        //Smooth to prevent jerking
        if(angle < cutOffAngle && angle > -cutOffAngle)
        {
            newDirection = rb.velocity + (rb.velocity.normalized * (avoidanceForce.magnitude * avoidanceRatio + seekForce.magnitude * seekRatio));
        }

        if(angle > (turnSpeed/* * Time.deltaTime*/))
        {
            newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, (turnSpeed / angle) * Time.deltaTime, 0.0f) * newDirection.magnitude;
        }
        else
        {
            newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, Time.deltaTime, 0.0f) * newDirection.magnitude;
        }
        //else if (angle < -turnSpeed * Time.deltaTime)
        //{
        //    newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, (turnSpeed / angle) * Time.deltaTime, 0.0f) * newDirection.magnitude;
        //}

        if ((newDirection.magnitude - rb.velocity.magnitude) / Time.deltaTime > maxAcceleration)
        {
            newDirection = newDirection.normalized * (rb.velocity.magnitude + maxAcceleration * Time.deltaTime);
        }

        if(newDirection.magnitude > maxVelocity)
        {
            rb.velocity = newDirection.normalized * maxVelocity;
        }
        else if(newDirection.magnitude < minVelocity)
        {
            rb.velocity = newDirection.normalized * minVelocity;
        }
        else
        {
            rb.velocity = newDirection;
        }
        curSpeed = rb.velocity.magnitude;

        LookAtDirection();
    }

    public void LookAtDirection()
    {
        Vector3 direction = rb.velocity;
        direction.Normalize();

        /* If we have a non-zero direction then look towards that direciton otherwise do nothing */
        if (direction.sqrMagnitude > 0.001f)
        {
            Vector3 perp = Vector3.Cross(transform.forward, direction);
            float dir = Vector3.Dot(perp, transform.up);

            if(Mathf.Abs(dir) < 0.0005)
            {
                body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0, 0), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z)) * Time.deltaTime * bodyRotateSpeed);
            }
            else
            {
                body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0,Mathf.Clamp(-dir * 10000, -45, 45)), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z - Mathf.Clamp(-dir * 10000, -45, 45))) * Time.deltaTime * bodyRotateSpeed);
            }

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
