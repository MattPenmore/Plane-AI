using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;

public class SteeringHybrid : Agent
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
    Vector3 avoidanceDirection;

    public List<GameObject> checkPoints;

    List<Vector3> sightObservations = new List<Vector3>();
    Vector3 targetDirection;

    [SerializeField]
    List<GameObject> startPositions;
    public bool reachedEnd = false;

    float checkTime = 0;

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
        //If plane has a target add observation of local direction between plane and checkpoint
        if (target)
        {
            sensor.AddObservation(transform.InverseTransformDirection(target.transform.position - transform.position) / sight.maxSight);
            targetDirection = target.transform.position - transform.position;
        }

        if (GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize == 28)
        {
            //Add observation of plane velocity magnitude and normalize to between 0 and 1
            sensor.AddObservation((rb.velocity.magnitude - minVelocity) / (maxVelocity - minVelocity));
            //Add observation of distance for each raycast point and normalise to between 0 and 1
            foreach (float sightM in sight.sightMagnitudes)
            {
                sensor.AddObservation(sightM / sight.maxSight);
            }
            //Reset avoidance direction
            avoidanceDirection = Vector3.zero;
            int i = 0;
            //Add to avoidance force for each sight point, depending on distance and angle
            foreach (Vector3 sightDir in sight.sightDirections)
            {
                float angleToPoint = Mathf.Abs(Vector3.Angle(sight.sightDirections[i], transform.TransformDirection(Vector3.forward)));

                avoidanceDirection -= (sight.sightDirections[i].normalized * sight.maxSight - sight.sightDirections[i]) / Mathf.Sqrt(angleToPoint) / sight.sightDirections[i].magnitude * 5;
                i++;
            }
            //Add observation of avoidance force
            Vector3 avoidanceForce = transform.InverseTransformDirection(avoidanceDirection) / maxAcceleration;
            sensor.AddObservation(avoidanceForce);
        }
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        //If plane is moving and application has been running long enough
        if (Time.time > 10 && rb.velocity != Vector3.zero)
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

            //Set outputs of neural network as avoidance ratio, avoidance strength and seek strength
            float avoidanceRatio = vectorAction[0];
            float avoidanceStrength = vectorAction[1] * 2;
            float seekStrength = vectorAction[2];

            //Set avoidance velocity as avoidance direction and strength.
            Vector3 avoidanceVector = avoidanceDirection * avoidanceStrength;
            if (avoidanceVector.magnitude > maxAcceleration)
                avoidanceVector = avoidanceVector.normalized * maxAcceleration;

            //combine original velocity, avoidance velocity and seek velocity, using outputs from neural net
            Vector3 newDirection = rb.velocity + targetDirection.normalized * maxAcceleration * (1 - avoidanceRatio) * seekStrength + avoidanceVector * avoidanceRatio;
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
        else if (Time.time < 10)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = (target.transform.position - startPositions[startPos].transform.position).normalized * minVelocity;
        }
    }
    public override void Heuristic(float[] actionsOut)
    {
        
    }
    public override void OnEpisodeBegin()
    {
        reward = 0;
        timeGotCloser = Time.time;
        //pathfinding = GetComponent<PlanePathfinding>();
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

        //Reset checkpoint data on whether plane has reached it
        foreach (GameObject checkPoint in checkPoints)
        {
            if (checkPoint.GetComponentInChildren<CheckPoint>().HybridPlaneReachedTarget.Count >= 1)
            {
                checkPoint.GetComponentInChildren<CheckPoint>().HybridPlaneReachedTarget[System.Array.IndexOf(checkPoint.GetComponentInChildren<CheckPoint>().HybridPlanes, GetComponent<SteeringHybrid>())] = false;
            }
        }
    }
}
