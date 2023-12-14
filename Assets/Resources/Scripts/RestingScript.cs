using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RestingScript : MonoBehaviour
{
    public List<GameObject> occupiedRestPoints = new List<GameObject>();
    public List<GameObject> inactiveCarriers = new List<GameObject>();

    public GameObject[] restPoints = new GameObject[8];
    public List<GameObject> restPointsList = new List<GameObject>();

    public GameObject[] carriers = new GameObject[8];

    public string testing;

    // Start is called before the first frame update
    void Start()
    {
        restPoints = GameObject.FindGameObjectsWithTag("RestPoint");

        carriers = GameObject.FindGameObjectsWithTag("Carrier");

        for(int i = 8; i >=0; i--)
        {
            testing += i.ToString();
        }
        Debug.Log(testing.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("This thing is running");
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

        string debugLogTesting = "";

        //foreach( GameObject carry in carriers)
        //{
        //    ConveyorItemScript conveyorScript = carry.GetComponent<ConveyorItemScript>();

        //    if (conveyorScript.itemToMake_string == null || conveyorScript.itemToMake_string == "")
        //    {
        //        inactiveCarriers.Add(carry);
        //        debugLogTesting += carry.ToString();
        //    }
        //}

        foreach(GameObject restPoints in restPoints)
        {
            RestPointHolder restpointHOlder = restPoints.GetComponent<RestPointHolder>();

            //Debug.Log(restpointHOlder);

            if(restpointHOlder.itemResting != null)
            {
                inactiveCarriers.Add(restpointHOlder.itemResting);
                debugLogTesting += restpointHOlder.itemResting.ToString() + "\n";
            }
        }

        //Debug.Log(debugLogTesting);
        
        return inactiveCarriers;
    }
    public int GetLastPossibleRestSlot()
    {

    restart:
        Debug.Log("restarting");
        for(int i = 7; i > 0; i--)
        {
            //Debug.Log("for looop " + i);
            RestPointHolder restPointHolderScipr = restPoints[i].GetComponent<RestPointHolder>();

            if (restPointHolderScipr.itemResting != null)
            {
               // Debug.Log("Rest Bay number" + i + "has " + restPointHolderScipr.itemResting);

                if(i == 7)
                {
                    Debug.Log("triggered"); 
                  //  RemoveRestBayItems();
                    SetRestingPoints();
                    MoveInactiveCarriersToNewRest();
                    goto restart;
                }
                int empty = i;
                return i;
            }
        }


        return 0;
    }

    public void RemoveRestBayItems()
    {
        foreach(GameObject restPoint in restPoints)
        {
            RestPointHolder restPointHolder = restPoint.GetComponent<RestPointHolder>();
            restPointHolder.itemResting = null;
        }
    }
    public void SetRestingPoints()
        ///sets resting point for ALL the inactive carriers
    {
        int i = 0;

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
        foreach(GameObject restPoint in restPointsList)
        {
            RestPointHolder restPointHolder = restPoint.GetComponent<RestPointHolder>();

            restPointHolder.itemResting = null;
        }
    }

    public void MoveInactiveCarriersToNewRest()
    {
        foreach(GameObject carrier in carriers)
        {
            ConveyorItemScript itemScript = carrier.GetComponent<ConveyorItemScript>();

            //Debug.Log(itemScript.gameObject + "is resting " + itemScript.resting);

            if ((itemScript.itemToMake_string == "" || itemScript.itemToMake_string == null) && itemScript.resting == true)
            {
             //   Debug.Log("Moving " + carrier.name + " to " + itemScript.currentRestPoint_GO);
                   StartCoroutine(itemScript.MoveUpRestBay());
            }
            else
            {
               // Debug.Log(itemScript.itemToMake_string);
            }
        }
    }
    ///HOW TO GET CORRECT ORDER OF ITEM RESTING??
    ///just go thru the list of restpoint and find which item is resting
    ///it WILL go thru the order and able to find the carrier to go first
    ///need to set the carrier order number afterwards!
    ///
    ///setting new restpoints doesnt remove the saved items in restpoint 7!
    ///meaning if plate 0 was in respoint 7 and goes to 6, 7 would still store plate0 
    ///as resting there 
}