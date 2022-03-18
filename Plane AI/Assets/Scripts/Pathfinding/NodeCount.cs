using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCount : MonoBehaviour
{
    [SerializeField]
    int numNodes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        numNodes = transform.childCount;
    }
}
