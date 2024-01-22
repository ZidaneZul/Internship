using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject[] points;
    public GameObject[] restPoints;
    public GameObject[] carriers;

    public GameObject finalFlashLight, finalUSB, finalPushButton, finalWifi, finalLimitSwitch;

    public GameObject[] carriersInOrder = new GameObject[8];

    public int Wifi, LimitButton, FlashLight, USB, PushButton;

    public TextMeshProUGUI Amt;

    public List<string> ListOfItemsToMake = new List<string>();

    MachineMaterialScript machineMatsScript;

    public GameObject entireMachine;
    public GameObject player;

    public float[] zoomScale = {1, 0.5f, .25f};
    public float[] zoomX = { 0, 0, 0 };
    public float[] zoomY = { 1.29f, 1.29f, 1.62f };
    public float[] zoomZ = { 0, 0, 0 };
    public float[] zoomPos = { 14.5f, 2.5f, 1.5f };
    public int zoomIndex = 0;


    public GameObject debuggingTextPrefab;
    public GameObject worldSpaceCanvas;
    public Vector3 offset = new Vector3(0, 0.1f,0);


    private void Awake()
    {
        ///
        points = SortArrays(GameObject.FindGameObjectsWithTag("StopPoint"));
        carriers = SortArrays(GameObject.FindGameObjectsWithTag("Carrier"));
        restPoints = SortArrays(GameObject.FindGameObjectsWithTag("RestPoint"));

        machineMatsScript = GameObject.Find("Machine2").GetComponent<MachineMaterialScript>();

        entireMachine = GameObject.Find("Machine");
    }

    // Start is called before the first frame update
    void Start()
    {
        
        player = Camera.main.gameObject;

        //int i = 0;
        //int a = 0;
        //foreach (GameObject point in points)
        //{
        //    GameObject text = Instantiate(debuggingTextPrefab,worldSpaceCanvas.transform);


        //    text.transform.position = new Vector3(point.transform.position.x, point.transform.position.y + 0.5f, point.transform.position.z);

        //    TextMeshProUGUI tmp = text.GetComponent<TextMeshProUGUI>();

        //    tmp.text = i.ToString(); 
        //    i++;
        //}

        //foreach (GameObject point in carriers)
        //{
        //    Debug.Log("showing" + point.name + a);
        //    GameObject text = Instantiate(debuggingTextPrefab, worldSpaceCanvas.transform);
        //    text.transform.position = new Vector3(point.transform.position.x, point.transform.position.y + 0.5f, point.transform.position.z);
        //    TextMeshProUGUI tmp = text.GetComponent<TextMeshProUGUI>();

        //    Debug.Log(point.name + a);
        //    tmp.text = a.ToString(); 
        //    a++;
        //}
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Space))
        {
           // GameObject item = Instantiate(itemPrefab, startPoint.transform);
        }

        if (Input.GetKey(KeyCode.T))
        {
            ZoomIn();
        }
        if (Input.GetKey(KeyCode.Y))
        {
            ZoomOut();
        }
        if (Input.GetKey(KeyCode.U))
        {
            RestartSimulation();
        }

            DisplayCorrectNumberOfItemsToMake();
       // Debug.Log("Player is in " + player.transform.position + "\n machine is in " + entireMachine.transform.position);
    }

    /// <summary>
    /// Used in display 2 where buttons are used to start producing itemss
    /// </summary>
    public void StartProductionUI()
    {
        SortCarrier();

        foreach (GameObject carry in carriersInOrder)
        {
            ConveyorItemScript carrierScript = carry.GetComponent<ConveyorItemScript>();
            if (ListOfItemsToMake != null)
            {
                string currentItemToMake = ListOfItemsToMake[0];
                carrierScript.itemToMake_string = currentItemToMake;
                ListOfItemsToMake.Remove(currentItemToMake);

                

                carrierScript.MakeItem();
            }
        }
    }

    public void StartProductionAR(string itemToMake)
    {
        SortCarrier();

        foreach (GameObject carry in carriersInOrder)
        {
            ConveyorItemScript carrierScript = carry.GetComponent<ConveyorItemScript>();
            if (itemToMake != null)
            {
                carrierScript.itemToMake_string = itemToMake;

                carrierScript.MakeItem();
                break;
            }
        }
    }


    public void CircleInactiveItems()
    {
        foreach(GameObject carry in carriersInOrder)
        {
            if (carry != null)
            {
                ConveyorItemScript carrierScript = carry.GetComponent<ConveyorItemScript>();

                if (carrierScript.resting == true)
                {
                    //Debug.Log(carry.gameObject + "is resting! Moving now");
                    StartCoroutine(carrierScript.Circling());
                }
            }
        }
    }

    public void SortCarrier()
    {
        Array.Clear(carriersInOrder, 0, carriersInOrder.Length);

        int i = 0; 

        foreach (GameObject points in restPoints)
        {
            RestPointHolder restPointHOlder = points.GetComponent<RestPointHolder>();

            if (restPointHOlder.itemResting != null)
            {
                carriersInOrder[i] = restPointHOlder.itemResting;
                i++;
            }
        }
    }

    public void MakeItem(string itemType)
    {
        switch (itemType)
        {
            case "Flashlight":
                Debug.Log("Making flashLight!");
                StartProductionAR(itemType);
                break;
            case "USB":
                Debug.Log("Makine USB!");
                StartProductionAR(itemType);
                break;
            case "PushButton":
                Debug.Log("Making PushButton!");
                StartProductionAR(itemType);
                break;
            case "Limit":
                Debug.Log("Making Limit");
                StartProductionAR(itemType);
                break;
            case "Wifi":
                Debug.Log("Makine Wifi!");
                StartProductionAR(itemType);
                break;
        }
    }

    public void ZoomIn()
    {
        if(zoomIndex > 0)
        {
            zoomIndex--;
            float zoomScale = this.zoomScale[zoomIndex];

            entireMachine.transform.localScale = new Vector3(zoomScale, zoomScale, zoomScale);
            foreach(GameObject carry in carriers)
            {
                ConveyorItemScript carryScript = carry.GetComponent<ConveyorItemScript>();

                carryScript.ChangeZoomScale(zoomScale);
            }
        }

    }
    public void ZoomOut()
    {
        if(zoomIndex < 2)
        {
            zoomIndex++;
            float zoomScale = this.zoomScale[zoomIndex];

            entireMachine.transform.localScale = new Vector3(zoomScale, zoomScale, zoomScale);
            foreach (GameObject carry in carriers)
            {
                ConveyorItemScript carryScript = carry.GetComponent<ConveyorItemScript>();

                carryScript.ChangeZoomScale(zoomScale);
            }
        }
    }

    public void RestartSimulation()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void RepositionToPlayer()
    {
        Vector3 offsetHeight = new Vector3(0, 1f,0);
        float distance = zoomPos[zoomIndex];
        Vector3 playerVect = player.transform.position;
        Vector3 playerDir = player.transform.forward;
        playerDir.y = 0;

        Vector3 machine = playerVect + (playerDir * distance) - (offsetHeight * zoomScale[zoomIndex]);
        
        entireMachine.transform.position = machine;
        //ntireMachine.transform.forward = playerDir;
        
    }

    public void SwitchToFinalProduct(string matName, GameObject matGO, Transform itemPosition, ConveyorItemScript script)
    {
        Destroy(matGO);
        GameObject tempGO;

        switch (matName)
        {
            case "Flashlight":
                tempGO = Instantiate(finalFlashLight, itemPosition, true);
                tempGO.transform.position = itemPosition.position;
                tempGO.transform.localScale *= zoomScale[zoomIndex];
                script.material = tempGO;
                break;
            case "USB":
                tempGO = Instantiate(finalUSB, itemPosition,true);
                tempGO.transform.position = itemPosition.position;
                tempGO.transform.localScale *= zoomScale[zoomIndex];
                script.material = tempGO;
                break;
            case "PushButton":
                tempGO = Instantiate(finalPushButton, itemPosition, true);
                tempGO.transform.position = itemPosition.position;
                tempGO.transform.localScale *= zoomScale[zoomIndex];
                script.material = tempGO;
                break;
            case "Limit":
                tempGO = Instantiate(finalLimitSwitch, itemPosition, true);
                tempGO.transform.position = itemPosition.position;
                tempGO.transform.localScale *= zoomScale[zoomIndex];
                script.material = tempGO;
                break;
            case "Wifi":
                tempGO = Instantiate(finalWifi, itemPosition, true);
                tempGO.transform.position = itemPosition.position;
                tempGO.transform.localScale *= zoomScale[zoomIndex];
                script.material = tempGO;
                break;
        }
    }

    public GameObject[] SortArrays(GameObject[] arrayToSort)
    {
        Debug.Log("Sorting array");

        GameObject[] array = new GameObject[arrayToSort.Length];


        string debugTest = null;

        foreach (GameObject item in arrayToSort)
        {
            SortingValueScript valueScript = item.GetComponent<SortingValueScript>();

            array[valueScript.value] = item;
        }

        foreach (GameObject sort in array)
        {
            debugTest += sort.ToString() + "\n";
        }
        Debug.Log(debugTest);
        return array;
    }

    #region UI Functions
    public void UpdateItemCount()
    {
        Wifi = 0;
        LimitButton = 0;
        FlashLight = 0;
        USB = 0;
        PushButton = 0;

        foreach (string item in ListOfItemsToMake)
        {
            switch (item)
            {
                case "Flashlight":
                    FlashLight++;
                    break;
                case "USB":
                    USB++;
                    break;
                case "PushButton":
                    PushButton++;
                    break;
                case "Limit":
                    LimitButton++;
                    break;
                case "Wifi":
                    Wifi++;
                    break;
            }
        }
    }   
    public void DisplayCorrectNumberOfItemsToMake()
    {
        UpdateItemCount();

        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
          "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
    }

    public void AddWifi()
    {
        Wifi++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
            "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("Wifi");
        machineMatsScript.PutMatsOnPoint("Wifi");
    }
    public void AddLimit()
    {
        LimitButton++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
                    "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("Limit");
        machineMatsScript.PutMatsOnPoint("Limit");

    }
    public void AddFlashLight()
    {
        FlashLight++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
                    "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("Flashlight");
        machineMatsScript.PutMatsOnPoint("Flashlight");

    }
    public void AddUSB()
    {
        USB++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
                    "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("USB");
        machineMatsScript.PutMatsOnPoint("USB");

    }
    public void AddPushButton()
    {
        PushButton++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
                    "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("PushButton");
        machineMatsScript.PutMatsOnPoint("PushButton");

    }
    #endregion
}
