using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfind : MonoBehaviour
{
    GameObject currentNode;
    public int maxPathLength = 500;

    public List<GameObject> FindPath(GameObject startPoint, GameObject endPoint)
    {
        //Get list of all Nodes
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Node");

        List<GameObject> openPathNodes = new List<GameObject>();
        List<GameObject> closedPathNodes = new List<GameObject>();

        currentNode = startPoint;

        // Add the start tile to the open list
        openPathNodes.Add(currentNode);

        currentNode.GetComponent<Node>().g = 0;
        currentNode.GetComponent<Node>().h = GetEstimatedPathCost(currentNode.transform.position, endPoint.transform.position);


        while (openPathNodes.Count != 0)
        {
            //sort open list
            openPathNodes = openPathNodes.OrderBy(x => x.GetComponent<Node>().f).ThenByDescending(x => x.GetComponent<Node>().g).ToList();
            currentNode = openPathNodes[0];

            //Remove current Node from open list and add to closed list
            openPathNodes.Remove(currentNode);
            closedPathNodes.Add(currentNode);

            //Limit number of hexagons that can be checked
            int g = currentNode.GetComponent<Node>().g - 1;
            if (g < -maxPathLength)
            {
                break;
            }

            //Check if endpoint is in closed list
            if (closedPathNodes.Contains(endPoint))
            {
                //We have found a path
                break;
            }

            currentNode.GetComponent<Node>().adjacentNodes = currentNode.GetComponent<Node>().FindAdjacentNodes();

            foreach (GameObject adjacentNode in currentNode.GetComponent<Node>().adjacentNodes)
            {
                //Check if  path to current Node can be reduced. if not continue. If so reduce it and break.
                if (closedPathNodes.Contains(adjacentNode))
                {
                    if (adjacentNode.GetComponent<Node>().g > currentNode.GetComponent<Node>().g + 1)
                    {
                        currentNode.GetComponent<Node>().g = adjacentNode.GetComponent<Node>().g - 1;
                        currentNode.GetComponent<Node>().previousNode = adjacentNode;
                        closedPathNodes.Remove(currentNode);

                        break;
                    }

                    continue;
                }
                // If not in open list add and compute g and h
                if (!(openPathNodes.Contains(adjacentNode)))
                {
                    adjacentNode.GetComponent<Node>().g = g;
                    adjacentNode.GetComponent<Node>().h = GetEstimatedPathCost(adjacentNode.transform.position, endPoint.transform.position);
                    openPathNodes.Add(adjacentNode);
                    adjacentNode.GetComponent<Node>().previousNode = currentNode;
                }
                //Otherwise check if F value can be lowered with current G
                else if (adjacentNode.GetComponent<Node>().f > g + adjacentNode.GetComponent<Node>().h)
                {
                    adjacentNode.GetComponent<Node>().g = g;
                    adjacentNode.GetComponent<Node>().previousNode = currentNode;
                }


            }
        }

        List<GameObject> finalPathNodes = new List<GameObject>();

        if (closedPathNodes.Contains(endPoint))
        {
            currentNode = endPoint;
            finalPathNodes.Add(currentNode);

            for (int i = endPoint.GetComponent<Node>().g + 1; i <= 0; i++)
            {
                foreach (GameObject Node in closedPathNodes)
                {
                    if (Node.GetComponent<Node>().g == i && Node.GetComponent<Node>().adjacentNodes.Contains(currentNode))
                    {
                        currentNode = Node;
                        finalPathNodes.Add(currentNode);
                    }
                }
            }
            finalPathNodes.Reverse();
        }
        return finalPathNodes;
    }

    static int GetEstimatedPathCost(Vector3 currentPosition, Vector3 endPosition)
    {
        Vector3 diff = currentPosition - endPosition;
        float curDistance = diff.sqrMagnitude;
        return Mathf.FloorToInt(curDistance);
    }
}