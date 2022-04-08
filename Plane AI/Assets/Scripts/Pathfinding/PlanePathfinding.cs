using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePathfinding : MonoBehaviour
{
    public GameObject target;
    [SerializeField]
    Camera cam;

    public int checkPointsReached = 0;
    public int numCheckPoints;
    int startPos;
    public int targetnumber;

    public List<GameObject> checkPoints;

    [SerializeField]
    List<GameObject> startPositions;

    public Vector3 seekForce;
    float maxAccelleration = 50;

    Timer timer;
    public bool reachedEnd = false;

    NumReachedEnd numReachedEnd;
    bool hasCrashed = false;

    private void Awake()
    {
        numReachedEnd = FindObjectOfType<NumReachedEnd>();
        timer = FindObjectOfType<Timer>();
        numCheckPoints = checkPoints.Count;
        cam = Camera.main;

        ResetPlane();
    }

    // Update is called once per frame
    void Update()
    {
        if(checkPointsReached > numCheckPoints && !reachedEnd)
        {
            //timer.reachedEnd = true;
            timer.time = 0;
            reachedEnd = true;
            numReachedEnd.reachEndAmount++;
            ResetPlane();
        }
        else
        {
            if(targetnumber >= numCheckPoints)
            {
                targetnumber -= numCheckPoints;
            }

            target = checkPoints[targetnumber];
        }

        Vector3 seekDirection = new Vector3();
        seekDirection = (target.transform.position - transform.position).normalized * maxAccelleration;
        seekForce = seekDirection;

    }

    public void ResetPlane()
    {
        startPos = 1;//Random.Range(0, numCheckPoints);
        targetnumber = startPos;
        target = checkPoints[targetnumber];
        transform.position = startPositions[startPos].transform.position;
        transform.rotation = startPositions[startPos].transform.rotation;
        //cam.transform.rotation = startPositions[startPos].transform.rotation;
        //cam.transform.position = startPositions[startPos].transform.position - startPositions[startPos].transform.forward * 80 + startPositions[startPos].transform.up * 80;
        checkPointsReached = 0;
        reachedEnd = false;
        hasCrashed = false;

        GetComponent<Rigidbody>().velocity = (target.transform.position - startPositions[startPos].transform.position).normalized * 50;

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
            numReachedEnd.crashedAmount++;
            ResetPlane();
        }
    }
}
