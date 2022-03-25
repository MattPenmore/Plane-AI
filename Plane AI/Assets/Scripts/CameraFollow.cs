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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 back = -plane.transform.forward * distance;
        Vector3 up = plane.transform.up * height; // this determines how high. Increase for higher view angle.
        transform.position = Vector3.Lerp(transform.position ,plane.transform.position + back + up, lerpSpeed * Time.deltaTime);

        transform.forward = plane.transform.position - transform.position;
    }
}
