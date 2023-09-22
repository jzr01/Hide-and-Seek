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
    public float walldetectrange;

    public GameObject target1;
    public GameObject target2;


    public List<GameObject> target_list = new List<GameObject>();

    public List<GameObject> wall_list = new List<GameObject>();
    public List<int> wall_number_list =new List<int> { 0, 0};

    private Vector3 lastDirection;
    private Vector3 Direction;
    float angle;
    bool cw;
    GameObject old_wall = null;
    GameObject wall;
    void Start()
    {
        speed = maxspeed;
        angle = 0;
        cw = false;
    }

    // Update is called once per frame
    void Update()
    {

        target_list = new List<GameObject>();
        hitcolliders = Physics.OverlapSphere(transform.position, detectrange);
        foreach (var hitcollider in hitcolliders)
        {
            if (hitcollider.tag == "Seeker")
            {
                target_list.Add(hitcollider.gameObject);
            }
        }

        wall_list.Clear();
        hitcolliders = Physics.OverlapSphere(transform.position, walldetectrange);
        foreach (var hitcollider in hitcolliders)
        {
            if (hitcollider.tag == "Wall") 
            {

                wall_list.Add(hitcollider.gameObject);
                 

            }
        }
        
        wall_number_list[0] = wall_number_list[1];
        wall_number_list[1] = wall_list.Count;


        if (target_list.Count == 0)
        {
            //move randomly aroud the map
        }

        else if (target_list.Count == 1)
        {
            target1 = target_list[0];
            var Heading = target1.transform.position - transform.position;
            var Distance = Heading.magnitude;
            Direction = -Heading / Distance;
            if (wall_number_list[1] == 0)
            {
                if (wall_number_list[0] != 0)
                {
                    Debug.Log("No wall");
                }
            }
            else
            {
                
                if (wall_number_list[1] - wall_number_list[0] == 2)
                {
                   //This will never happend 
                }
                else if (wall_number_list[1] - wall_number_list[0] == 1)
                {
                    
                    if (old_wall == null)
                    {
                        wall = wall_list[0];
                    }
                    else
                    {
                        foreach (GameObject wallob in wall_list)
                            if (wallob != old_wall)
                            {
                                wall = wallob;
                            }
                    }
                    
                    Vector3 cD =  transform.forward;
                    Vector3 wD = wall.transform.forward;
                    float total_angle = Mathf.Atan2(Vector3.Magnitude(Vector3.Cross(cD, wD)), Vector3.Dot(cD, wD));
                    float delta_angle;

                    if (wall_number_list[0] == 0)
                    {   
                        Debug.Log("Open space to wall" + wall);
                        if (total_angle >= Mathf.PI/2)
                        {
                            total_angle = Mathf.PI - total_angle; 
                            cw = true;
                        }

                    }
                    else
                    {
                        Debug.Log("Wall to corner "+wall);
                        Debug.Log(total_angle);
                    }

                    if (total_angle != 0)
                    {
                        float d_to_wall = walldetectrange/(Mathf.Sin(total_angle));
                        float time = d_to_wall/(speed*Time.deltaTime);
                        delta_angle = total_angle/time;

                        angle = delta_angle;
                        Direction = rotate(lastDirection,delta_angle,cw);
                    }

                    old_wall = wall;

                }
                else
                {
                    //Vector3 v1 = transform.position - target1.transform.position ;
                    Vector3 v1 = transform.forward;
                    Vector3 v2 = wall.transform.forward;
                    if (Mathf.Approximately(Vector3.Dot(v1, v2), 1.0f) || Mathf.Approximately(Vector3.Dot(v1, v2), -1.0f))
                    {
                        angle = 0;
                    }
                    Direction = rotate(lastDirection,angle,cw);
                    
                }
            }

        }
        else
        {
            target1 = target_list[0];
            target2 = target_list[1];

            var A = target1.transform.position - transform.position;
            var B = target2.transform.position - transform.position;
            var Distance1 = A.magnitude;
            var Distance2 = B.magnitude;

            float theta = Mathf.Atan2(Vector3.Magnitude(Vector3.Cross(A, B)), Vector3.Dot(A, B));


            float slope = (target1.transform.position.z - target2.transform.position.z) / (target1.transform.position.x - target2.transform.position.x);
            float intersection = target1.transform.position.z - slope * target1.transform.position.x;
            float threshold = slope * transform.position.x + intersection;

            if (threshold > transform.position.z)
            {
                if (target1.transform.position.x <= target2.transform.position.x)
                {
                    Direction = Getangle(theta, Distance2, Distance1, A);
                }
                else
                {
                    Direction = Getangle(theta, Distance1, Distance2, B);
                }
            }
            else
            {
                if (target1.transform.position.x >= target2.transform.position.x)
                {
                    Direction = Getangle(theta, Distance2, Distance1, A);
                }
                else
                {
                    Direction = Getangle(theta, Distance1, Distance2, B);
                }
            }
            
        }

        Vector3 Move = new Vector3(Direction.x * speed, 0, Direction.z * speed);
        rb.velocity = Move;

        transform.forward = Move;

        lastDirection = Direction;
    }

    Vector3 Getangle(float theta, float Distance1, float Distance2, Vector3 B)
    {
        float theta2 = (2 * Mathf.PI - theta) * Distance1 / (Distance1 + Distance2);

        float sinAngle = Mathf.Sin(theta2);
        float cosAngle = Mathf.Cos(theta2);

        float newX = B.x * cosAngle - B.z * sinAngle;
        float newZ = B.x * sinAngle + B.z * cosAngle;
        Vector3 Heading = new Vector3(newX, 0, newZ);

        return Heading / Heading.magnitude;
    }

    Vector3 rotate(Vector3 oD,float theta, bool clockwise)
    {
        float sinAngle = Mathf.Sin(theta);
        float cosAngle = Mathf.Cos(theta);
        float newX;
        float newZ;
        if (clockwise == false)
        {
            newX = oD.x * cosAngle - oD.z * sinAngle;
            newZ = oD.x * sinAngle + oD.z * cosAngle;
        }
        else
        {
            newX = oD.x * cosAngle + oD.z * sinAngle;
            newZ = -oD.x * sinAngle + oD.z * cosAngle;
        }
        Vector3 Heading = new Vector3(newX, 0, newZ);

        return Heading / Heading.magnitude;
    }
}