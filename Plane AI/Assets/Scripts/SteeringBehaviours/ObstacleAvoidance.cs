using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    [SerializeField]
    PlaneSight sight;
    float maxAccelleration = 50;

    public Vector3 avoidanceForce;

    // Start is called before the first frame update
    void Start()
    {
        
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
        foreach (Vector3 objPos in sight.sightDirections)
        {

            //avoidanceDirection -= Quaternion.AngleAxis(90, transform.TransformDirection(Vector3.up)) * (objPos * (sight.maxSight - objPos.magnitude));
            float angleToPoint = Mathf.Abs(Vector3.Angle(sight.sightDirections[i], transform.TransformDirection(Vector3.forward)));

            avoidanceDirection -= (sight.sightDirections[i].normalized * sight.maxSight / angleToPoint - sight.sightDirections[i] / angleToPoint);
            i++;
        }

        //float angle = Vector3.Angle(avoidanceDirection, transform.TransformDirection(Vector3.forward));
        //if (angle > 180)
        //    angle -= 360;
        //if (angle < -180)
        //    angle += 360;
        //if (angle > 0)
        //    avoidanceDirection = Quaternion.AngleAxis(90, transform.TransformDirection(Vector3.up)) * avoidanceDirection;
        //else
        //{
        //    avoidanceDirection = Quaternion.AngleAxis(-90, transform.TransformDirection(Vector3.up)) * avoidanceDirection;
        //}

        if (avoidanceDirection.magnitude > maxAccelleration)
            avoidanceDirection = avoidanceDirection.normalized * maxAccelleration;

        avoidanceForce = avoidanceDirection;
        //sight.rb.velocity += avoidanceDirection * Time.deltaTime;
    }
}
