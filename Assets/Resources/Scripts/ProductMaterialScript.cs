using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductMaterialScript : MonoBehaviour
{
    public Transform pointInMachine;

    Rigidbody rb;

    public GameObject storingBox, point, floor;

    public MaterialPointHolder pointScript;
    bool isInZone, keptInBox;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        storingBox = GameObject.Find("StoringBox");

        point = storingBox.transform.GetChild(2).gameObject;
        floor = GameObject.Find("Floor");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckIfInBox()
    {
        if (isInZone)
        {
            //Debug.Log("the shit is in the zone");
            pointScript.matWaiting = null;
            transform.parent = storingBox.transform;
            keptInBox = true;
        }
    }
    public void DisableGrav()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Product collided with trigger " + other.gameObject);
        if(other.gameObject == storingBox)
        {
            isInZone = true;
        }

        if(other.gameObject == floor)
        {
            Debug.Log("The flor the floor th flooeoooeoeoeoe");
            if (keptInBox)
            {
                transform.position = point.transform.position;
            }
            else
            {
                transform.position = pointScript.gameObject.transform.position;
                rb.velocity = Vector3.zero;
                transform.rotation = new Quaternion(0, 0, 0, 0);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == storingBox)
        {
            isInZone=false;
        }
    }
}
