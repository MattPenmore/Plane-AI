using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    GameObject plane;

    [SerializeField]
    float distance;

    [SerializeField]
    float height;

    [SerializeField]
    float lerpSpeed;

    public bool isMLAgents;
    public ObstacleCourseAgent[] MLAgents;
    // Start is called before the first frame update
    void Start()
    {
        MLAgents = FindObjectsOfType<ObstacleCourseAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isMLAgents)
        {
            float bestReward = 0;

            foreach(ObstacleCourseAgent agent in MLAgents)
            {
                if(agent.reward > bestReward)
                {
                    bestReward = agent.reward;
                    plane = agent.gameObject;
                }
            }
        }

        Vector3 back = -plane.transform.forward * distance;
        Vector3 up = plane.transform.up * height; // this determines how high. Increase for higher view angle.
        transform.position = Vector3.Lerp(transform.position ,plane.transform.position + back + up, lerpSpeed * Time.deltaTime);

        transform.forward = plane.transform.position - transform.position;
    }
}
