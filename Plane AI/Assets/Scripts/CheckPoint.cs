using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public PlanePathfinding[] planes;
    public ObstacleCourseAgent[] MLPlanes;
    public SteeringHybrid[] HybridPlanes;
    public List<bool> planeReachedTarget = new List<bool>();
    public List<bool> MLPlaneReachedTarget = new List<bool>();
    public List<bool> HybridPlaneReachedTarget = new List<bool>();

    private void Awake()
    {
        //Get lists of each type of plane
        MLPlanes = FindObjectsOfType<ObstacleCourseAgent>();
        foreach (ObstacleCourseAgent MLPlane in MLPlanes)
        {
            MLPlaneReachedTarget.Add(false);
        }

        planes = FindObjectsOfType<PlanePathfinding>();
        foreach(PlanePathfinding plane in planes)
        {
            planeReachedTarget.Add(false);
        }

        HybridPlanes = FindObjectsOfType<SteeringHybrid>();
        foreach (SteeringHybrid plane in HybridPlanes)
        {
            HybridPlaneReachedTarget.Add(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.GetComponent<ObstacleCourseAgent>())
        {
            //If plane is aiming for this checkpoint and hasn't reached it previously
            if (other.transform.root.GetComponent<ObstacleCourseAgent>().target == transform.parent.gameObject && MLPlaneReachedTarget[System.Array.IndexOf(MLPlanes, other.transform.root.GetComponent<ObstacleCourseAgent>())] == false)
            {
                //Increase number of checkpoints reached, and tell plane to aim for the next checkpoint.
                other.transform.root.GetComponent<ObstacleCourseAgent>().checkPointsReached++;
                other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber++;
                //Mark down the plane as having reached this checkpoint.
                MLPlaneReachedTarget[System.Array.IndexOf(MLPlanes, other.transform.root.GetComponent<ObstacleCourseAgent>())] = true;

                //Loop planes checkpoint target if this is the last checkpoint in it's list
                if (other.transform.root.GetComponent<ObstacleCourseAgent>().checkPointsReached <= other.transform.root.GetComponent<ObstacleCourseAgent>().numCheckPoints)
                {
                    if (other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber >= other.transform.root.GetComponent<ObstacleCourseAgent>().numCheckPoints)
                    {
                        other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber -= other.transform.root.GetComponent<ObstacleCourseAgent>().numCheckPoints;
                    }

                    other.transform.root.GetComponent<ObstacleCourseAgent>().target = other.transform.root.GetComponent<ObstacleCourseAgent>().checkPoints[other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber];
                }
            }
            //If this is the start/finish checkpoint and the plane has reached this checkpoint for the finish of the lap, increment the planes checkpoints reached.
            else if (other.transform.root.GetComponent<ObstacleCourseAgent>().target == transform.parent.gameObject && other.transform.root.GetComponent<ObstacleCourseAgent>().numCheckPoints == other.transform.root.GetComponent<ObstacleCourseAgent>().checkPointsReached)
            {
                other.transform.root.GetComponent<ObstacleCourseAgent>().checkPointsReached++;
                other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber++;
                MLPlaneReachedTarget[System.Array.IndexOf(MLPlanes, other.transform.root.GetComponent<ObstacleCourseAgent>())] = true;
            }
        }

        if (other.transform.root.GetComponent<SteeringHybrid>())
        {
            //If plane is aiming for this checkpoint and hasn't reached it previously
            if (other.transform.root.GetComponent<SteeringHybrid>().target == transform.parent.gameObject && HybridPlaneReachedTarget[System.Array.IndexOf(HybridPlanes, other.transform.root.GetComponent<SteeringHybrid>())] == false)
            {
                //Increase number of checkpoints reached, and tell plane to aim for the next checkpoint.
                other.transform.root.GetComponent<SteeringHybrid>().checkPointsReached++;
                other.transform.root.GetComponent<SteeringHybrid>().targetnumber++;
                //Mark down the plane as having reached this checkpoint.
                HybridPlaneReachedTarget[System.Array.IndexOf(HybridPlanes, other.transform.root.GetComponent<SteeringHybrid>())] = true;

                //Loop planes checkpoint target if this is the last checkpoint in it's list
                if (other.transform.root.GetComponent<SteeringHybrid>().checkPointsReached <= other.transform.root.GetComponent<SteeringHybrid>().numCheckPoints)
                {
                    if (other.transform.root.GetComponent<SteeringHybrid>().targetnumber >= other.transform.root.GetComponent<SteeringHybrid>().numCheckPoints)
                    {
                        other.transform.root.GetComponent<SteeringHybrid>().targetnumber -= other.transform.root.GetComponent<SteeringHybrid>().numCheckPoints;
                    }

                    other.transform.root.GetComponent<SteeringHybrid>().target = other.transform.root.GetComponent<SteeringHybrid>().checkPoints[other.transform.root.GetComponent<SteeringHybrid>().targetnumber];
                }
            }
            //If this is the start/finish checkpoint and the plane has reached this checkpoint for the finish of the lap, increment the planes checkpoints reached.
            else if (other.transform.root.GetComponent<SteeringHybrid>().target == transform.parent.gameObject && other.transform.root.GetComponent<SteeringHybrid>().numCheckPoints == other.transform.root.GetComponent<SteeringHybrid>().checkPointsReached)
            {
                other.transform.root.GetComponent<SteeringHybrid>().checkPointsReached++;
                other.transform.root.GetComponent<SteeringHybrid>().targetnumber++;
                HybridPlaneReachedTarget[System.Array.IndexOf(HybridPlanes, other.transform.root.GetComponent<SteeringHybrid>())] = true;
            }
        }

        if (other.transform.root.GetComponent<PlanePathfinding>())
        {
            //If plane is aiming for this checkpoint and hasn't reached it previously
            if (other.transform.root.GetComponent<PlanePathfinding>().target == transform.parent.gameObject && planeReachedTarget[System.Array.IndexOf(planes, other.transform.root.GetComponent<PlanePathfinding>())] == false)
            {
                //Increase number of checkpoints reached, and tell plane to aim for the next checkpoint.
                other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached++;
                other.transform.root.GetComponent<PlanePathfinding>().targetnumber++;
                //Mark down the plane as having reached this checkpoint.
                planeReachedTarget[System.Array.IndexOf(planes, other.transform.root.GetComponent<PlanePathfinding>())] = true;

                //Loop planes checkpoint target if this is the last checkpoint in it's list
                if (other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached <= other.transform.root.GetComponent<PlanePathfinding>().numCheckPoints)
                {
                    if (other.transform.root.GetComponent<PlanePathfinding>().targetnumber >= other.transform.root.GetComponent<PlanePathfinding>().numCheckPoints)
                    {
                        other.transform.root.GetComponent<PlanePathfinding>().targetnumber -= other.transform.root.GetComponent<PlanePathfinding>().numCheckPoints;
                    }

                    other.transform.root.GetComponent<PlanePathfinding>().target = other.transform.root.GetComponent<PlanePathfinding>().checkPoints[other.transform.root.GetComponent<PlanePathfinding>().targetnumber];
                }
            }
            //If this is the start/finish checkpoint and the plane has reached this checkpoint for the finish of the lap, increment the planes checkpoints reached.
            else if (other.transform.root.GetComponent<PlanePathfinding>().target == transform.parent.gameObject && other.transform.root.GetComponent<PlanePathfinding>().numCheckPoints == other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached)
            {
                other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached++;
                other.transform.root.GetComponent<PlanePathfinding>().targetnumber++;
                planeReachedTarget[System.Array.IndexOf(planes, other.transform.root.GetComponent<PlanePathfinding>())] = true;
            }
        }

    }
}
