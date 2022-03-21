using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 coordinates;
    public List<GameObject> adjacentNodes;

     public GameObject controller;

    public bool adjecentNodesKnown = false;

    public int f => -g + h;
    public int g;
    public int h;

    public GameObject previousNode;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(!controller.GetComponent<NodeGrid>().creatingGrid && !adjecentNodesKnown)
    //    {
    //        FindAdjacentNodes();
    //        adjecentNodesKnown = true;
    //    }
    //}

    public List<GameObject> FindAdjacentNodes()
    {
        adjacentNodes = new List<GameObject>();
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Node");


        if(coordinates.x == 59)
        {

        }
        foreach (GameObject go in gos)
        {
            //Get coordinates of Node
            Vector3 otherCoordinates = go.GetComponent<Node>().coordinates;
            //If within range, add to list of adjacent Nodes
            if (Mathf.Abs(otherCoordinates.x - coordinates.x) <= 1 && Mathf.Abs(otherCoordinates.y - coordinates.y) <= 1 && Mathf.Abs(otherCoordinates.z - coordinates.z) <= 1)
            {
                if(otherCoordinates != coordinates)
                {
                    RaycastHit hit;
                    Vector3 dir = go.transform.position - transform.position;

                    if (!Physics.Raycast(transform.position, dir.normalized, out hit, dir.magnitude))
                    {
                        adjacentNodes.Add(go);
                    }
                    else
                    {
                        Debug.DrawRay(transform.position, dir, Color.red);
                    }

                }
            }
        }
        return adjacentNodes;
    }

    public void SetPreviousNode(GameObject makePreviousNode)
    {
        previousNode = makePreviousNode;
    }
}
