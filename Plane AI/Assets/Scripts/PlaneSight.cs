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
    // Start is called before the first frame update
    void Start()
    {
        
        //numRayCasts = numRayCastsWidth * numRayCastsHeight;
    }

    // Update is called once per frame
    void Update()
    {
        int k = 0;
        sightMagnitudes = new float[numRayCastsWidth * numRayCastsHeight];
        sightDirections.Clear();
        for (int i = 0; i < numRayCastsWidth; i++)
        {
            for (int j = 0; j < numRayCastsHeight; j++)
            {
                float curWidth = -sightWidth + (sightWidth * 2 / numRayCastsWidth * i);
                float curHeight = -sightHeight + (sightHeight * 2 / numRayCastsHeight * j);

                Vector3 dir = (transform.right * curWidth) + (transform.up * curHeight) + (transform.forward * sightLength);

                RaycastHit hit;
                if (Physics.Raycast(transform.position, dir.normalized, out hit, maxSight, ~6))
                {
                    Debug.DrawRay(transform.position, dir, Color.red);
                    sightDirections.Add(dir.normalized * hit.distance);
                    sightMagnitudes[k] = hit.distance;
                }
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
