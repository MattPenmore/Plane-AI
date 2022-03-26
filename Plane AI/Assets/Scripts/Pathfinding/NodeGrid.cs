using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeGrid : MonoBehaviour
{
    [SerializeField]
    float offsetX;
    [SerializeField]
    float offsetY;
    [SerializeField]
    float offsetZ;

    [SerializeField]
    float width;
    [SerializeField]
    float length;
    [SerializeField]
    float height;

    [SerializeField]
    float distBetweenNodes;

    [SerializeField]
    GameObject node;

    GameObject Grid;

    public bool creatingGrid = false;

    public List<GameObject> nodes = new List<GameObject>();

    int i = 0;
    int j = 0;
    int k = 0;

    [SerializeField]
    int numNodesPerFrame;

    int currentNodes = 0;

    [SerializeField]
    GameObject sphere;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    creatingGrid = true;
        //    Grid = new GameObject();
        //}

        currentNodes = 0;
        if(creatingGrid)
        {
            while (i <= width)
            {
                while (j <= height)
                {
                    while (k <= length)
                    {
                        if(currentNodes <= numNodesPerFrame)
                        {
                            Vector3 position = new Vector3(i * distBetweenNodes + offsetX, j * distBetweenNodes + offsetY, k * distBetweenNodes + offsetZ); ;
                            RaycastHit hit;
                            if (Physics.Raycast(position, Vector3.down, out hit, height * distBetweenNodes))
                            {
                                Debug.DrawRay(position, Vector3.down, Color.red);
                                if (hit.transform.tag == "Ground")
                                {
                                    CreateNode(i, j, k);
                                }
                            }
                            k++;
                            currentNodes++;
                        }
                        else
                        {
                            return;
                        }
                    }
                    if (k > length)
                    {
                        k = 0;
                        j++;
                    }
                }
                if (j > height)
                {
                    k = 0;
                    j = 0;
                    i++;
                }
            }

            if (i > width)
            {
                creatingGrid = false;
            }
        }
    }

    void CreateNode(int i, int j, int k)
    {
        Vector3 position = new Vector3(i * distBetweenNodes + offsetX, j * distBetweenNodes + offsetY, k * distBetweenNodes + offsetZ);
        GameObject nodePoint = Instantiate(node);
        nodePoint.transform.position = position;
        nodePoint.GetComponent<Node>().coordinates = new Vector3(i, j, k);
        nodePoint.GetComponent<Node>().controller = gameObject;
        nodePoint.transform.parent = Grid.transform;
    }

    void CreateGrid()
    {
        Grid = new GameObject();
        while (i <= width)
        {
            while (j <= height)
            {
                while (k <= length)
                {

                    Vector3 position = new Vector3(i * distBetweenNodes + offsetX, j * distBetweenNodes + offsetY, k * distBetweenNodes + offsetZ); ;
                    RaycastHit hit;
                    if (Physics.Raycast(position, Vector3.down, out hit, height * distBetweenNodes))
                    {
                        Debug.DrawRay(position, Vector3.down, Color.red);
                        if (hit.transform.tag == "Ground")
                        {
                            CreateNode(i, j, k);
                        }
                    }
                    k++;
                }
                if(k > length)
                {
                    k = 0;
                    j++;
                }
            }
            if (j > height)
            {
                k = 0;
                j = 0;
                i++;
            }
        }

        if(i > width)
        {
            creatingGrid = false;
        }
    }
}
