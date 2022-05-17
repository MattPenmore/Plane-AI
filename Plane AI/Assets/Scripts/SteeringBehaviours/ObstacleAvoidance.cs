using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    [SerializeField]
    PlaneSight sight;
    float maxAccelleration = 50;
    [SerializeField]
    SteeringController controller;

    [SerializeField]
    float avoidanceStrength;

    public Vector3 avoidanceForce;

    // Start is called before the first frame update
    void Start()
    {
        maxAccelleration = controller.maxAcceleration;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 avoidanceDirection = new Vector3();
        if(sight.sightDirections.Count == 0)
        {
            avoidanceForce = Vector3.zero;
            return;
        }
        int i = 0;
        //Add to avoidance force for each sight point, depending on distance and angle
        foreach (Vector3 objPos in sight.sightDirections)
        {
            float angleToPoint = Mathf.Abs(Vector3.Angle(sight.sightDirections[i], transform.TransformDirection(Vector3.forward)));

            avoidanceDirection -= (sight.sightDirections[i].normalized * sight.maxSight - sight.sightDirections[i]) / Mathf.Sqrt(angleToPoint) / sight.sightDirections[i].magnitude * avoidanceStrength;
            i++;
        }

        //Limit avoidance force to max accepted
        if (avoidanceDirection.magnitude > maxAccelleration)
            avoidanceDirection = avoidanceDirection.normalized * maxAccelleration;

        avoidanceForce = avoidanceDirection;
    }
}
