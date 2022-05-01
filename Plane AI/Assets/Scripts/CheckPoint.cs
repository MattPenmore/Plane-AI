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
            if (other.transform.root.GetComponent<ObstacleCourseAgent>().target == transform.parent.gameObject && MLPlaneReachedTarget[System.Array.IndexOf(MLPlanes, other.transform.root.GetComponent<ObstacleCourseAgent>())] == false)
            {
                other.transform.root.GetComponent<ObstacleCourseAgent>().checkPointsReached++;
                other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber++;
                MLPlaneReachedTarget[System.Array.IndexOf(MLPlanes, other.transform.root.GetComponent<ObstacleCourseAgent>())] = true;

                if (other.transform.root.GetComponent<ObstacleCourseAgent>().checkPointsReached <= other.transform.root.GetComponent<ObstacleCourseAgent>().numCheckPoints)
                {
                    if (other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber >= other.transform.root.GetComponent<ObstacleCourseAgent>().numCheckPoints)
                    {
                        other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber -= other.transform.root.GetComponent<ObstacleCourseAgent>().numCheckPoints;
                    }

                    other.transform.root.GetComponent<ObstacleCourseAgent>().target = other.transform.root.GetComponent<ObstacleCourseAgent>().checkPoints[other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber];
                }
            }
            else if (other.transform.root.GetComponent<ObstacleCourseAgent>().target == transform.parent.gameObject && other.transform.root.GetComponent<ObstacleCourseAgent>().numCheckPoints == other.transform.root.GetComponent<ObstacleCourseAgent>().checkPointsReached)
            {
                other.transform.root.GetComponent<ObstacleCourseAgent>().checkPointsReached++;
                other.transform.root.GetComponent<ObstacleCourseAgent>().targetnumber++;
                MLPlaneReachedTarget[System.Array.IndexOf(MLPlanes, other.transform.root.GetComponent<ObstacleCourseAgent>())] = true;
            }
        }

        if (other.transform.root.GetComponent<SteeringHybrid>())
        {
            if (other.transform.root.GetComponent<SteeringHybrid>().target == transform.parent.gameObject && HybridPlaneReachedTarget[System.Array.IndexOf(HybridPlanes, other.transform.root.GetComponent<SteeringHybrid>())] == false)
            {
                other.transform.root.GetComponent<SteeringHybrid>().checkPointsReached++;
                other.transform.root.GetComponent<SteeringHybrid>().targetnumber++;
                HybridPlaneReachedTarget[System.Array.IndexOf(HybridPlanes, other.transform.root.GetComponent<SteeringHybrid>())] = true;

                if (other.transform.root.GetComponent<SteeringHybrid>().checkPointsReached <= other.transform.root.GetComponent<SteeringHybrid>().numCheckPoints)
                {
                    if (other.transform.root.GetComponent<SteeringHybrid>().targetnumber >= other.transform.root.GetComponent<SteeringHybrid>().numCheckPoints)
                    {
                        other.transform.root.GetComponent<SteeringHybrid>().targetnumber -= other.transform.root.GetComponent<SteeringHybrid>().numCheckPoints;
                    }

                    other.transform.root.GetComponent<SteeringHybrid>().target = other.transform.root.GetComponent<SteeringHybrid>().checkPoints[other.transform.root.GetComponent<SteeringHybrid>().targetnumber];
                }
            }
            else if (other.transform.root.GetComponent<SteeringHybrid>().target == transform.parent.gameObject && other.transform.root.GetComponent<SteeringHybrid>().numCheckPoints == other.transform.root.GetComponent<SteeringHybrid>().checkPointsReached)
            {
                other.transform.root.GetComponent<SteeringHybrid>().checkPointsReached++;
                other.transform.root.GetComponent<SteeringHybrid>().targetnumber++;
                HybridPlaneReachedTarget[System.Array.IndexOf(HybridPlanes, other.transform.root.GetComponent<SteeringHybrid>())] = true;
            }
        }

        if (other.transform.root.GetComponent<PlanePathfinding>())
        {
            if(other.transform.root.GetComponent<PlanePathfinding>().target == transform.parent.gameObject && planeReachedTarget[System.Array.IndexOf(planes, other.transform.root.GetComponent<PlanePathfinding>())] == false)
            {
                other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached++;
                other.transform.root.GetComponent<PlanePathfinding>().targetnumber++;
                planeReachedTarget[System.Array.IndexOf(planes, other.transform.root.GetComponent<PlanePathfinding>())] = true;

                if (other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached <= other.transform.root.GetComponent<PlanePathfinding>().numCheckPoints)
                {
                    if (other.transform.root.GetComponent<PlanePathfinding>().targetnumber >= other.transform.root.GetComponent<PlanePathfinding>().numCheckPoints)
                    {
                        other.transform.root.GetComponent<PlanePathfinding>().targetnumber -= other.transform.root.GetComponent<PlanePathfinding>().numCheckPoints;
                    }

                    other.transform.root.GetComponent<PlanePathfinding>().target = other.transform.root.GetComponent<PlanePathfinding>().checkPoints[other.transform.root.GetComponent<PlanePathfinding>().targetnumber];
                }
            }
            else if(other.transform.root.GetComponent<PlanePathfinding>().target == transform.parent.gameObject && other.transform.root.GetComponent<PlanePathfinding>().numCheckPoints == other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached)
            {
                other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached++;
                other.transform.root.GetComponent<PlanePathfinding>().targetnumber++;
                planeReachedTarget[System.Array.IndexOf(planes, other.transform.root.GetComponent<PlanePathfinding>())] = true;
            }
        }

    }
}
