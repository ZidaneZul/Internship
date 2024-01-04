using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsScript : MonoBehaviour
{
    public string typeOfMaterial;

    public Transform spawnPoint;

    public GameObject manufactoringMachine;
    GameObject floor;

    public bool isItemInManufactoringZone;

    #region Scripts

    Manager manager;
    public MachineMaterialScript machineMatScript;


    #endregion




    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
        machineMatScript = GameObject.Find("Machine2").GetComponent<MachineMaterialScript>();

        floor = GameObject.Find("Floor");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TeleportBackToBox()
    {
        Debug.Log(gameObject + "is gonna tp back to box at " + spawnPoint.position);
        transform.position = spawnPoint.position;
    }

    public void CheckIfItemShouldBeManufactored()
    {
        if (isItemInManufactoringZone)
        {
            ///spawn the prefab on point in the machine
            machineMatScript.PutMatsOnPoint(typeOfMaterial);

            ///runs the code to make the carrier run
            if (machineMatScript.didAddNewMat)
            {
                Debug.Log("Gonna run manager!");
                manager.MakeItem(typeOfMaterial);
            }

            TeleportBackToBox();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(gameObject + "Item has collided with " + other.gameObject);
        if (other.gameObject == manufactoringMachine)
        {
            Debug.Log("Item is in zone!");
            isItemInManufactoringZone = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == manufactoringMachine)
        {
            isItemInManufactoringZone = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == floor)
        {
            TeleportBackToBox();
        }
    }

}
