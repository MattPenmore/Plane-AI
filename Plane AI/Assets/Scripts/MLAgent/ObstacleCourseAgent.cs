using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class ObstacleCourseAgent : Agent
{
    public float reward;

    float rewardTime = 0;

    [SerializeField]
    PlaneSight sight;

    //[SerializeField]
    //GameObject body;

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

    List<Vector3> sightObservations = new List<Vector3>();
    Vector3 targetDirection;

    [SerializeField]
    List<GameObject> startPositions;
    public bool reachedEnd = false;

    float checkTime = 0;

    [SerializeField]
    float avoidanceStrength;

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
            sensor.AddObservation(transform.InverseTransformDirection(target.transform.position - transform.position)/sight.maxSight);
            targetDirection = target.transform.position - transform.position;
        }

        if(GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize == 27)
        {
            sensor.AddObservation(rb.velocity.normalized * (rb.velocity.magnitude - minVelocity)/(maxVelocity-minVelocity));
            foreach(float sightM in sight.sightMagnitudes)
            {
                sensor.AddObservation(sightM / sight.maxSight);
            }
        }
        else if(GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize == 69)
        {
            sensor.AddObservation(rb.velocity);
            foreach (Vector3 sight in sight.sightDirections)
            {
                sensor.AddObservation(sight);
            }
        }
        else if (GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize == 25)
        {
            sensor.AddObservation((rb.velocity.magnitude - minVelocity) / (maxVelocity - minVelocity));
            foreach (float sightM in sight.sightMagnitudes)
            {
                sensor.AddObservation(sightM / sight.maxSight);
                
            }

            sightObservations.Clear();
            foreach (Vector3 sight in sight.sightDirections)
            {
                sightObservations.Add(sight);
            }
        }
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        if (checkPointsReached > numCheckPoints && !reachedEnd)
        {
            reachedEnd = true;
        }

        if (targetnumber >= numCheckPoints)
        {
            targetnumber -= numCheckPoints;
        }

        target = checkPoints[targetnumber];

         Vector3 output = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]) * maxAcceleration;
         Vector3 newDirection = rb.velocity + transform.TransformDirection(output);
         Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
         //Quaternion oldRotation = transform.rotation;
         transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(newDirection), turnSpeed * Time.deltaTime);
         newDirection = transform.TransformDirection(localVel * newDirection.magnitude);
         //transform.rotation = oldRotation;

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
        checkTime = Time.time;

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
            AddReward(2000 + Mathf.Clamp(10000 / (Time.time - timeTargetReached), 0, 6000));
            reward += 2000 + Mathf.Clamp(10000 / (Time.time - timeTargetReached), 0, 6000);
            rewardTime = Time.time;
            timeGotCloser = Time.time;
            timeTargetReached = Time.time;
            Debug.Log(checkPointsReached);
        }

        if(reachedEnd)
        {
            AddReward(20000);
            reward += 20000;
            rewardTime = Time.time;
            EndEpisode();
        }
        else if(hasCrashed || transform.position.y >= 600 || Vector3.Magnitude(target.transform.position - transform.position) > 1000 || Time.time - rewardTime > 10 || Time.time - timeGotCloser > 10 || reward < -10 /*|| Time.time - timeTargetReached > 20*/)
        {
            //if(checkPointsReached != 0)
            //{
            //    if(targetnumber != 0)
            //    {
            //        AddReward(Mathf.Abs(Vector3.Magnitude(checkPoints[targetnumber].transform.position - checkPoints[targetnumber - 1].transform.position)) - Mathf.Abs(Vector3.Magnitude(checkPoints[targetnumber].transform.position - transform.position)));
            //    }
            //    else
            //    {
            //        AddReward(Mathf.Abs(Vector3.Magnitude(checkPoints[targetnumber].transform.position - checkPoints[numCheckPoints -1].transform.position)) - Mathf.Abs(Vector3.Magnitude(checkPoints[targetnumber].transform.position - transform.position)));
            //    }
            //}

            rewardTime = Time.time;
            timeTargetReached = Time.time;
            timeGotCloser = Time.time;
            EndEpisode();
        }
    }
    public override void Heuristic(float[] actionsOut)
    {
        Vector3 avoidanceDirection = Vector3.zero;
        int i = 0;
        foreach (Vector3 objPos in sightObservations)
        {
            float angleToPoint = Mathf.Abs(Vector3.Angle(sight.sightDirections[i], transform.TransformDirection(Vector3.forward)));

            avoidanceDirection -= (sight.sightDirections[i].normalized * sight.maxSight - sight.sightDirections[i]) / Mathf.Sqrt(angleToPoint) / sight.sightDirections[i].magnitude * avoidanceStrength;
            i++;
        }

        if (avoidanceDirection.magnitude > maxAcceleration)
            avoidanceDirection = avoidanceDirection.normalized * maxAcceleration;

        
        Vector3 avoidanceForce = avoidanceDirection;
        Vector3 seekForce = targetDirection.normalized * maxAcceleration;

        float avoidanceRatio = avoidanceForce.magnitude / maxAcceleration;
        float seekRatio = 1 - avoidanceRatio;
        Vector3 desiredDirection = avoidanceForce * avoidanceRatio + seekForce * seekRatio;

        Vector3 localDir = transform.InverseTransformDirection(desiredDirection) / maxAcceleration;

        actionsOut[0] = localDir.x;
        actionsOut[1] = localDir.y;
        actionsOut[2] = localDir.z;

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
            //Vector3 perp = Vector3.Cross(transform.forward, direction);
            //float dir = Vector3.Dot(perp, transform.up);

            //if (Mathf.Abs(dir) < 0.00005)
            //{
            //    body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0, 0), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z)) * Time.deltaTime * bodyRotateSpeed);
            //}
            //else
            //{
            //    if (dir < 0)
            //    {
            //        body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0, Mathf.Clamp(-dir * bodyRotateAmount, -45, 45)), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z - Mathf.Clamp(-dir * bodyRotateAmount, -45, 45))) * Time.deltaTime * bodyRotateSpeed);
            //    }
            //    else
            //    {
            //        body.transform.localRotation = Quaternion.Lerp(body.transform.localRotation, Quaternion.Euler(0, 0, Mathf.Clamp(-dir * bodyRotateAmount, -45, 45)), (1 / Mathf.Abs(body.transform.localRotation.eulerAngles.z - Mathf.Clamp(dir * bodyRotateAmount, -45, 45))) * Time.deltaTime * bodyRotateSpeed);
            //    }
            //}

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
        startPos = 1;//Random.Range(0, numCheckPoints);
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
        //body.transform.rotation = Quaternion.identity;
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
