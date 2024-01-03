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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckIfInBox()
    {
        if (isInZone)
        {
            Debug.Log("the shit is in the zone");
            rb.useGravity = true;
            pointScript.matWaiting = null;
            transform.parent = storingBox.transform;
            keptInBox = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == storingBox)
        {
            isInZone = true;
        }

        if(other.gameObject == floor)
        {
            if (keptInBox)
            {
                transform.position = point.transform.position;
            }
            else
            {
                transform.position = pointScript.gameObject.transform.position;
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
