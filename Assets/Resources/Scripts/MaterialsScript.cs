using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsScript : MonoBehaviour
{
    public string typeOfMaterial;

    public Transform spawnPoint;

    MaterialSpawner matSpawner;
    Manager manager;

    public GameObject manufactoringMachine;

    bool isItemInManufactoringZone;

    // Start is called before the first frame update
    void Start()
    {
        matSpawner = GameObject.FindGameObjectWithTag("Manager").GetComponent<MaterialSpawner>();
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void DeleteThis()
    {
        matSpawner.RemoveItem(gameObject);
        matSpawner.SpawnMaterial();
        Destroy(gameObject);
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
            manager.MakeItem(typeOfMaterial);
        }
        else
        {
            TeleportBackToBox(); 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == manufactoringMachine)
        {
            isItemInManufactoringZone = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == manufactoringMachine)
        {
            isItemInManufactoringZone = false;
        }
    }
}
