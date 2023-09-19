using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideScipt : MonoBehaviour
{
    public float maxspeed;
    private float speed;

    private Collider[] hitcolliders;
    public Rigidbody rb;
    private bool seeplayer;
    public float detectrange;

    public GameObject target1;
    public GameObject target2;

    private Vector3 lastPosition;

    private List<GameObject> target_list = new List<GameObject>();
    void Start()
    {
        speed = maxspeed;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        lastPosition = transform.position;
        if (!seeplayer)
        {
    
            hitcolliders = Physics.OverlapSphere(transform.position, detectrange);
            foreach (var hitcollider in hitcolliders )
            {
                if (hitcollider.tag == "Seeker")
                {
                    target_list.Add(hitcollider.gameObject);
                    seeplayer = true;
                }
            }
        }
        else
        {

            if (target_list.Count == 1)
            {
                target1 = target_list[0];
                var Heading = target1.transform.position - transform.position;
                var Distance  = Heading.magnitude;
                var Direction = Heading / Distance;


                Vector3 Move = new Vector3(-Direction.x*speed,0,-Direction.z*speed);
                rb.velocity = Move;

                transform.forward  =  Move;
            }
            else
            {
                target1 = target_list[0];
                target2 = target_list[1];
                var A= target1.transform.position - transform.position;
                var B = target2.transform.position - transform.position;
                var Distance1 = A.magnitude;
                var Distance2 = B.magnitude;
   
                float theta = Mathf.Atan2(Vector3.Magnitude(Vector3.Cross(A, B)), Vector3.Dot(A, B));
                
                float theta1 = (2*Mathf.PI-theta)*Distance2/(Distance1+Distance2);

                float sinAngle = Mathf.Sin(theta1);
                float cosAngle = Mathf.Cos(theta1);

                float newX= A.x * cosAngle - A.z * sinAngle;
                float newZ = A.x * sinAngle + A.z * cosAngle;
                var Heading = new Vector3(newX, 0, newZ);

                var Direction = Heading / Heading.magnitude;


                Vector3 Move = new Vector3(Direction.x*speed,0,Direction.z*speed);
                rb.velocity = Move;

                transform.forward  =  Move;
            }
            
        }
    }

    void OnCollisionEnter(Collision collision)
            {
                if (collision.gameObject.CompareTag("Seeker"))
                {
                    speed = 0;
                }
                
                else if (collision.gameObject.CompareTag("Wall"))
                {
                    Debug.Log("Hit the wall");
                    Vector3 normal = collision.contacts[0].normal;
                    Debug.Log("Last Position: "+lastPosition);
                    Debug.Log("Current Position: "+transform.position);
                    Vector3 currentDirection = transform.position - lastPosition;
                    Debug.Log("Current Direction: "+currentDirection);

                    Vector3 afterdirection = Vector3.Reflect(currentDirection,normal);
                    Debug.Log("Afterward Direction: "+afterdirection);
                }
            }
}

