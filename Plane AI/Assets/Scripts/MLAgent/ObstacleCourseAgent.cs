using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class ObstacleCourseAgent : Agent
{
    [SerializeField]
    float reward;

    float rewardTime = 0;

    [SerializeField]
    PlaneSight sight;

    [SerializeField]
    GameObject body;

    [SerializeField]
    float bodyRotateSpeed;

    [SerializeField]
    float bodyRotateAmount;

    Rigidbody rb;
    EnvironmentParameters defaultParameters;

    public float maxVelocity = 100;
    public float minVelocity = 50;

    public float maxAcceleration = 60;

    public float turnSpeed = 60f;
    public float angle;

    public GameObject target;
    public GameObject oldTarget;
    float prevDistance;
    float distanceToTarget;
    float prevDistanceReached;
    float timeGotCloser = 0;

    float timeTargetReached = 0;
    bool hasCrashed = false;
    public float curSpeed;


    public int checkPointsReached = 0;
    public int numCheckPoints;
    int startPos;
    public int targetnumber;

    public List<GameObject> checkPoints;

    [SerializeField]
    List<GameObject> startPositions;
    public bool reachedEnd = false;

    public override void Initialize()
    {
        defaultParameters = Academy.Instance.EnvironmentParameters;
        rb = GetComponent<Rigidbody>();
        numCheckPoints = checkPoints.Count;
        ResetPlane();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        if(target)
        {
            sensor.AddObservation(target.transform.position - transform.position);
        }
        sensor.AddObservation(rb.velocity);
        foreach(Vector3 sight in sight.sightDirections)
        {
            sensor.AddObservation(sight);
        }
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        if (checkPointsReached > numCheckPoints && !reachedEnd)
        {
            reachedEnd = true;
        }
        else
        {
            if (targetnumber >= numCheckPoints)
            {
                targetnumber -= numCheckPoints;
            }

            target = checkPoints[targetnumber];
        }

        Vector3 newDirection = rb.velocity + new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]) * maxAcceleration;
        angle = Vector3.Angle(newDirection, transform.TransformDirection(Vector3.forward));

        if (angle > (turnSpeed))
        {
            newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, (turnSpeed / angle) * Time.deltaTime, 0.0f) * newDirection.magnitude;
        }
        else
        {
            newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, Time.deltaTime, 0.0f) * newDirection.magnitude;
        }

        if (newDirection.magnitude > maxVelocity)
        {
            rb.velocity = newDirection.normalized * maxVelocity;
        }
        else if (newDirection.magnitude < minVelocity)
        {
            rb.velocity = newDirection.normalized * minVelocity;
        }
        else
        {
            rb.velocity = newDirection;
        }

        curSpeed = rb.velocity.magnitude;
        LookAtDirection();

        if (oldTarget == target)
        {
            distanceToTarget = Vector3.Magnitude(target.transform.position - transform.position);
            if (distanceToTarget < prevDistance)
            {
                AddReward(prevDistance - distanceToTarget);
                reward += prevDistance - distanceToTarget;
                rewardTime = Time.time;
                if(distanceToTarget < prevDistanceReached)
                {
                    prevDistanceReached = distanceToTarget;
                    timeGotCloser = Time.time;
                }
                prevDistance = distanceToTarget;
            }
            else if(prevDistance < distanceToTarget)
            {
                AddReward(prevDistance - distanceToTarget);
                reward += prevDistance - distanceToTarget;

                prevDistance = distanceToTarget;

            }
        }
        else
        {
            oldTarget = target;
            distanceToTarget = Vector3.Magnitude(target.transform.position - transform.position);
            prevDistanceReached = distanceToTarget;
            AddReward(10000 + Mathf.Clamp(50000 / (Time.time - timeTargetReached), 0, 50000));
            reward += 10000 + Mathf.Clamp(50000 / (Time.time - timeTargetReached), 0, 50000);
            rewardTime = Time.time;
            timeGotCloser = Time.time;
            timeTargetReached = Time.time;
            Debug.Log(checkPointsReached);
        }

        if(reachedEnd)
        {
            AddReward(100000);
            reward += 100000;
            rewardTime = Time.time;
            EndEpisode();
        }
        else if(hasCrashed || transform.position.y >= 600 || Vector3.Magnitude(target.transform.position - transform.position) > 1000 || Time.time - rewardTime > 10)
        {
            rewardTime = Time.time;
            timeTargetReached = Time.time;
            timeGotCloser = Time.time;
            EndEpisode();
        }
        else if(Time.time - timeGotCloser > 10 || reward < -10)
        {
            rewardTime = Time.time;
            timeTargetReached = Time.time;
            timeGotCloser = Time.time;
            AddReward(-1000);
            EndEpisode();
        }
    }
    public override void Heuristic(float[] actionsOut)
    {
        Vector3 newDirection = rb.velocity + new Vector3(actionsOut[0], actionsOut[1], actionsOut[2]) * maxAcceleration;
        angle = Vector3.Angle(newDirection, transform.TransformDirection(Vector3.forward));

        if (angle > (turnSpeed))
        {
            newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, (turnSpeed / angle) * Time.deltaTime, 0.0f) * newDirection.magnitude;
        }
        else
        {
            newDirection = Vector3.RotateTowards(transform.TransformDirection(Vector3.forward), newDirection.normalized, Time.deltaTime, 0.0f) * newDirection.magnitude;
        }

        if (newDirection.magnitude > maxVelocity)
        {
            rb.velocity = newDirection.normalized * maxVelocity;
        }
        else if (newDirection.magnitude < minVelocity)
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
    public override void OnEpisodeBegin()
    {
        reward = 0;
        timeGotCloser = Time.time;
        //pathfinding = GetComponent<PlanePathfinding>();
        ResetPlane();
        
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

            if (Mathf.Abs(dir) < 0.00005)
            {
                body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0, 0), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z)) * Time.deltaTime * bodyRotateSpeed);
            }
            else
            {
                if (dir < 0)
                {
                    body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0, Mathf.Clamp(-dir * bodyRotateAmount, -45, 45)), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z - Mathf.Clamp(-dir * bodyRotateAmount, -45, 45))) * Time.deltaTime * bodyRotateSpeed);
                }
                else
                {
                    body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0, Mathf.Clamp(-dir * bodyRotateAmount, -45, 45)), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z - Mathf.Clamp(dir * bodyRotateAmount, -45, 45))) * Time.deltaTime * bodyRotateSpeed);
                }
            }

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCrashed)
        {
            hasCrashed = true;
        }
    }

    public void ResetPlane()
    {
        startPos = Random.Range(1,numCheckPoints);
        targetnumber = startPos;
        target = checkPoints[targetnumber];
        oldTarget = target;
        transform.position = startPositions[startPos].transform.position;
        transform.rotation = startPositions[startPos].transform.rotation;
        //cam.transform.rotation = startPositions[startPos].transform.rotation;
        //cam.transform.position = startPositions[startPos].transform.position - startPositions[startPos].transform.forward * 80 + startPositions[startPos].transform.up * 80;
        checkPointsReached = 0;
        reachedEnd = false;
        hasCrashed = false;
        distanceToTarget = Vector3.Magnitude(target.transform.position - transform.position);
        prevDistanceReached = distanceToTarget;
        prevDistance = distanceToTarget;
        rb.velocity = (target.transform.position - startPositions[startPos].transform.position).normalized * minVelocity;

        foreach ( GameObject checkPoint in checkPoints)
        {
            if(checkPoint.GetComponentInChildren<CheckPoint>().MLPlaneReachedTarget.Count >= 1)
            {
                checkPoint.GetComponentInChildren<CheckPoint>().MLPlaneReachedTarget[System.Array.IndexOf(checkPoint.GetComponentInChildren<CheckPoint>().MLPlanes, GetComponent<ObstacleCourseAgent>())] = false;
            }
        }
    }
}
