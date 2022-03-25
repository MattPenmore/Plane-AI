using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField]
    GameObject nextCheckpoint;

    bool checkpointReached = false;

    public GameObject connectedNode;
    Timer timer;

    private void Awake()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Node");
        float closestDist = Mathf.Infinity;
        GameObject closest = null;
        timer = FindObjectOfType<Timer>();
        foreach(GameObject go in gos)
        {
            if(Vector3.Distance(gameObject.transform.position, go.transform.position) < closestDist)
            {
                closestDist = Vector3.Distance(gameObject.transform.position, go.transform.position);
                closest = go;
            }
        }

        connectedNode = closest;

        transform.position = connectedNode.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.GetComponent<PlanePathfinding>() && !checkpointReached)
        {
            if(other.transform.root.GetComponent<PlanePathfinding>().target == transform.parent.gameObject)
            {
                if(nextCheckpoint != null)
                {
                    other.transform.root.GetComponent<PlanePathfinding>().target = nextCheckpoint;
                }
                else
                {
                    timer.reachedEnd = true;
                }
                checkpointReached = true;
            }
        }
    }
}
