using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsScript : MonoBehaviour
{
    Rigidbody rb;
    public GameObject floor, pointToKeep;

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
    }

    public void OnRelease()
    {
        transform.position = pointToKeep.transform.position;
        transform.rotation = pointToKeep.transform.rotation;
    }

   
}
