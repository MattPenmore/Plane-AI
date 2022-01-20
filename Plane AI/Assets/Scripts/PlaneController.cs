using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaneController : MonoBehaviour
{
    [SerializeField]
    GameObject[] planeBodyParts;

    [SerializeField]
    GameObject[] wheels;


    public float maxSpeed = 25f;

    private float minSpeed = 0;
    public float decceleration = 2.5f;

    public float acceleration = 2.5f;
    [SerializeField]
    private float activeSpeed = 0;

    public float verticalSensitivity;
    public float horizontalSensitivity;

    public float stallSpeed;

    Rigidbody rb;

    float fallTime = 0;
    public float fallSpeed;

    [SerializeField]
    bool onGround;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.Rotate(Input.GetAxis("Vertical") * verticalSensitivity * Time.deltaTime, 0.0f, -Input.GetAxis("Horizontal") * horizontalSensitivity * Time.deltaTime);

        if(Input.GetKey(KeyCode.Space))
        {
            activeSpeed = Mathf.Lerp(activeSpeed,maxSpeed, acceleration * Time.deltaTime);
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            activeSpeed = Mathf.Lerp(activeSpeed, minSpeed, decceleration * Time.deltaTime);
        }


        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity = new Vector3(0,0,activeSpeed);
        rb.velocity = transform.TransformDirection(localVelocity);

        float horizonatalSpeed = Mathf.Sqrt((rb.velocity.x * rb.velocity.x) + (rb.velocity.z * rb.velocity.z));
        if (horizonatalSpeed < stallSpeed && !onGround)
        {
            fallTime += Time.deltaTime;
            rb.AddForce(Vector3.down * fallSpeed * (stallSpeed - horizonatalSpeed) * rb.mass * fallTime);
        }
        else
        {
            fallTime = 0;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Collider myCollider = collision.contacts[0].thisCollider;
        // Now do whatever you need with myCollider.
        // (If multiple colliders were involved in the collision, 
        // you can find them all by iterating through the contacts)
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            List<GameObject> colliders = new List<GameObject>();
            foreach(ContactPoint contact in collision.contacts)
            {
                colliders.Add(contact.thisCollider.gameObject);
            }

            onGround = false;
            foreach(GameObject wheel in wheels)
            {
                if (colliders.Contains(wheel))
                {
                    onGround = true;
                }
            }
        }
    }
}
