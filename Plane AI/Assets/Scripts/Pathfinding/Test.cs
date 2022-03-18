using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject[] Path;
    [SerializeField]
    GameObject pathfind;
    [SerializeField]
    GameObject Grid;
    [SerializeField]
    GameObject startNode;
    [SerializeField]
    GameObject endNode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            int rand = Random.Range(0, Grid.transform.childCount);
            startNode = Grid.transform.GetChild(rand).gameObject;

            rand = Random.Range(0, Grid.transform.childCount);
            endNode = Grid.transform.GetChild(rand).gameObject;

            Path = pathfind.GetComponent<Pathfind>().FindPath(startNode, endNode).ToArray();

            for(int i = 0; i < Path.Length - 1; i++)
            {
                Vector3 dir = Path[i + 1].transform.position - Path[i].transform.position;

                //Debug.DrawLine(Path[i].transform.position, Path[i + 1].transform.position, Color.red);
                Debug.DrawRay(Path[i].transform.position, dir, Color.blue);
            }
        }
    }
}
