using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class ConveyorItemScript : MonoBehaviour
{

    #region containers
    public GameObject[] pointsToFollow;
    public GameObject[] carriers;
    public GameObject carrierInfront;

    public GameObject firstMachine, secondMachine, thirdMachine, fourthMachine, fifthMachine;
    public GameObject[] Machines = new GameObject[5];

    Dictionary<GameObject, int> machineNumbers = new Dictionary<GameObject, int>();

    int[] FlashLightOrder = { 5, 3, 4 };
    int[] USBOrder = { 5, 2, 4 };
    int[] PushButtonOrder = { 5, 3, 1 };
    int[] WifiOrder = { 5, 2, 3, 4, 1 };
    int[] LimitSwitchOrder = { 5, 1, 4, 1 };

    int[] currentOrder;
    #endregion

    Manager manager;
    public RestingScript restPointScript;
    public ConveyorItemScript carrierInfront_Script;
    
    //string to store which item is being made
    public string itemToMake_string;

    //suppose to be used to store the order of item, if first = 0, last = 7 (for production)
    public int currentOrderNumber;

    public GameObject currentRestPoint_GO;
    public int currentRestPoint_int;

    //to remember which machine it stopped at after finishing order
    public GameObject currentMachine_GO;

    public float limitDistance;
    int currentMachineNumber;
    public float speed;
    float time;
    public bool PauseHere;
    public bool isFinished = false;
    bool ResetValues;
    bool startGoing;

    public bool resting = true;

    TextMeshPro TMP;


    // Start is called before the first frame update
    void Start()
    {
        pointsToFollow = GameObject.FindGameObjectsWithTag("StopPoint");

        manager = GameObject.Find("SceneManager").GetComponent<Manager>();
        restPointScript = GameObject.Find("RestPoints").GetComponent<RestingScript>();

        TMP = GetComponentInChildren<TextMeshPro>();

        GetCarrierInfront();

        machineNumbers[firstMachine] = 1;
        machineNumbers[secondMachine] = 2;
        machineNumbers[thirdMachine] = 3;
        machineNumbers[fourthMachine] = 4;
        machineNumbers[fifthMachine] = 5;

        Machines[0] = firstMachine;
        Machines[1] = secondMachine;
        Machines[2] = thirdMachine;
        Machines[3] = fourthMachine;
        Machines[4] = fifthMachine;
    }

    // Update is called once per frame
    void Update()
    { 
        if (Input.GetKeyUp(KeyCode.A))
        {
            PauseHere = true;
            Debug.Log("PUASE HERE PLS");
        }
    }
    public void MakeItem()
    {
        switch (itemToMake_string)
        {
            case "Flashlight":
                currentOrder = FlashLightOrder;
                break;
            case "USB":
                currentOrder = USBOrder;
                break;
            case "PushButton":
                currentOrder = PushButtonOrder;
                break;
            case "Limit":
                currentOrder = LimitSwitchOrder;
                break;
            case "Wifi":
                currentOrder = WifiOrder;
                break;
        }

        StartCoroutine(MakeItem(currentOrder));
    }

    private IEnumerator MakeItem(int[] order)
    {
        resting = false;

        Debug.Log("check to see how much it runs EAEWAEA");
        int i = 0;
        RemoveRestBayValue();
        currentMachineNumber = order[i];

        TextChange("M");

        foreach (int int1 in order)
        {
            foreach (GameObject point in pointsToFollow)
            {
                //while loop runs if the plate is far from the point to follow
                while (Vector3.Distance(point.transform.position, transform.position) > 0.05f)
                {
                    ///moves the plate using the transform position
                    transform.position = Vector3.MoveTowards(transform.position, point.transform.position, speed * Time.deltaTime);

                    currentMachine_GO = point;

                    while (IsCloseToFrontItem())
                    {
                        yield return null;
                    }

                    // Debug.Log("Going to point " + point + "\n current machine " + Machines[currentMachineNumber - 1]);

                    ///if the plate is close enough, runs code to continue to the next machine order or reset to bay.
                    /// currentMachine - 1 is cuz array starts from 0 while naming the machines starts from 1
                    if (Vector3.Distance(transform.position, Machines[currentMachineNumber - 1].transform.position) <= 0.1)
                    {
                        i++;

                        
                        //Debug.Log("conveyor item value i is " + i + "\n" +
                         //   "order count is " + order.Count());

                        if (i < order.Count())
                        {
                            currentMachineNumber = order[i];
                            yield return new WaitForSeconds(1f);

                        }
                        else
                        {
                         //   Debug.Log("nothing to make!");

                            StartCoroutine(ResetToBay());

                            yield break;
                        }
                    }
                    yield return null;
                }
               // Debug.Log("In foreach out of everything");
           }
        }
    }
    private IEnumerator ResetToBay()
    {
        itemToMake_string = null;

        TextChange("R");

        startGoing = false;
        currentRestPoint_int = restPointScript.GetLastPossibleRestSlot() + 1;
        Debug.Log("Curresntly gonna rest in RestPoint" + currentRestPoint_int);
        currentRestPoint_GO = GameObject.Find("RestPoint" + currentRestPoint_int);

        RestPointHolder restPointHolder = currentRestPoint_GO.GetComponent<RestPointHolder>();

        if (restPointHolder.itemResting != gameObject)
        {
            restPointHolder.itemResting = gameObject;
        }

        for (int i = 0; i < 3; i++)
        {

            foreach (GameObject point in pointsToFollow)
            //to cycle thru the points and start to move after reaching the correct machine
            {
                if (point == currentMachine_GO)
                {
                    startGoing = true;
                }

                // while (Vector3.Distance(point.transform.position, transform.position) > 0.05f && startGoing)
                while (point.transform.position != transform.position && startGoing)
                {
                    //Debug.Log("Distance is " + Vector3.Distance(currentRestPoint_GO.transform.position, transform.position)
                    //    + " to " + currentRestPoint_GO);

                    //stops the carrier if its close to the item infront
                    while (IsCloseToFrontItem())
                    {
                        yield return null;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, point.transform.position, speed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) < 0.05f)
                    //if (transform.position == currentRestPoint_GO.transform.position)
                    {
                        Debuging("ResetToBay complete");
                        resting = true;
                        TextChange("");
                        yield break;
                    }

                    //function to move the carriers
                    yield return null;
                }

            }
        }
    }


    public IEnumerator MoveUpRestBay()
    {
        TextChange("G");
        //while(Vector3.Distance(currentRestPoint_GO.transform.position, transform.position) > 0.05f)
        Debug.Log("Move up restBay!");
        while(currentRestPoint_GO.transform.position != transform.position)
        {
            //Debug.Log("going there!");
            while (IsCloseToFrontItem())
            {
                yield return null;
            }
            transform.position = Vector3.MoveTowards(transform.position, currentRestPoint_GO.transform.position, speed * Time.deltaTime);
            yield return null;
        }

        if (Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) < 0.2f)
        {
            resting = true;
            TextChange("");
            Debuging(gameObject + " MoveUpRestBay Complete \n Distance: " + Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) + currentRestPoint_GO);
            yield break;
        }

    }
    private IEnumerator Circling()
    {
        TextChange("C");
        int count  = 0;

        foreach (GameObject point in pointsToFollow)
        {
            // Debug.Log("goin thru points");
            while (Vector3.Distance(point.transform.position, transform.position) > 0.05f)
            {

                //stops the carrier if its close to the item infront
                while (IsCloseToFrontItem())
                {
                    yield return null;
                }

                //function to move the carriers
                transform.position = Vector3.MoveTowards(transform.position, point.transform.position, speed * Time.deltaTime);
                yield return null;
            }
            count++;

        }

        yield break;
    }

    public bool StartTimer(float seconds)
    {
        if (ResetValues)
        {
           time = 0f;
            ResetValues = false;
        }

        time += Time.deltaTime;
        Debug.Log(time);

        if (seconds <= time)
        {   
            ResetValues = true;
            PauseHere = false;
            return true;
        }
        else return false;
    }

    public void GetCarrierInfront()
    {
        carriers = GameObject.FindGameObjectsWithTag("Carrier");
        List<GameObject> carrierList = carriers.ToList();


        float distance = 9999f;
        foreach (GameObject carry in carrierList)
        {
            if (carry == gameObject)
            {
                carrierList.Remove(carry);
                break;
            }
            if (Vector3.Distance(transform.position, carry.transform.position) <= distance && transform.position.x < carry.transform.position.x)
            {
                distance = Vector3.Distance(transform.position, carry.transform.position);
                carrierInfront = carry;
            }

        }
        if (carrierInfront == null)
        {
            float farDistance = 0f;
            foreach(GameObject carry in carrierList)
            {
                if(Vector3.Distance(carry.transform.position, transform.position) >= farDistance)
                {
                    farDistance = Vector3.Distance(transform.position, carry.transform.position);
                    carrierInfront = carry;
                    carrierInfront_Script = carry.GetComponent<ConveyorItemScript>();
                }
            }
        }
    }

    public bool IsCloseToFrontItem()
    {
        Debug.Log(Vector3.Distance(transform.position, carrierInfront.transform.position) + gameObject.name);
        if(Vector3.Distance(transform.position, carrierInfront.transform.position) <= limitDistance)
        {
            if (carrierInfront_Script.resting)
            {


            }
          //  Debug.LogWarning("Pauseeeee");
            return true;
        }
        return false;
    }

    public void RemoveRestBayValue()
    {
        RestPointHolder restPointHolder = currentRestPoint_GO.GetComponent<RestPointHolder>();

        restPointHolder.itemResting = null;
        currentRestPoint_GO = null;
        currentRestPoint_int = 99;
    }

    public void Debuging(string str)
    {
        Debug.Log(str);
    }

    public void TextChange(string str)
    {
        TMP.text = str;
    }
}
