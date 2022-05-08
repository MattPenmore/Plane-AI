using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumReachedEnd : MonoBehaviour
{
    [SerializeField]
    string name;

    public int reachEndAmount = 0;
    public int crashedAmount = 0;

    [SerializeField]
    Text reachedEnd;
    [SerializeField]
    Text crashed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        reachedEnd.text = "Finished: " + reachEndAmount.ToString();
        crashed.text = "Crashed: " + crashedAmount.ToString();
    }
}
