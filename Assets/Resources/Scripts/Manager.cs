using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Manager : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject[] points;
    public GameObject[] restPoints;
    public GameObject[] carriers;

    public GameObject[] carriersInOrder = new GameObject[8];

    public int Wifi, LimitButton, FlashLight, USB, PushButton;

    public TextMeshProUGUI Amt;

    public List<string> ListOfItemsToMake = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        points = GameObject.FindGameObjectsWithTag("StopPoint");
        carriers = GameObject.FindGameObjectsWithTag("Carrier");
        restPoints = GameObject.FindGameObjectsWithTag("RestPoint");
       // GameObject item = Instantiate(itemPrefab, holder.transform);

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Space))
        {
           // GameObject item = Instantiate(itemPrefab, startPoint.transform);
        }

        DisplayCorrectNumberOfItemsToMake();
    }

    public void StartProduction()
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
    }
    public void AddLimit()
    {
        LimitButton++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
                    "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("Limit");
    }
    public void AddFlashLight()
    {
        FlashLight++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
                    "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("Flashlight");
    }
    public void AddUSB()
    {
        USB++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
                    "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("USB");
    }
    public void AddPushButton()
    {
        PushButton++;
        Amt.text = FlashLight.ToString() + "\n" + USB.ToString() +
                    "\n" + Wifi.ToString() + "\n" + LimitButton.ToString() + "\n" + PushButton.ToString();
        ListOfItemsToMake.Add("PushButton");
    }
}
