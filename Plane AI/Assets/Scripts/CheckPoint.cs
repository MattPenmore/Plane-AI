using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    Timer timer;

    PlanePathfinding[] steeringPlanes;
    List<bool> planeReachedTarget = new List<bool>();

    private void Awake()
    {
        steeringPlanes = FindObjectsOfType<PlanePathfinding>();
        foreach(PlanePathfinding plane in steeringPlanes)
        {
            planeReachedTarget.Add(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.GetComponent<PlanePathfinding>())
        {
            if(other.transform.root.GetComponent<PlanePathfinding>().target == transform.parent.gameObject && planeReachedTarget[System.Array.IndexOf(steeringPlanes, other.transform.root.GetComponent<PlanePathfinding>())] == false)
            {
                other.transform.root.GetComponent<PlanePathfinding>().checkPointsReached++;
                other.transform.root.GetComponent<PlanePathfinding>().targetnumber++;
                planeReachedTarget[System.Array.IndexOf(steeringPlanes, other.transform.root.GetComponent<PlanePathfinding>())] = true;
            }
        }
    }
}
