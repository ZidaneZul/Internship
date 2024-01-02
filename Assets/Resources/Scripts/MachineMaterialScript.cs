using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineMaterialScript : MonoBehaviour
{
    #region scripts

    BoxCollider boxColl;
    Manager manager;
    MaterialPointHolder matsPointHolder, matsPointHolder2, productHolder;

    #endregion

    #region Materials Prefab and Points for Materials to be in

    public GameObject Material_USB;
    public GameObject Material_Flashlight;
    public GameObject Material_Wifi;
    public GameObject Material_Limit;
    public GameObject Material_Push;

    public GameObject[] Mats_Points;
    public GameObject[] Product_Points;

    #endregion

    public bool didAddNewMat = false;

    // Start is called before the first frame update
    void Start()
    {
        boxColl = GetComponent<BoxCollider>();

        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();

        Mats_Points = GameObject.FindGameObjectsWithTag("MaterialPoint");

        Product_Points = GameObject.FindGameObjectsWithTag("ProductPoint");
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("Testing testing test test");
    }

    public void PutMatsOnPoint(string material)
    {
        Debug.Log("PutMatsOnPoint is running!");
        didAddNewMat = false;

        foreach(GameObject point in Mats_Points)
        {
            matsPointHolder = point.GetComponent<MaterialPointHolder>();
            if (matsPointHolder.matWaiting == null)
            {
             //   Debug.Log("POint is in " + point.transform.position + "\n " +
             //       "material is " + material);
                GameObject matPoint = null;

                switch (material)
                {
                    case "Flashlight":
                         matPoint = Instantiate(Material_Flashlight, point.transform);
                        break;
                    case "USB":
                        matPoint = Instantiate(Material_USB, point.transform);
                        break;
                    case "PushButton":
                        matPoint = Instantiate(Material_Push, point.transform);
                        break;
                    case "Limit":
                        matPoint = Instantiate(Material_Limit, point.transform);
                        break;
                    case "Wifi":
                        matPoint = Instantiate(Material_Wifi, point.transform);
                        break;
                }

                matsPointHolder.matWaiting = matPoint;
                didAddNewMat = true;
             
                break;
            }
        }
    }

    public GameObject PutProductsOnPoints(GameObject product)
    {
        foreach(GameObject point in Product_Points)
        {
            productHolder = point.GetComponent<MaterialPointHolder>();

            if(productHolder.matWaiting == null)
            {
                productHolder.matWaiting = product;
                product.transform.parent = point.transform;
                product.transform.position = point.transform.position;
                return point;
            }
        }
        return null;
    }

    public void SetCarrierMaterial(ConveyorItemScript carrierScript)
    {
        foreach(GameObject point in Mats_Points)
        {
            matsPointHolder2 = point.GetComponent<MaterialPointHolder>();
            Debug.Log("Mats is " + point.name);

            if(matsPointHolder2.matWaiting != null)
            {
                Debug.Log("There is a material in " + matsPointHolder2.gameObject + 
                    "\n which is " + matsPointHolder2.matWaiting);
                carrierScript.material = matsPointHolder2.matWaiting;
                matsPointHolder2.matWaiting = null;
                break;
            }
        }
    }


}
