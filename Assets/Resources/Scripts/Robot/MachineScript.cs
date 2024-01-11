using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineScript : MonoBehaviour
{
    public GameObject pickRobot, placeRobot;

    RobotAnimationScript pickRobot_animScript, placeRobot_animScript;

    float time = 0;

    bool canCarrierMoveOn = false;

    MachineMaterialScript machineMatsScript;

    public Machine5Script machine5Script;

    public GameObject pointInMachine, productPoint;

    ErrorImageScript errorImage2Script, errorImage5Script;

    

    // Start is called before the first frame update
    void Start()
    {
        machineMatsScript = GameObject.Find("Machine2").GetComponent<MachineMaterialScript>();
        pickRobot_animScript = pickRobot.GetComponent<RobotAnimationScript>();
        placeRobot_animScript = placeRobot.GetComponent<RobotAnimationScript>();

        errorImage2Script = GameObject.Find("ErrorImageMachine2").GetComponent<ErrorImageScript>();
        errorImage5Script = GameObject.Find("ErrorImageMachine5").GetComponent<ErrorImageScript>();


        machine5Script = GetComponent<Machine5Script>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayPickAnimation()
    {
        pickRobot_animScript.PlayPickAnim();
    }
    public void PlayPlaceAnimation()
    {
        placeRobot_animScript.PlayPlaceAnim();
    }
    public void PlayPickTrigger()
    {
        pickRobot_animScript.PickTrigger();
    }
    public void PlayPlaceTrigger()
    {
        placeRobot_animScript.PlaceTrigger();
    }
    public void PlayPlaceMachine5()
    {
        placeRobot_animScript.Machine5PlaceAnimation();
    }

    public void SetToIdle()
    {
        pickRobot_animScript.ResetBools();
        placeRobot_animScript.ResetBools();
    }
    public bool GetCanCarrierGoBoolFromBothMachine()
    {
        if(pickRobot_animScript.canCarrierGo || placeRobot_animScript.canCarrierGo)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetIsMachineAnimationBoolFromBothMachine()
    {
        if (pickRobot_animScript.isAnimationDone || placeRobot_animScript.isAnimationDone)
        {
            return true;
        }
        else return false;
    }

    public bool AllowCarrierToMoveOn()
    {
        return canCarrierMoveOn;
    }
    public void ResetBothMachineBools()
    {
        pickRobot_animScript.ResetBools();
        placeRobot_animScript.ResetBools();
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

    public IEnumerator RunMachineSequence(int machineNumber,GameObject carrier, ConveyorItemScript carrierScript)
    {
       // Debug.Log("received " + machineNumber + "\n " + carrier + "\n" + carrierScript  );
        canCarrierMoveOn = false;
        switch (machineNumber)
        {
            case 0:
                if(gameObject.name == "Machine2")
                {
                    
                }
                break;
            case 1:
                if(gameObject.name == "Machine1")
                {
                    PlayPickTrigger();
                    yield return new WaitUntil(() => pickRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    PlayPlaceAnimation();
                    yield return new WaitUntil(() => placeRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    canCarrierMoveOn = true;

                }
                break;
            case 2:

                if (gameObject.name == "Machine2" && !carrierScript.pastStartingMachine)
                {
                    PlayPlaceTrigger();
                    yield return new WaitUntil(() => placeRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    canCarrierMoveOn = true;
                    carrierScript.pastStartingMachine = true;
                }
                else if (gameObject.name == "Machine2" && carrierScript.pastStartingMachine)
                {

                    ///this line of codes would play the error flashing first and then checks if there is 
                    ///available space to store the products, if there isnt, it waits for the player 
                    ///to make space by moving the products into the box. If there is space, run the code
                    ///to play the animation and move the product from the carrier into the machine.
                    errorImage2Script.isEnabled = true;
                    yield return new WaitUntil(() => machineMatsScript.CheckForEmptyProductPoint());
                    errorImage2Script.isEnabled = false;

                    productPoint = machineMatsScript.PutProductsOnPoints(pickRobot_animScript.materialToParent);

                    pickRobot_animScript.PickLastMachine();
                    yield return new WaitUntil(() => pickRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    canCarrierMoveOn = true;
                    break;
                }
                if (gameObject.name == "Machine2" && carrierScript.pastStartingMachine)
                {
                    Debug.Log("Too bad we breking");
                }
                    break;
            case 3:
                if (gameObject.name == "Machine3")
                {
                    PlayPickTrigger();
                    yield return new WaitUntil(() => pickRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    PlayPlaceAnimation();
                    yield return new WaitUntil(() => placeRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    canCarrierMoveOn = true;

                }
                break;
            case 4:
                if (gameObject.name == "Machine4")
                {
                    PlayPickTrigger();
                    yield return new WaitUntil(() => pickRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    PlayPlaceAnimation();
                    yield return new WaitUntil(() => placeRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    canCarrierMoveOn = true;

                }
                break;
            case 5:
                if (gameObject.name == "Machine5")
                {
                    errorImage5Script.isEnabled = true;
                    yield return new WaitUntil(() => machine5Script.IsThereMaterialsLeft());
                    errorImage5Script.isEnabled = false;

                    PlayPlaceMachine5();
                    yield return new WaitUntil(() => placeRobot_animScript.isAnimationDone);
                    machine5Script.DecreaseMaterialCount();
                    ResetBothMachineBools();

                    canCarrierMoveOn = true;

                }
                break;
        }
    }

    public void GetCarrierProperties(GameObject material, GameObject carryPoint)
    {
        //Debug.Log("In the machine SCriopt" + material.name + carryPoint.name);
        pickRobot_animScript.GetMaterialAndCarrierPoint(material, carryPoint);
        placeRobot_animScript.GetMaterialAndCarrierPoint(material,carryPoint);
    }

    public void SetMaterialToCarrier(ConveyorItemScript carrierScript)
    {
        machineMatsScript.SetCarrierMaterial(carrierScript);

    }

}
