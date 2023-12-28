using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnimationScript : MonoBehaviour
{
    Animator anim;

    public GameObject Claw_GO, pointInMachine, carrierPoint;

    public bool canCarrierGo, isAnimationDone;

    public MachineScript parentMachineScript;

    public GameObject materialToParent;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        parentMachineScript = transform.parent.GetComponent<MachineScript>();
    }

    /// <summary>
    /// Would rather use triggers but its running mulitple times so using bool to make it
    /// run once
    /// </summary>
    public void PlayPickAnim()
    {
        canCarrierGo = false;
        isAnimationDone = false; 
        anim.SetBool("Pick", true);
    }

    public void PickTrigger()
    {
        canCarrierGo = false;
        isAnimationDone = false;
        anim.SetTrigger("PickTrigger");
    }
    public void PlaceTrigger()
    {
        canCarrierGo=false;
        isAnimationDone=false;
        anim.SetTrigger("PlaceTrigger");
    }

    public void PlayPlaceOnPick()
    {
        Debug.Log("playing place anim after pick");
        anim.SetBool("Pick", false);
        isAnimationDone = false;
        parentMachineScript.PlayPlaceAnimation();
    }

    public void PlayPlaceAnim()
    {
        canCarrierGo = false;
        isAnimationDone = false;
        anim.SetBool("Place", true);
    }
    public void ResetBools()
    {
        anim.SetBool("Pick", false);
        anim.SetBool("Place", false);
        canCarrierGo = false;
        isAnimationDone = false;
    }

    public void EndOfAnimation()
    {
        isAnimationDone = true;
        
    }

    public void SetBoolForCarriersToGo()
    {
        canCarrierGo = true;
    }

    public void ParentMatToCLaw()
    {
        materialToParent.transform.parent = Claw_GO.transform;
    }

    public void ParentMatToPointInMachine()
    {
        materialToParent.transform.parent = pointInMachine.transform; 
    }

    public void ParentMatToCarrier()
    {
        materialToParent.transform.parent = carrierPoint.transform;
    }

    public void GetMaterialAndCarrierPoint(GameObject material, GameObject carryPoint)
    {
        Debug.Log("rooted function is running");
        materialToParent = material;
        carrierPoint = carryPoint;
    }



}
