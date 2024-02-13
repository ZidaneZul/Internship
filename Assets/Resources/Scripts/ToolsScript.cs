using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsScript : MonoBehaviour
{
    Rigidbody rb;
    public GameObject floor;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGrab()
    {
        rb.useGravity = false;
    }

    public void OnRelease()
    {
        rb.useGravity = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == floor)
        {

        }
    }
}
