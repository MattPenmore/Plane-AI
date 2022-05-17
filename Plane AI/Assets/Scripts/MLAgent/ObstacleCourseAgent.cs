using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class ObstacleCourseAgent : Agent
{
    public float reward;
    bool resetting;
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

    [SerializeField]
    NumReachedEnd numReachedEnd;

    [SerializeField]
    Timer timer;

    public override void Initialize()
    {
        defaultParameters = Academy.Instance.EnvironmentParameters;
        rb = GetComponent<Rigidbody>();
        numCheckPoints = checkPoints.Count;
        ResetPlane();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //Used for reinforcement V2
        if(GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize == 27)
        {
            //If plane has a target add observation of direction between plane and checkpoint
            if (target)
            {
                sensor.AddObservation(transform.InverseTransformDirection(target.transform.position - transform.position));
                targetDirection = target.transform.position - transform.position;
            }
            //Add observation of plane velocity and normalize to between 0 and 1
            sensor.AddObservation(rb.velocity.normalized * (rb.velocity.magnitude - minVelocity)/(maxVelocity-minVelocity));
            //Add observation of distance for each raycast point and normalise to between 0 and 1
            foreach(float sightM in sight.sightMagnitudes)
            {
                sensor.AddObservation(sightM / sight.maxSight);
            }
        }
        //Used for reinforcement V1
        else if(GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize == 69)
        {
            //If plane has a target add observation of local direction between plane and checkpoint
            if (target)
            {
                sensor.AddObservation(target.transform.position - transform.position);
                targetDirection = target.transform.position - transform.position;
            }
            //Add observation of plane velocity
            sensor.AddObservation(rb.velocity);
            //Add observation of relative position from plane of each raycast point
            foreach (Vector3 sight in sight.sightDirections)
            {
                sensor.AddObservation(sight);
            }
        }
        //Used for reinforcement V3 and imitation
        else if (GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize == 25 && !resetting)
        {
            //If plane has a target add observation of local direction between plane and checkpoint
            if (target)
            {
                sensor.AddObservation(transform.InverseTransformDirection(target.transform.position - transform.position) / sight.maxSight);
                targetDirection = target.transform.position - transform.position;
            }
            //Add observation of plane velocity magnitude and normalize to between 0 and 1
            sensor.AddObservation((rb.velocity.magnitude - minVelocity) / (maxVelocity - minVelocity));
            //Add observation of distance for each raycast point and normalise to between 0 and 1
            foreach (float sightM in sight.sightMagnitudes)
            {
                sensor.AddObservation(sightM / sight.maxSight);
                
            }

            //create list of sight positions for heuristic method
            sightObservations.Clear();
            foreach (Vector3 sight in sight.sightDirections)
            {
                sightObservations.Add(sight);
            }
        }
        resetting = false;
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        //If plane is moving and application has been running long enough
        if(Time.time > 10 && rb.velocity != Vector3.zero)
        {
            //Check if plane has reached end of course
            if (checkPointsReached > numCheckPoints && !reachedEnd)
            {
                reachedEnd = true;
            }

            //Loop checkpoints if neccesary
            if (targetnumber >= numCheckPoints)
            {
                targetnumber -= numCheckPoints;
            }

            target = checkPoints[targetnumber];

            //Set output of neural network as directional velocity.
            Vector3 output = new Vector3(vectorAction[0], vectorAction[1], vectorAction[2]) * maxAcceleration;
            //Add output velocity and previous velocity to get new direction
            Vector3 newDirection = rb.velocity + transform.TransformDirection(output);
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
            else if (newDirection.magnitude < minVelocity)
            {
                rb.velocity = newDirection.normalized * minVelocity;
            }
            else
            {
                rb.velocity = newDirection;
            }

            curSpeed = rb.velocity.magnitude;
            checkTime = Time.time;

            //If plane hasn't reached a new checkpoint
            if (oldTarget == target)
            {
                //Get distance to next checkpoint
                distanceToTarget = Vector3.Magnitude(target.transform.position - transform.position);
                //Reward agent if it gets closer to target
                if (distanceToTarget < prevDistance)
                {
                    AddReward(prevDistance - distanceToTarget);
                    reward += prevDistance - distanceToTarget;
                    rewardTime = Time.time;
                    //Set closet distrance to target plane has reached
                    if(distanceToTarget < prevDistanceReached)
                    {
                        prevDistanceReached = distanceToTarget;
                        timeGotCloser = Time.time;
                    }
                    prevDistance = distanceToTarget;
                }
                //Punush plane if further from target
                else if(prevDistance < distanceToTarget)
                {
                    AddReward(prevDistance - distanceToTarget);
                    reward += prevDistance - distanceToTarget;

                    prevDistance = distanceToTarget;
                }
            }
            //If plane has reached a new checkpoint
            else
            {
                oldTarget = target;
                //Get distance to gext target
                distanceToTarget = Vector3.Magnitude(target.transform.position - transform.position);
                prevDistanceReached = distanceToTarget;
                //Reward agent based on time taked to reach checkpoint
                AddReward(2000 + Mathf.Clamp(10000 / (Time.time - timeTargetReached), 0, 6000));
                reward += 2000 + Mathf.Clamp(10000 / (Time.time - timeTargetReached), 0, 6000);
                rewardTime = Time.time;
                timeGotCloser = Time.time;
                timeTargetReached = Time.time;
                Debug.Log(checkPointsReached);
            }
            //If plane takes too long to reach a new checkpoint, treat it as being stuck and having crashed
            if (Time.time - timeTargetReached > 20)
                hasCrashed = true;

            //Reward and reset plane if it reaches end of course
            if(reachedEnd)
            {
                AddReward(20000);
                reward += 20000;
                rewardTime = Time.time;

                numReachedEnd.reachEndAmount++;
                ResetPlane();
                EndEpisode();
            }
            //Reset plane if it has crashed
            //Commented out code used for training
            else if(hasCrashed /*|| transform.position.y >= 600 || Vector3.Magnitude(target.transform.position - transform.position) > 1000 || Time.time - rewardTime > 10 || Time.time - timeGotCloser > 10 || reward < -10*/ /*|| Time.time - timeTargetReached > 20*/)
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
                ResetPlane();
                EndEpisode();
            }
        }
        else if( Time.time < 10)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = (target.transform.position - startPositions[startPos].transform.position).normalized;
        }
    }
    public override void Heuristic(float[] actionsOut)
    {
        //Reset avoidance direction
        Vector3 avoidanceDirection = Vector3.zero;
        int i = 0;
        //Add to avoidance force for each sight point, depending on distance and angle
        foreach (Vector3 objPos in sightObservations)
        {
            float angleToPoint = Mathf.Abs(Vector3.Angle(sight.sightDirections[i], transform.TransformDirection(Vector3.forward)));

            avoidanceDirection -= (sight.sightDirections[i].normalized * sight.maxSight - sight.sightDirections[i]) / Mathf.Sqrt(angleToPoint) / sight.sightDirections[i].magnitude * avoidanceStrength;
            i++;
        }

        //Limit force to the max accelleration
        if (avoidanceDirection.magnitude > maxAcceleration)
            avoidanceDirection = avoidanceDirection.normalized * maxAcceleration;

        //Set seek and avoidance force
        Vector3 avoidanceForce = avoidanceDirection;
        Vector3 seekForce = targetDirection.normalized * maxAcceleration;

        //Set ratio of forces based on avoidance strength
        float avoidanceRatio = avoidanceForce.magnitude / maxAcceleration;
        float seekRatio = 1 - avoidanceRatio;

        //Combine forces into a single direction and localise
        Vector3 desiredDirection = avoidanceForce * avoidanceRatio + seekForce * seekRatio;
        Vector3 localDir = transform.InverseTransformDirection(desiredDirection) / maxAcceleration;

        //Set neural net output to localised desired direciton
        actionsOut[0] = localDir.x;
        actionsOut[1] = localDir.y;
        actionsOut[2] = localDir.z;

    }
    public override void OnEpisodeBegin()
    {
        reward = 0;
        timeGotCloser = Time.time;
        ResetPlane();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCrashed)
        {
            hasCrashed = true;
        }
    }

    //Reset all values and place plane back at start of the course
    public void ResetPlane()
    {
        resetting = true;
        startPos = 1;//Random.Range(0, numCheckPoints);
        targetnumber = startPos;
        target = checkPoints[targetnumber];
        oldTarget = target;
        transform.position = startPositions[startPos].transform.position;
        transform.rotation = startPositions[startPos].transform.rotation;       
        checkPointsReached = 0;

        //Increment number of times plane as completed or failed the course as neccessary
        if (hasCrashed)
            numReachedEnd.crashedAmount++;

        if (reachedEnd)
            numReachedEnd.checkTime = timer.time;

        reachedEnd = false;
        hasCrashed = false;
        timer.time = 0;
        distanceToTarget = Vector3.Magnitude(target.transform.position - transform.position);
        prevDistanceReached = distanceToTarget;
        prevDistance = distanceToTarget;
        rb.velocity = Vector3.zero;

        //Reset checkpoint data on whether plane has reached it.
        foreach ( GameObject checkPoint in checkPoints)
        {
            if(checkPoint.GetComponentInChildren<CheckPoint>().MLPlaneReachedTarget.Count >= 1)
            {
                checkPoint.GetComponentInChildren<CheckPoint>().MLPlaneReachedTarget[System.Array.IndexOf(checkPoint.GetComponentInChildren<CheckPoint>().MLPlanes, GetComponent<ObstacleCourseAgent>())] = false;
            }
        }
    }
}
