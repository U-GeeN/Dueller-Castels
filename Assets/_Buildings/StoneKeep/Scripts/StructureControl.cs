using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureControl : MonoBehaviour, IsActionable {
    
    public Vector3 closedRot; //the door's closed rotation
    public Vector3 openRot; //the door's new (open) rotation
    public Vector3 startRot;
    public int rotateSpeed = 80; //how fast the door opens

    public bool opened = false; //whether the door is opened or closed
    public bool action = false;
    float progress = 0.0f;

    // Use this for initialization
    void Start ()
    {
        startRot = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update ()
    {

        if (action)
        {
            
            action = false;
            SetAction();
        }
    }


    void OpenDoor () 
    {
        //transform.eulerAngles = Vector3.RotateTowards(transform.localEulerAngles, newRot, 0.1f * rotateSpeed, 0.0f);
        if (opened)
        {
            transform.localEulerAngles = Vector3.Lerp(openRot, closedRot, progress += 0.025f);

        }
        else
        {
            transform.localEulerAngles = Vector3.Lerp(closedRot, openRot, progress += 0.025f);
        }

        if (progress >= 1f)
        {
            opened = !opened;
            CancelInvoke("OpenDoor");
        }
    }

    // interface IsActionable 
    // vom Anderen Objekt aufgerufen

    public void SetAction ()
    {
        if (IsInvoking("OpenDoor")){
            return;
        }
        startRot = transform.localEulerAngles;
        progress = 0.0f;
        InvokeRepeating("OpenDoor", 0.0f, 0.05f);
    }

   

}

public interface IsActionable 
{
    void SetAction();
}