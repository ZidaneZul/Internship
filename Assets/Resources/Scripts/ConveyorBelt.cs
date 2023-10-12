using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public List<GameObject> items;
    public float speed = 10f;
    public Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        Quaternion rotation = transform.rotation;
        direction = rotation * Vector3.right;
    }

    // Update is called once per frame
    void Update()
    {
        //foreach(GameObject item in items)
        //{
        //    Debug.Log("touching shits");   
        //    item.GetComponent<Rigidbody>().velocity = direction * speed * Time.deltaTime;
        //}
    }

    //public void OnCollisionEnter(Collision collision)
    //{
    //    items.Add(collision.gameObject);
    //}
    //public void OnCollisionExit(Collision collision)
    //{
    //    items.Remove(collision.gameObject);
    //}

    public void OnTriggerEnter(Collider other)
    {
        items.Add(other.gameObject);
    }

    public void OnTriggerExit(Collider other)
    {
        items.Remove(other.gameObject);
        Rigidbody rb = other.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void OnCollisionEnter(Collision collision)
    {
        items.Add(collision.gameObject);
    }
    public void OnCollisionExit(Collision collision)
    {
        items.Remove(collision.gameObject);
    }

    public void OnTriggerStay(Collider other)
    {
        //other.gameObject.GetComponent<Rigidbody>().velocity = transform.right *speed * Time.timeScale;
    }
}
