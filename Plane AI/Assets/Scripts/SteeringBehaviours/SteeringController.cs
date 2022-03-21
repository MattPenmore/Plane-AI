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

        if(angle > (turnSpeed * Time.deltaTime))
        {
            newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, (turnSpeed / angle) * Time.deltaTime, 0.0f) * newDirection.magnitude;
        }
        else if (angle < -turnSpeed * Time.deltaTime)
        {
            newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, (turnSpeed / angle) * Time.deltaTime, 0.0f) * newDirection.magnitude;
        }

        if ((newDirection.magnitude - rb.velocity.magnitude) / Time.deltaTime > maxAcceleration)
        {
            newDirection = newDirection.normalized * (rb.velocity.magnitude + maxAcceleration * Time.deltaTime);
        }

        rb.velocity = newDirection;
        if(rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
        else if(rb.velocity.magnitude < minVelocity)
        {
            rb.velocity = rb.velocity.normalized * minVelocity;
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

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
