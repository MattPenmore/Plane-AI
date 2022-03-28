using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePathfinding : MonoBehaviour
{
    public GameObject target;
    Camera cam;

    public int checkPointsReached = 0;
    public int numCheckPoints;
    int startPos;
    public int targetnumber;

    public List<GameObject> checkPoints;

    [SerializeField]
    List<GameObject> startPositions;

    [SerializeField]
    SteeringController controller;

    public Vector3 seekForce;
    float maxAccelleration = 50;

    Timer timer;
    bool reachedEnd = false;

    NumReachedEnd numReachedEnd;
    bool hasCrashed = false;

    // Start is called before the first frame update
    void Start()
    {
        numReachedEnd = FindObjectOfType<NumReachedEnd>();
        timer = FindObjectOfType<Timer>();
        numCheckPoints = checkPoints.Count;
        maxAccelleration = controller.maxAcceleration;
        cam = Camera.main;

        startPos = Random.Range(0, numCheckPoints);
        targetnumber = startPos;
        transform.position = startPositions[startPos].transform.position;
        transform.rotation = startPositions[startPos].transform.rotation;
        cam.transform.rotation = startPositions[startPos].transform.rotation;
        cam.transform.position = startPositions[startPos].transform.position - startPositions[startPos].transform.forward * 80 + startPositions[startPos].transform.up * 80;
    }

    // Update is called once per frame
    void Update()
    {
        if(checkPointsReached > numCheckPoints && !reachedEnd)
        {
            timer.reachedEnd = true;
            reachedEnd = true;
            numReachedEnd.reachEndAmount++;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasCrashed && !reachedEnd)
        {
            hasCrashed = true;;
            numReachedEnd.crashedAmount++;
        }
    }
}
