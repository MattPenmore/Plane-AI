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

    public float maxAcceleration = 50;

    public float turnSpeed = 20f;

    public Vector3 avoidanceForce;
    public Vector3 seekForce;
    public Vector3 newDirection;
    public Vector3 desiredDirection;

    public float curSpeed;
    public float angle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //If plane is moving and application has been running long enough
        if (Time.time > 10 && rb.velocity != Vector3.zero)
        {
            //Get avoidance and seek force from other scripts
            avoidanceForce = avoidance.avoidanceForce;
            seekForce = seek.seekForce;

            //Calculate importance of avoidance dependant on its strength
            avoidanceRatio = avoidanceForce.magnitude / maxAcceleration;
            seekRatio = 1 - avoidanceRatio;
            //Combine forces using ratio
            desiredDirection = avoidanceForce * avoidanceRatio + seekForce * seekRatio;
            //Set new direction to original velocity combind with desired direciton velocity
            newDirection = rb.velocity + avoidanceForce * avoidanceRatio + seekForce * seekRatio;
            Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
            //Rotate plane towards new direction, with limit based on turn speed
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(newDirection), turnSpeed * Time.deltaTime);
            //Set new direction to direciton plane is facing
            newDirection = transform.TransformDirection(localVel * newDirection.magnitude);

            //Limit speed to between max and min velocity
            if (newDirection.magnitude > maxVelocity)
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
        }
        else if (Time.time < 10)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = transform.TransformDirection(Vector3.forward) * 100;
        }
    }
}
