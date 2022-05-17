using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSight : MonoBehaviour
{
    //int numRayCasts;
    int numRayCastsWidth = 7;
    int numRayCastsHeight = 3;

    [SerializeField]
    float sightWidth = 50;
    [SerializeField]
    float sightLength = 50;
    [SerializeField]
    float sightHeight = 30;

    public float maxSight = 100;

    public Rigidbody rb;

    public List<Vector3> sightDirections = new List<Vector3>();
    public float[] sightMagnitudes;

    // Update is called once per frame
    void Update()
    {
        int k = 0;
        sightMagnitudes = new float[numRayCastsWidth * numRayCastsHeight];
        //Clear previous data from sight directions
        sightDirections.Clear();
        //Iterate through raycasts based on height and width data
        for (int i = 0; i < numRayCastsWidth; i++)
        {
            for (int j = 0; j < numRayCastsHeight; j++)
            {
                //Get height and width of this raycast
                float curWidth = -sightWidth + (sightWidth * 2 / numRayCastsWidth * i);
                float curHeight = -sightHeight + (sightHeight * 2 / numRayCastsHeight * j);

                //Get direction of raycast
                Vector3 dir = (transform.right * curWidth) + (transform.up * curHeight) + (transform.forward * sightLength);

                //If raycast hits an object add relative position of hit to sight directions and add distance to sight magnitudes
                RaycastHit hit;
                if (Physics.Raycast(transform.position, dir.normalized, out hit, maxSight, ~6))
                {
                    //Debug.DrawRay(transform.position, dir, Color.red);
                    sightDirections.Add(dir.normalized * hit.distance);
                    sightMagnitudes[k] = hit.distance;
                }
                //Otherwise assume max distance as hit distance
                else
                {
                    sightDirections.Add(dir.normalized * maxSight);
                    sightMagnitudes[k] = maxSight;
                }
                k++;
            }
        }
    }
}
