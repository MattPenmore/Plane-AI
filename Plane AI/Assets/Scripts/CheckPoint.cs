using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    PlanePathfinding[] planes;
    public ObstacleCourseAgent[] MLPlanes;
    List<bool> planeReachedTarget = new List<bool>();
    public List<bool> MLPlaneReachedTarget = new List<bool>();

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
        else if (other.transform.root.GetComponent<PlanePathfinding>())
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
