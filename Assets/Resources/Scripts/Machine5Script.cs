using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine5Script : MonoBehaviour
{
    public int materialRemainingInside;

    public GameObject matToInstantiate;

    public GameObject[] points;

    bool didAddMaterialsToMachine = false;

    // Start is called before the first frame update
    void Start()
    {
        points = GameObject.FindGameObjectsWithTag("Machine5MaterialPoint");

        GenerateRandomMats();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsThereMaterialsLeft()
    {
        if (materialRemainingInside > 0)
        {
            return true;
        }
        return false;
    }

    ///returns bool to check if material got added, used to instantiate mat to ponit
    public void IncreaaseMaterialCount()
    {
        if(materialRemainingInside < 7)
        {
            materialRemainingInside++;
            AddMaterialsToPoint();
        }
    }

    public void DecreaseMaterialCount()
    {
        if(materialRemainingInside> 0)
        {
            materialRemainingInside--;
        }
    }

    public void AddMaterialsToPoint()
    {
        foreach (GameObject point in points)
        {
            MaterialPointHolder holder = point.GetComponent<MaterialPointHolder>();

            if (holder.matWaiting == null)
            {
                GameObject instantiatedItem = Instantiate(matToInstantiate, point.transform);
                holder.matWaiting = instantiatedItem;
                break;
            }
        }
    }

    public void GenerateRandomMats()
    {
        int random = Random.Range(0, 7);
        for(int i = 0; i < random; i++)
        {
            AddMaterialsToPoint();
        }
    }
}
