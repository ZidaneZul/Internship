using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConveyorItemScript : MonoBehaviour
{

    public GameObject[] pointsToFollow;
    public GameObject[] carriers;
    public GameObject carrierInfront;

    public GameObject firstMachine, secondMachine, thirdMachine, fourthMachine, fifthMachine;
    public GameObject[] Machines = new GameObject[5];

    Dictionary<GameObject, int> machineNumbers = new Dictionary<GameObject, int>();

    int currentMachineNumber;

    public int[] FlashLightOrder = { 5, 3, 4 };
    public int[] USBOrder = { 5, 2, 4 };
    public int[] PushButtonOrder = { 5, 3, 1 };
    public int[] WifiOrder = { 5, 2, 3, 4, 1 };
    public int[] LimitSwitchOrder = { 5, 1, 4, 1 };

    public float speed;
    float time;

    public float limitDistance;

    Rigidbody rb;
    Manager manager;
    public string ItemToMake;

    public bool PauseHere;
    public bool isFinished = false;
    bool ResetValues;
    // Start is called before the first frame update
    void Start()
    {
        pointsToFollow = GameObject.FindGameObjectsWithTag("StopPoint");
        rb = GetComponent<Rigidbody>();

        manager = GameObject.Find("SceneManager").GetComponent<Manager>();

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


        //StartCoroutine(Circling());
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
    public void GetItemToMake()
    {
        if (manager.ListOfItemsToMake != null)
        {
            ItemToMake = manager.ListOfItemsToMake[0];
            manager.ListOfItemsToMake.Remove(ItemToMake);

            if (ItemToMake == "Flashlight")
            {
                StartCoroutine(MakeFlashLight());
            }
        }
    }

    private IEnumerator MakeFlashLight()
    {
        int i = 0;
        currentMachineNumber = FlashLightOrder[i];

        Debug.Log("order debug");
        foreach (GameObject point in pointsToFollow)
        {
            Debug.Log("point debug");
           // Debug.Log(Vector3.Distance(point.transform.position, transform.position) > 0.05f);
            while (Vector3.Distance(point.transform.position, transform.position) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, point.transform.position, speed * Time.deltaTime);

              //  Debug.Log(Vector3.Distance(transform.position, Machines[i].transform.position));
                if (Vector3.Distance(transform.position, Machines[currentMachineNumber - 1].transform.position) <= 0.1)
                {
                    Debug.Log("we reached");
                    currentMachineNumber = FlashLightOrder[i++];
                    yield return new WaitForSeconds(1f);
                }
                yield return null;
            }
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
}
