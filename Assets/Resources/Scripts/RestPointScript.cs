using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestPointScript : MonoBehaviour
{

    public GameObject itemResting;

    public Dictionary<GameObject, GameObject> restingPairs = new Dictionary<GameObject, GameObject>();
    public List<GameObject> itemsRestingList = new List<GameObject>();

    public GameObject[] restPoint = new GameObject[8];

    // Start is called before the first frame update
    void Start()
    {
        restPoint = GameObject.FindGameObjectsWithTag("RestPoint");

        foreach(GameObject points in restPoint)
        {
            restingPairs.Add(points, null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetAvailableSlotInt()
    { 
        for(int i = 0; i < restPoint.Length; i++)
        {
            RestPointHolder holder = restPoint[i].GetComponent<RestPointHolder>();

            if(holder.itemResting == null)
            {
                Debug.Log("returning avail slot " + i);
                return i;
            }
        }
        return 8;
    }

    public List<GameObject> GetListOfResting()
    {
        itemsRestingList.Clear();

        foreach(GameObject point in restPoint)
        {
            if(point.transform.GetChild(0) != null)
            {
                itemsRestingList.Add(point);
            }
        }
        return itemsRestingList;
    }
    public void SetRestingPoints()
    {
        foreach(GameObject itemResting in GetListOfResting())
        {
            
        }
        foreach(GameObject point in restPoint)
        {
            RestPointScript restScript = point.GetComponent<RestPointScript>();

        }
    }
}