using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MaterialSpawner : MonoBehaviour
{
    public GameObject[] materials;

    public List<GameObject> tempList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveItem(GameObject gameObject)
    {
        tempList = materials.ToList();

        tempList.Remove(gameObject);

        Debug.Log("Made temp List!");
    }

    public void SpawnMaterial()
    {
        Debug.Log("Running spawnMats in material spawner");

        GameObject[] diff = materials.Except(tempList).ToArray();

        foreach(GameObject item in diff)
        {
            Debug.Log("Diff is " + item);
        }

        foreach (var mats in materials)
        {
            //Debug.Log("Mat in mats is " + mats);
            if (mats != null)
            {
                tempList.Remove(mats);
            }
            else if (tempList.Count == 1)
            {
                Debug.Log("Material " + mats + " is missing!");
            }
        }
       // Instantiate(material, spawnPoint);
        Debug.Log("Spawning in!");
    }
}
