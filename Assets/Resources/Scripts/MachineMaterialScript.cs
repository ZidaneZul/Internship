using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineMaterialScript : MonoBehaviour
{
    BoxCollider boxColl;

    Manager manager;

    // Start is called before the first frame update
    void Start()
    {
        boxColl = GetComponent<BoxCollider>();

        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("Testing testing test test");
    }

    void CheckForMats()
    {
    
    }
    
}
