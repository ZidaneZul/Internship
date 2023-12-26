using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnimationScript : MonoBehaviour
{
    Animator anim;

    public GameObject Claw_GO;

    public GameObject carrier, pointInMachine;

    public bool canCarrierGo;

    public MachineScript parentMachineScript;

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
        anim.SetBool("Pick", true);
    }

    public void PlayPlaceOnPick()
    {
        Debug.Log("playing place anim after pick");
        anim.SetBool("Pick", false);
        parentMachineScript.PlayPlaceAnimation();
    }

    public void PlayPlaceAnim()
    {
        canCarrierGo = false;
        anim.SetBool("Place", true);
    }


    public void ResetBools()
    {
        anim.SetBool("Pick", false);
        anim.SetBool("Place", false);
        canCarrierGo = false;
    }


    public void PickItemUp(GameObject item)
    {
        item.transform.parent = Claw_GO.transform;
    }
    public void PlaceItemOnCarrier(GameObject gameObject)
    {
        gameObject.transform.parent = carrier.transform;
    }
    public void PlaceItemInside(GameObject GO)
    {
        GO.transform.parent = pointInMachine.transform;
    }

    public void SetBoolForCarriersToGo()
    {
        canCarrierGo = true;
    }



}
