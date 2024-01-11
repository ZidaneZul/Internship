using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine5MaterialScript : MonoBehaviour
{
    public Transform spawnPoint;

    public GameObject machine5, floor;

    Machine5Script machine5Script;

    public bool IsInsideMachineZone;

    // Start is called before the first frame update
    void Start()
    {
        machine5 = GameObject.Find("Machine5");
        machine5Script = machine5.GetComponent<Machine5Script>();
        floor = GameObject.Find("Floor");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == machine5)
        {
            IsInsideMachineZone = true;
        }

        if(other.gameObject == floor)
        {
            transform.position = spawnPoint.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == machine5)
        {
            IsInsideMachineZone = false;
        }
    }

    public void CheckIfInZone()
    {
        if (IsInsideMachineZone)
        {
            machine5Script.IncreaaseMaterialCount();
            transform.position = spawnPoint.position;
            IsInsideMachineZone = false;
        }
    }
}
