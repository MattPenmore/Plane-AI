using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePathfinding : MonoBehaviour
{
    public GameObject target;

    public int checkPointsReached = 0;
    public int numCheckPoints;
    public int startPos;
    public int targetnumber;

    public List<GameObject> checkPoints;

    [SerializeField]
    public List<GameObject> startPositions;

    public Vector3 seekForce;
    float maxAccelleration = 50;

    [SerializeField]
    Timer timer;
    public bool reachedEnd = false;

    [SerializeField]
    NumReachedEnd numReachedEnd;
    bool hasCrashed = false;

    private void Awake()
    {
        //numReachedEnd = FindObjectOfType<NumReachedEnd>();
        //timer = FindObjectOfType<Timer>();
        numCheckPoints = checkPoints.Count;
        //cam = Camera.main;

        ResetPlane();
    }

    // Update is called once per frame
    void Update()
    {
        if(hasCrashed)
        {
            ResetPlane();
            return;
        }

        //Reset plane if it reaches end of course
        if (checkPointsReached > numCheckPoints && !reachedEnd)
        {
            reachedEnd = true;
            numReachedEnd.reachEndAmount++;
            ResetPlane();
        }
        else
        {
            //Loop checkpoints if neccesary
            if (targetnumber >= numCheckPoints)
            {
                targetnumber -= numCheckPoints;
            }

            target = checkPoints[targetnumber];
        }

        //set seek force in direciton of next checkpoint
        Vector3 seekDirection = new Vector3();
        seekDirection = (target.transform.position - transform.position).normalized * maxAccelleration;
        seekForce = seekDirection;
    }

    //Reset all values and place plane back at start of the course
    public void ResetPlane()
    {
        startPos = 1;//Random.Range(0, numCheckPoints);
        targetnumber = startPos;
        target = checkPoints[targetnumber];
        transform.position = startPositions[startPos].transform.position;
        transform.rotation = startPositions[startPos].transform.rotation;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        //Increment number of times plane as completed or failed the course as neccessary
        if (hasCrashed)
            numReachedEnd.crashedAmount++;
        if(reachedEnd)
            numReachedEnd.checkTime = timer.time;

        checkPointsReached = 0;
        reachedEnd = false;
        hasCrashed = false;
        timer.time = 0;
        //Reset checkpoint data on whether plane has reached it
        foreach (GameObject checkPoint in checkPoints)
        {
            if (checkPoint.GetComponentInChildren<CheckPoint>().planeReachedTarget.Count >= 1)
            {
                checkPoint.GetComponentInChildren<CheckPoint>().planeReachedTarget[System.Array.IndexOf(checkPoint.GetComponentInChildren<CheckPoint>().planes, GetComponent<PlanePathfinding>())] = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCrashed && !reachedEnd)
        {
            hasCrashed = true;;
        }
    }
}
