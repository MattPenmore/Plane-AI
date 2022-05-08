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

    [SerializeField]
    GameObject body;

    [SerializeField]
    float bodyRotateSpeed;

    [SerializeField]
    float bodyRotateAmount;

    bool hasCrashed = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if((!hasCrashed) && Time.time > 2 && rb.velocity != Vector3.zero)
        {
            avoidanceForce = avoidance.avoidanceForce;
            seekForce = seek.seekForce;

            avoidanceRatio = avoidanceForce.magnitude / maxAcceleration;
            seekRatio = 1 - avoidanceRatio;
            desiredDirection = avoidanceForce * avoidanceRatio + seekForce * seekRatio;
            newDirection = rb.velocity + avoidanceForce * avoidanceRatio + seekForce * seekRatio;
            //if(!GetComponent<ObstacleCourseAgent>())
            //{
                Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
                //Quaternion oldRotation = transform.rotation;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(newDirection), turnSpeed * Time.deltaTime);
                newDirection = transform.TransformDirection(localVel * newDirection.magnitude);
                //transform.rotation = oldRotation;
                //newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, turnSpeed * Time.deltaTime, 0.0f) * newDirection.magnitude;

                //newDirection = newDirection.normalized * (rb.velocity.magnitude + maxAcceleration * Time.deltaTime);

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
                LookAtDirection();
            //}
        }
        else if (Time.time < 2)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = transform.TransformDirection(Vector3.forward) * 100;
        }
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

            if(Mathf.Abs(dir) < 0.00005)
            {
                body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0, 0), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z)) * Time.deltaTime * bodyRotateSpeed);
            }
            else
            {
                if(dir < 0)
                {
                    body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0,Mathf.Clamp(-dir * bodyRotateAmount, -45, 45)), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z - Mathf.Clamp(-dir * bodyRotateAmount, -45, 45))) * Time.deltaTime * bodyRotateSpeed);
                }
                else
                {
                    body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0,Mathf.Clamp(-dir * bodyRotateAmount, -45, 45)), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z - Mathf.Clamp(dir * bodyRotateAmount, -45, 45))) * Time.deltaTime * bodyRotateSpeed);
                }
            }

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(!hasCrashed)
    //    {
    //        hasCrashed = true;
    //        rb.useGravity = true;
    //        rb.velocity = Vector3.zero;
    //    }   
    //}
}
