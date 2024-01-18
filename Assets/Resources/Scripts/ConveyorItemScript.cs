using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;

public class ConveyorItemScript : MonoBehaviour
{

    #region Containers for Machines, Carriers and Orders
    [HideInInspector]
    public GameObject[] pointsToFollow;
    [HideInInspector]
    public GameObject[] carriers;
    [HideInInspector]
    public GameObject carrierInfront;

    public GameObject firstMachine_Point, secondMachine_Point, thirdMachine_Point, fourthMachine_Point, fifthMachine_Point;
    public GameObject firstMachine_GO, secondMachine_GO, thirdMachine_GO, fourthMachine_GO, fifthMachine_GO;
    [HideInInspector]
    public GameObject[] Machines_Points = new GameObject[5];
    [HideInInspector]
    public GameObject[] Machines_GOs = new GameObject[5];

    public GameObject currentRestPoint_GO;
    public int currentRestPoint_int;

    //to remember which machine it stopped at after finishing order
    public GameObject currentMachine_GO;

    public GameObject machineToRunAnimation;

    public GameObject material;
    public GameObject itemLocation;


    int[] FlashLightOrder = { 2, 3, 2 };
    int[] USBOrder = { 2, 3, 5, 2 };
    int[] PushButtonOrder = { 2, 3, 5, 2 };
    int[] WifiOrder = { 2, 3, 4, 2 };
    int[] LimitSwitchOrder = { 2, 1, 4, 5, 2 };

    int[] currentOrder;

    int currentMachineNumber;

    //suppose to be used to store the order of item, if first = 0, last = 7 (for production)
    public int currentOrderNumber;

    TextMeshPro TMP;


    
    #endregion

    #region Scripts

    Manager manager;
    public RestingScript restPointScript;
    public ConveyorItemScript carrierInfront_Script;

    public MachineScript machineScript;

    public AudioSource audioSource;
    #endregion

    #region Other Variables

    //string to store which item is being made
    public string itemToMake_string;

    public float limitDistance;
    public float speed;
    public float currentSpeed;
    float time;
    public bool PauseHere;
    public bool isFinished = false;
    bool ResetValues;
    bool startGoing;

    public bool resting = true;
    public bool pastRestPoint = false;

    public bool pastStartingMachine = false;

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        //pointsToFollow = GameObject.FindGameObjectsWithTag("StopPoint");

        manager = GameObject.Find("SceneManager").GetComponent<Manager>();
        pointsToFollow = manager.points;

        restPointScript = GameObject.Find("RestPoints").GetComponent<RestingScript>();

        carriers = manager.carriers;

        audioSource = GetComponent<AudioSource>();

        TMP = GetComponentInChildren<TextMeshPro>();

        itemLocation = transform.GetChild(1).gameObject;

        GetCarrierInfront();

        Machines_Points[0] = firstMachine_Point;
        Machines_Points[1] = secondMachine_Point;
        Machines_Points[2] = thirdMachine_Point;
        Machines_Points[3] = fourthMachine_Point;
        Machines_Points[4] = fifthMachine_Point;

        Machines_GOs[0] = firstMachine_GO;
        Machines_GOs[1] = secondMachine_GO;
        Machines_GOs[2] = thirdMachine_GO;
        Machines_GOs[3] = fourthMachine_GO;
        Machines_GOs[4] = fifthMachine_GO;
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(audioSource.clip);
            audioSource.volume = 0.1f;
            audioSource.Pause();
        }
    }

    public void ChangeSpeed(float multiplier)
    {
        currentSpeed = speed * multiplier;
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

    #region Coroutines to move item Holders
    private IEnumerator MakeItem(int[] order)
    {
        resting = false;

        Debug.Log("check to see how much it runs EAEWAEA");
        int i = 0;
        RemoveRestBayValue();
        currentMachineNumber = order[i];
        Debug.Log("before running foreach loop, i= " + currentMachineNumber);

        TextChange("M");

        ///this foreach loop is just so that the carrier would not be stuck after looping
        ///pointsToFollow once
        foreach (int int1 in order)
        {
            ///goes thru points in the machine for the carrier to follow
            foreach (GameObject point in pointsToFollow)
            {
                ///while loop runs if the plate is far from the point to follow
                while (Vector3.Distance(point.transform.position, transform.position) > 0.05f)
                {
                    ///moves the plate using the transform position
                    transform.position = Vector3.MoveTowards(transform.position, point.transform.position, currentSpeed * Time.deltaTime);

                    Debug.Log("Playing audio");
                    audioSource.Play();

                    ///<summary>
                    ///to remember which machine the carrier would be in after finishing producing the 
                    ///item. Makes it so running ResetToBay the carrier would follow the correect points
                    ///instead of floating to the starting point and looping arond from there.
                    ///</summary>
                    currentMachine_GO = point;

                    ///To make the carrier stop when its too close to the carrier infront
                    while (IsCloseToFrontItem())
                    {
                        audioSource.Pause();
                        yield return null;
                    }

                    // Debug.Log("Going to point " + point + "\n current machine " + Machines[currentMachineNumber - 1]);

                    ///if the plate is close enough, runs code to continue to the next machine order or reset to bay.
                    /// currentMachine - 1 is cuz array starts from 0 while naming the machines starts from 1
                    if (Vector3.Distance(transform.position, Machines_Points[currentMachineNumber - 1].transform.position) <= 0.1)
                    {
                        audioSource.Pause();

                        machineToRunAnimation = Machines_GOs[currentMachineNumber - 1];
                        machineScript = machineToRunAnimation.GetComponent<MachineScript>();

                       
                        if(material == null)
                        {
                        machineScript.SetMaterialToCarrier(this);

                        }
                        machineScript.GetCarrierProperties(material, itemLocation);

                        /// <summary>
                        ///i is used to get the value in the array of machine order. Helps to check if the next cycle of this 
                        ///code
                        ///would be needed to be ran again. Checks if there is a next machine or not
                        ///If i++ would be bigger than the array of machine order, make the carrier run ResetToBay(), 
                        ///else continue with the 
                        ///next machine.
                        ///</summary>
                        i++;

                        StartCoroutine(machineScript.RunMachineSequence(currentMachineNumber, gameObject, this));

                        yield return new WaitUntil(() => machineScript.AllowCarrierToMoveOn());

                        //if its the second last machine
                        if (i == order.Count() - 1)
                        {
                            manager.SwitchToFinalProduct(itemToMake_string, material, itemLocation.transform, this);
                        }

                        if (i < order.Count())
                        {

                            currentMachineNumber = order[i];
                            //Debug.Log("Current machine number is " + currentMachineNumber);
                        }
                        ///This else statement will run after finishing all the machine order
                        else
                        {
                            StartCoroutine(ResetToBay());
                            yield break;
                        }
                    }
                    yield return null;
                }
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
            ///to cycle thru the points and start to move after reaching the correct machine
            {
                if (point == currentMachine_GO)
                {
                    startGoing = true;
                }

                ///while (Vector3.Distance(point.transform.position, transform.position) > 0.05f && startGoing)
                while (point.transform.position != transform.position && startGoing)
                {
                    audioSource.Play();

                    ///stops the carrier if its close to the item infront
                    while (IsCloseToFrontItem())
                    {
                        // Debug.Log(gameObject.name + " is close to item infront!");
                        audioSource.Pause();
                        yield return null;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, point.transform.position, currentSpeed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) < 0.1f)
                    //if (transform.position == currentRestPoint_GO.transform.position)
                    {
                        //Debug.Log("ResetToBay complete");
                        resting = true;
                        pastStartingMachine = false;
                        TextChange("");

                        audioSource.Pause();
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

            audioSource.Play();

            while (IsCloseToFrontItem())
            {
                audioSource.Pause();
                yield return null;
            }
            transform.position = Vector3.MoveTowards(transform.position, currentRestPoint_GO.transform.position, currentSpeed * Time.deltaTime);
            yield return null;
        }

        if (Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) < 0.2f)
        {
            resting = true;
            TextChange("");
            audioSource.Pause();
           // Debuging(gameObject + " MoveUpRestBay Complete \n Distance: " + Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) + currentRestPoint_GO);
            yield break;
        }

    }
    public IEnumerator Circling()
    {
        TextChange("C");

        pastRestPoint = false; 

        resting = false;
        for (int i = 0; i < 3; i++)
        {
            foreach (GameObject point in pointsToFollow)
            {

                // Debug.Log("goin thru points");

                while (Vector3.Distance(point.transform.position, transform.position) > 0.05f)
                {

                    audioSource.Play();
                    ///<summary>
                    ///to make make sure it goes past its own rest point before changing the bool
                    ///to true. Bool is there to make sure it stops at the resting point after 
                    ///circle, else there would be infinite circling.
                    ///</summary>
                    if (Vector3.Distance(point.transform.position, transform.position) < 0.5f)
                    {
                        pastRestPoint = true;
                    }

                    if (pastRestPoint && Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) < 0.1f)
                    {
                        TextChange("");
                        audioSource.Pause();
                        yield break;
                    }

                    ///stops the carrier if its close to the item infront
                    while (IsCloseToFrontItem())
                    {
                        audioSource.Pause();
                        yield return null;
                    }

                    Vector3 currrentPosition = transform.position;

                    ///function to move the carriers
                    transform.position = Vector3.MoveTowards(transform.position, point.transform.position, currentSpeed * Time.deltaTime);

                    //if (currrentPosition == transform.position && Timer(5f))
                    //{
                    //    transform.position = currentRestPoint_GO.transform.position;
                    //    TextChange("");
                    //    audioSource.Pause();
                    //    yield break;
                    //}
                    yield return null;
                }
            }
            resting = true;
            audioSource.Pause();
            TextChange("");
        }
    }

    #endregion

   

    public void GetCarrierInfront()
    {
        carriers = manager.carriers;
        List<GameObject> carrierList = carriers.ToList();


        float distance = 9999f;
        foreach (GameObject carry in carrierList)
        {
            if (carry == gameObject)
            {
                carrierList.Remove(carry);
                break;
            }
            if ((Vector3.Distance(transform.position, carry.transform.position) <= distance) && transform.position.x < carry.transform.position.x)
            {
                distance = Vector3.Distance(transform.position, carry.transform.position);
                carrierInfront = carry;
                carrierInfront_Script = carry.GetComponent<ConveyorItemScript>();
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
        //Debug.Log(Vector3.Distance(transform.position, carrierInfront.transform.position) + gameObject.name);
        if(Vector3.Distance(transform.position, carrierInfront.transform.position) <= limitDistance * manager.zoomScale[manager.zoomIndex])
        {
            if (carrierInfront_Script.resting == true && resting == false)
            {
                //Debug.Log(gameObject + " triggered the other carrier to move");
                manager.CircleInactiveItems();

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
    public bool Timer(float seconds)
    {
        time += Time.deltaTime;
        while (time < seconds)
        {
            return false;
        }
        time = 0;
        return true;
    }
}
