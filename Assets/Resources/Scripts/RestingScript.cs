using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RestingScript : MonoBehaviour
{
    public Dictionary<GameObject, GameObject> restingPairs = new Dictionary<GameObject, GameObject>();
    public List<GameObject> occupiedRestPoints = new List<GameObject>();
    public List<GameObject> inactiveCarriers = new List<GameObject>();

    public GameObject[] restPoints = new GameObject[8];
    public List<GameObject> restPointsList = new List<GameObject>();

    public GameObject[] carriers = new GameObject[8];

    // Start is called before the first frame update
    void Start()
    {
        restPoints = GameObject.FindGameObjectsWithTag("RestPoint");

        carriers = GameObject.FindGameObjectsWithTag("Carrier");

        foreach(GameObject points in restPoints)
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
        for(int i = 0; i < restPoints.Length; i++)
        {
            RestPointHolder holder = restPoints[i].GetComponent<RestPointHolder>();

            if(holder.itemResting == null)
            {
                Debug.Log("returning avail slot " + i);
                return i;
            }
        }
        return 8;
    }

    public List<GameObject> GetListOfOccupiedRestPoints()
    {
        occupiedRestPoints.Clear();

        foreach(GameObject point in restPoints)
        {
            RestPointHolder restHolder = point.GetComponent<RestPointHolder>();
            if(restHolder.itemResting != null)
            {
                occupiedRestPoints.Add(point);
            }
        }
        return occupiedRestPoints;
    }

    public List<GameObject> GetListOfInactiveCarriers()
    {
        inactiveCarriers.Clear();

        foreach( GameObject carry in carriers)
        {
            ConveyorItemScript conveyorScript = carry.GetComponent<ConveyorItemScript>();

            if(conveyorScript.itemToMake_string != null)
            {
                inactiveCarriers.Add(carry);
            }
        }

        return inactiveCarriers;
    }
    public int GetLastPossibleRestSlot()
    {
        for(int i = 8; i == 0; i--)
        {
            RestPointHolder restPointHolderScipr = restPointsList[i].GetComponentInParent<RestPointHolder>();

            if (restPointHolderScipr.itemResting != null)
            {
                return i++;
            }
        }
        return 8;
    }
    public void SetRestingPoints()
        ///sets resting point for ALL the inactive carriers
    {
        int i = 1;

        restPointsList.Clear();
        restPointsList = restPoints.ToList();

        foreach (GameObject inactiveCarrier in GetListOfInactiveCarriers())
        {
            ConveyorItemScript conveyorItemScript = inactiveCarrier.GetComponent<ConveyorItemScript>();

            conveyorItemScript.currentOrderNumber = i;
            i++;

            foreach(GameObject restPoint in restPointsList)
            {
                RestPointHolder restPointHolder = restPoint.GetComponent<RestPointHolder>();

                restPointHolder.itemResting = inactiveCarrier;
                conveyorItemScript.currentRestPoint_GO = restPoint;
                restPointsList.Remove(restPoint);
                break;
            }
        }
    }

    //public void UpdateRestPoint()
    //{
    //    for(int i = 1; i < carriers.Length; i++)
    //    {

    //    }
    //}

    ///HOW TO GET CORRECT ORDER OF ITEM RESTING??
    ///just go thru the list of restpoint and find which item is resting
    ///it WILL go thru the order and able to find the carrier to go first
    ///need to set the carrier order number afterwards!
}