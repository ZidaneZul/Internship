using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    RestingScript restPointScript;

    //string to store which item is being made
    public string itemToMake_string;

    //suppose to be used to store the order of item, if first = 0, last = 7 (for production)
    public int currentOrderNumber;

    public GameObject currentRestPoint_GO;
    public int currentRestPoint_int;

    //to remember which machine it stopped at after finishing order
    GameObject currentMachine_GO;

    public float limitDistance;
    int currentMachineNumber;
    public float speed;
    float time;
    public bool PauseHere;
    public bool isFinished = false;
    bool ResetValues;
    bool startGoing;


    // Start is called before the first frame update
    void Start()
    {
        pointsToFollow = GameObject.FindGameObjectsWithTag("StopPoint");

        manager = GameObject.Find("SceneManager").GetComponent<Manager>();
        restPointScript = GameObject.Find("RestPoints").GetComponent<RestingScript>();

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
        if (isFinished)
        {
            StartCoroutine(Circling());
        }
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
        int i = 0;
        RemoveRestBayValue();
        currentMachineNumber = order[i];

        foreach (GameObject point in pointsToFollow)
        { 
            //while loop runs if the plate is far from the point to follow
            while (Vector3.Distance(point.transform.position, transform.position) > 0.05f)
            {

                //moves the plate using the transform position
                transform.position = Vector3.MoveTowards(transform.position, point.transform.position, speed * Time.deltaTime);

                //if the plate is close enough, runs code to continue to the next machine order or reset to bay.
                if (Vector3.Distance(transform.position, Machines[currentMachineNumber - 1].transform.position) <= 0.1)
                {
                    i++;

                    if (i < order.Count())
                    {
                        currentMachineNumber = order[i];
                        currentMachine_GO = point;
                        yield return new WaitForSeconds(1f);

                    }
                    else
                    {
                        Debug.Log("nothing to make!");

                        StartCoroutine(ResetToBay());

                        yield break;
                    }
                }
                yield return null;
            }
        }

    }
    private IEnumerator ResetToBay()
    {
        itemToMake_string = null;

        currentRestPoint_int = restPointScript.GetLastPossibleRestSlot();
        currentRestPoint_GO = GameObject.Find("RestPoint" + currentRestPoint_int);
        
        RestPointHolder restPointHolder = currentRestPoint_GO.GetComponent<RestPointHolder>();

        if(restPointHolder.itemResting != gameObject)
        {
            restPointHolder.itemResting = gameObject;
        }
        
        foreach(GameObject point in pointsToFollow)
            //to cycle thru the points and start to move after reaching the correct machine
        {
            if(point == currentMachine_GO)
            {
                startGoing = true;
            }

            while (Vector3.Distance(point.transform.position, transform.position) > 0.05f && startGoing)
            {
                //stops the carrier if its close to the item infront
                while (IsCloseToFrontItem())
                {
                    yield return null;
                }

                if(Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) < 0.05f)
                {
                    yield break;
                }

                //function to move the carriers
                transform.position = Vector3.MoveTowards(transform.position, currentRestPoint_GO.transform.position, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
 
    public IEnumerator MoveUpRestBay()
    {
        while(Vector3.Distance(currentRestPoint_GO.transform.position, transform.position) > 0.05f)
        {
            while (IsCloseToFrontItem())
            {
                yield return null;
            }
            if(Vector3.Distance(transform.position, currentRestPoint_GO.transform.position) < 0.2f)
            {
                yield break;
            }
            transform.position = Vector3.MoveTowards(transform.position, currentRestPoint_GO.transform.position, speed * Time.deltaTime);
            yield return null;
        }

    }
    private IEnumerator Circling()
    {
        Debug.Log("Running IEnumerator");
        int count  = 0;
        isFinished = false;

        foreach (GameObject point in pointsToFollow)
        {
            // Debug.Log("goin thru points");
            while (Vector3.Distance(point.transform.position, transform.position) > 0.05f)
            {
                //just a puase
                while (PauseHere)
                {
                    StartTimer(2f);
                    yield return null;
                }

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

        //to rerun the coroutine after going thru all the points.
        //prevents the spamming and running mulitple coroutine.
        if(count >= pointsToFollow.Length)
        {
            isFinished = true;
        }
        
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
                }
            }
        }
    }

    public bool IsCloseToFrontItem()
    {
        //Debug.Log(Vector3.Distance(transform.position, carrierInfront.transform.position) + gameObject.name);
        if(Vector3.Distance(transform.position, carrierInfront.transform.position) <= limitDistance)
        {
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
}
