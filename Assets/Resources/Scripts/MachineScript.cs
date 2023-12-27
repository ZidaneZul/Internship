using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineScript : MonoBehaviour
{
    public GameObject pickRobot, placeRobot;

    Animator pickRobot_anim, placeRobot_anim;

    RobotAnimationScript pickRobot_animScript, placeRobot_animScript;

    float time = 0;

    bool canCarrierMoveOn = false;  


    // Start is called before the first frame update
    void Start()
    {
        pickRobot_anim = pickRobot.GetComponent<Animator>();
        placeRobot_anim = placeRobot.GetComponent<Animator>();
        
        pickRobot_animScript = pickRobot.GetComponent<RobotAnimationScript>();
        placeRobot_animScript = placeRobot.GetComponent<RobotAnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayPickAnimation()
    {
        pickRobot_animScript.PlayPickAnim();
    }

    public void PlayPickTrigger()
    {
        pickRobot_animScript.PickTrigger();
    }

    public void PlayPlaceTrigger()
    {
        placeRobot_animScript.PlaceTrigger();
    }


    public void PlayPlaceAnimation()
    {
        placeRobot_animScript.PlayPlaceAnim();
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
        if (canCarrierMoveOn)
        {
            return true;
        }
         return false;
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

    public IEnumerator RunMachineSequence(int machineNumber,GameObject carrier)
    {
        canCarrierMoveOn = false;
        switch (machineNumber)
        {
            case 0:
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
                if(gameObject.name == "Machine2")
                {
                    PlayPickTrigger();
                    yield return new WaitUntil(() => pickRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();
                    
                    canCarrierMoveOn = true;

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
                    PlayPlaceAnimation();
                    yield return new WaitUntil(() => placeRobot_animScript.isAnimationDone);
                    ResetBothMachineBools();

                    canCarrierMoveOn = true;

                }
                break;
        }
    }
}
