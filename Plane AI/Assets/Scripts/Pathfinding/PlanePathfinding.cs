using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePathfinding : MonoBehaviour
{
    [SerializeField]
    GameObject currentNode;


    float timeToPathFind = 0.1f;
    float timeUntilPathfind = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, currentNode.transform.position) > 25)
        {
            currentNode = FindCurrentNode();
            timeUntilPathfind = timeToPathFind;
        }


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
