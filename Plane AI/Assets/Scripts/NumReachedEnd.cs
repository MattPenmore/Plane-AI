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

    public float checkTime;
    float totalTime;
    public float bestTime;
    public float averageTime;

    [SerializeField]
    Text reachedEnd;
    [SerializeField]
    Text crashed;
    [SerializeField]
    Text bestTimeText;
    [SerializeField]
    Text averageTimeText;

    // Start is called before the first frame update
    void Start()
    {
        bestTime = Mathf.Infinity;
    }

    // Update is called once per frame
    void Update()
    {
        //When a new time comes in, set the best and average time for the plane
        if(checkTime != 0)
        {
            totalTime += checkTime;
            averageTime = totalTime / reachEndAmount;
            if(checkTime < bestTime)
            {
                bestTime = checkTime;
            }
            checkTime = 0;
        }

        //Set UI text
        reachedEnd.text = "Finished: " + reachEndAmount.ToString();
        crashed.text = "Crashed: " + crashedAmount.ToString();
        bestTimeText.text = "Best Time: " + bestTime.ToString("F2");
        averageTimeText.text = "Average Time: " + averageTime.ToString("F2");
    }
}
