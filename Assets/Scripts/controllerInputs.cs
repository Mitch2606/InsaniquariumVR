using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerInputs : MonoBehaviour
{
    public int controllerIn;

    // Start is called before the first frame update
    void Start()
    {
        controllerIn = 0;
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        //Pressing A button turns player passive
        if ((OVRInput.GetDown(OVRInput.RawButton.A)) || (OVRInput.GetDown(OVRInput.RawButton.X)))
        {
            controllerIn &= ~1;
        }

        //Pressing B button turns player hostile
        if ((OVRInput.GetDown(OVRInput.RawButton.B)) || (OVRInput.GetDown(OVRInput.RawButton.Y)))
        {
            controllerIn |= 1;
        }
    
        //Pressing Trigger R
        if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) >= 0.5f)
        {
            controllerIn |= 2;
        }
        else
        {
            controllerIn &= ~2;
        }

        //Pressing Trigger L
        if (OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger) >= 0.5f)
        {
            controllerIn |= 4;
        }
        else
        {
            controllerIn &= ~4;
        }

        //Grasping Something L 
        if (OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger) >= 0.5f)
        {
            controllerIn |= 8;
        }
        else
        {
            controllerIn &= ~8;
        }

        //Grasping Something R 
        if (OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) >= 0.5f)
        {
            controllerIn |= 16;
        }
        else
        {
            controllerIn &= ~16;
        }
    }

    void FixedUpdate()
    {
        OVRInput.FixedUpdate();

    }

}


