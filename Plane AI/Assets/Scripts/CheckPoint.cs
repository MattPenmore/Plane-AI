using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    PlanePathfinding[] planes;
    List<bool> planeReachedTarget = new List<bool>();

    private void Awake()
    {
        planes = FindObjectsOfType<PlanePathfinding>();
        foreach(PlanePathfinding plane in planes)
        {
            planeReachedTarget.Add(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.GetComponent<PlanePathfinding>())
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
