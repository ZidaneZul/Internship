using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotAnimationScript : MonoBehaviour
{
    Animator anim;

    public GameObject Claw_GO, carrierPoint;

    public bool canCarrierGo, isAnimationDone;

    public MachineScript parentMachineScript;
    public Machine5Script machine5Script;

    public GameObject materialToParent, machine5Material;

    GameObject pointInMachine;

    Rigidbody rb;

    AudioSource audioSource;
    public AudioClip Short1, Short2, Long1;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        parentMachineScript = transform.parent.GetComponent<MachineScript>();

        machine5Script = transform.parent.GetComponent<Machine5Script>();

        pointInMachine = parentMachineScript.pointInMachine;

        audioSource = GetComponent<AudioSource>();
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

    public void PickLastMachine()
    {
        canCarrierGo=false;
        isAnimationDone=false;
        anim.SetTrigger("FinalMachine");
        
    }

    public void PlaceTrigger()
    {
        canCarrierGo=false;
        isAnimationDone=false;
        anim.SetTrigger("PlaceTrigger");
    }

    public void PlayPlaceAnim()
    {
        canCarrierGo = false;
        isAnimationDone = false;
        anim.SetBool("Place", true);
    }

    public void Machine5PlaceAnimation()
    {
        canCarrierGo = false;
        isAnimationDone = false;
        anim.SetTrigger("Machine5PlaceTrigger");
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
        materialToParent.transform.position = Claw_GO.transform.position;
    }

    public void ParentMatToPointInMachine()
    {
        materialToParent.transform.parent = pointInMachine.transform; 
        materialToParent.transform.position = pointInMachine.transform.position;
    }
    public void ParentMatToFinalProductStore()
    {
        GameObject point = parentMachineScript.productPoint;
        materialToParent.transform.parent = point.transform;
        materialToParent.transform.position = point.transform.position;
        materialToParent.transform.rotation = new Quaternion(0,0,0,0);
        rb = materialToParent.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.velocity = new Vector3(0,0,0);
    }

    public void ParentMatToCarrier()
    {
        materialToParent.transform.parent = carrierPoint.transform;
        materialToParent.transform.position = carrierPoint.transform.position;
        materialToParent.transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    public void Machine5ParentMatToClaw()
    {
        GameObject mat = machine5Script.GetMaterial();
        mat.transform.parent = Claw_GO.transform;
        mat.transform.position = Claw_GO.transform.position;
        mat.transform.rotation = new Quaternion(0, 0, 0, 0);
        machine5Material = mat;
    }

    public void DestroyMaterialMachine5()
    {
        Destroy(machine5Material);
    }

    /// <summary>
    /// this code will get the material and carrier to run the animation, gets called in conveyorScript
    /// </summary>

    public void GetMaterialAndCarrierPoint(GameObject material, GameObject carryPoint)
    {
        //Debug.Log("rooted function is running");
        materialToParent = material;
        carrierPoint = carryPoint;
    }

    public void PlayShort1Audio()
    {
        audioSource.PlayOneShot(Short1, 0.5f);
    }
    public void PlayShort2Audio()
    {
        audioSource.PlayOneShot(Short2, 0.5f);
    }
    public void PlayLong1Audio()
    {
        audioSource.PlayOneShot(Long1, 0.5f);
    }



}
