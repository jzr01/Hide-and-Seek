using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerScript : MonoBehaviour
{
    public float normalspeed;
    public float sprintspeed;
    private float speed;

    public float stmina = 3.0f;
    public float relaxtime = 4.0f;

    private float timesincesprint = 0.0f;
    private float timesincerelax = 0.0f;

    private Collider[] hitcolliders;
    private RaycastHit Hit;

    public float detectrange;

    public Rigidbody rb;
    public GameObject target;

    private bool seehider;

    public float directionChangeInterval = 2f; // Time in seconds before changing direction

    private float timeSinceLastDirectionChange = 0.0f;
    private Vector3 currentDirection = Vector3.forward.normalized;

    private Vector3 lastKnownHiderPosition;


    // Start is called before the first frame update
    void Start()
    {
        speed = normalspeed;
    }

    // Update is called once per frame
    void Update()
    {
        // detect if there a hider in range 
        if (!seehider)
        {
            speed = normalspeed;
            // Move the object in the current direction
            transform.Translate(currentDirection * speed * Time.deltaTime);

            // Update the timer
            timeSinceLastDirectionChange += Time.deltaTime;

            // Check if it's time to change direction
            if (timeSinceLastDirectionChange >= directionChangeInterval)
            {
                // Change to a new random direction
                currentDirection = Random.insideUnitSphere.normalized;

                // Reset the timer
                timeSinceLastDirectionChange = 0.0f;
            }

            hitcolliders = Physics.OverlapSphere(transform.position, detectrange);
            foreach (var hitcollider in hitcolliders)
            {
                if (hitcollider.tag == "Hider")
                {
                    target = hitcollider.gameObject;
                    seehider = true;
                    lastKnownHiderPosition = target.transform.position;
                }
            }
        }
        else
        {
            
            if (timesincesprint <= stmina)
            {
                speed = sprintspeed;
                timesincesprint += Time.deltaTime;
            }
            else
            {
                if (timesincerelax < relaxtime)
                {
                    speed = normalspeed;
                    timesincerelax += Time.deltaTime;

                }
                else
                {
                    speed = sprintspeed;
                    timesincesprint = 0.0f;
                    timesincerelax = 0.0f;
                }

            }


            var Heading = target.transform.position - transform.position;
            var Distance = Heading.magnitude;
            var Direction = Heading / Distance;


            Vector3 Move = new Vector3(Direction.x * speed, 0, Direction.z * speed);
            rb.velocity = Move;
            transform.forward = Move;
            
            seehider = false;

            hitcolliders = Physics.OverlapSphere(transform.position, detectrange);
            foreach (var hitcollider in hitcolliders)
            {
                if (hitcollider.tag == "Hider")
                {
                    target = hitcollider.gameObject;
                    seehider = true;
                }
            }
        }
 
    }

    // Collision detection with the Hider
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hider"))
        {
            normalspeed = 0;
            sprintspeed = 0;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // Calculate the reflection vector off the wall
            Vector3 normal = collision.contacts[0].normal;
            currentDirection = Vector3.Reflect(currentDirection, normal);
        }
    }

}
