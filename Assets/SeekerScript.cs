using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerScript : MonoBehaviour
{
    public float maxspeed;
    private float speed;

    private Collider[] hitcolliders;

    public float detectrange;
    public Rigidbody rb;
    public GameObject target;

    private bool seeplayer;


    // Start is called before the first frame update
    void Start()
    {
        speed = maxspeed;
    }

    // Update is called once per frame
    void Update()
    {
        // detect if there a hider in range 
        if (!seeplayer)
        {
            hitcolliders = Physics.OverlapSphere(transform.position, detectrange);
            foreach (var hitcollider in hitcolliders )
            {
                if (hitcollider.tag == "Hider")
                {
                    target = hitcollider.gameObject;
                    seeplayer = true;
                }
            }
        }
        else
        {
            var Heading = target.transform.position - transform.position;
            var Distance  = Heading.magnitude;
            var Direction = Heading / Distance;


            Vector3 Move = new Vector3(Direction.x*speed,0,Direction.z*speed);
            rb.velocity = Move;

            transform.forward  =  Move;

        }
    }

 // Collision detection with the Hider
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hider"))
        {
            speed = 0;
        }
    }

}
