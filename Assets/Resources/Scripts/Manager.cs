using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject[] points;

    public GameObject[] carriers;

    public int Wifi, LimitButton, FlashLight, USB, PushButton;

    public TextMeshProUGUI wifiAmt, LimitAmt, FlashLightAmt, USBAmt, PushButtonAmt;

    public List<string> ListOfItemsToMake = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        points = GameObject.FindGameObjectsWithTag("StopPoint");
        carriers = GameObject.FindGameObjectsWithTag("Carrier");
       // GameObject item = Instantiate(itemPrefab, holder.transform);

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Space))
        {
           // GameObject item = Instantiate(itemPrefab, startPoint.transform);
        }
    }

    public void StartProduction()
    {
        foreach(GameObject carry in carriers)
        {
            carry.GetComponent<ConveyorItemScript>().GetItemToMake();
        }
    }


    public void AddWifi()
    {
        Wifi++;
        wifiAmt.text = Wifi.ToString();
        ListOfItemsToMake.Add("Wifi");
    }
    public void AddLimit()
    {
        LimitButton++;
        LimitAmt.text = LimitButton.ToString();
        ListOfItemsToMake.Add("Limit");
    }
    public void AddFlashLight()
    {
        FlashLight++;
        FlashLightAmt.text = FlashLight.ToString();
        ListOfItemsToMake.Add("Flashlight");
    }
    public void AddUSB()
    {
        USB++;
        USBAmt.text = USB.ToString();
        ListOfItemsToMake.Add("USB");
    }
    public void AddPushButton()
    {
        PushButton++;
        PushButtonAmt.text = PushButton.ToString();
        ListOfItemsToMake.Add("PushButton");
    }
}
