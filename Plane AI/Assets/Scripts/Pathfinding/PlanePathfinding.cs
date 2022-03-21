using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePathfinding : MonoBehaviour
{
    [SerializeField]
    GameObject currentNode;

    GameObject oldTarget = null;
    public GameObject target;
    public GameObject[] Path;
    [SerializeField]
    GameObject pathfind;

    [SerializeField]
    SteeringController controller;

    public Vector3 seekForce;
    float maxAccelleration = 50;

    // Start is called before the first frame update
    void Start()
    {
        Path = new GameObject[500];
        if (target && currentNode)
        {
            if (target.GetComponentInChildren<CheckPoint>())
            {
                if(target.GetComponentInChildren<CheckPoint>().connectedNode)
                {
                    Path = pathfind.GetComponent<Pathfind>().FindPath(currentNode, target.GetComponentInChildren<CheckPoint>().connectedNode).ToArray();
                    oldTarget = target;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, currentNode.transform.position) > 35 || oldTarget != target)
        {
            currentNode = FindCurrentNode();
            if (target && currentNode)
            {
                if (target.GetComponentInChildren<CheckPoint>())
                {
                    if (target.GetComponentInChildren<CheckPoint>().connectedNode)
                    {
                        Path = pathfind.GetComponent<Pathfind>().FindPath(currentNode, target.GetComponentInChildren<CheckPoint>().connectedNode).ToArray();
                        oldTarget = target;
                    }
                }
            }
        }
        Vector3 seekDirection = new Vector3();
        maxAccelleration = controller.maxAcceleration;

        if (Path.Length <= 1)
        {
            seekDirection = (target.transform.position - transform.position).normalized * maxAccelleration;
        }
        else
        {
            seekDirection = (Path[1].transform.position - transform.position).normalized * maxAccelleration;
        }
        seekForce = seekDirection;
    }

    GameObject FindCurrentNode()
    {
        //Get list of all Nodes
        List<GameObject> gos;
        gos = currentNode.GetComponent<Node>().adjacentNodes;
        gos.Add(currentNode);

        float curDistance = Mathf.Infinity;
        GameObject closest = null;
        foreach(GameObject go in gos)
        {
            float dis = Vector3.Distance(gameObject.transform.position, go.transform.position);
            if (dis < curDistance)
            {
                curDistance = dis;
                closest = go;
            }
        }
        return closest;
        
    }
}
