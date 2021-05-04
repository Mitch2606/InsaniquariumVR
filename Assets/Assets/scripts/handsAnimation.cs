using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handsAnimation : MonoBehaviour
{
    public bool modeFlag;

    // Start is called before the first frame update
    void Start()
    {
        modeFlag = 0;
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        //Pressing A button turns player passive
        if ((OVRInput.GetDown(OVRInput.Button.One)))
        {
            modeFlag = 0;
            passiveMode();
        }

        //Pressing B button turns player hostile
        if ((OVRInput.GetDown(OVRInput.Button.Two)))
        {
            modeFlag = 1;
            angryMode();
        }

        //Pressing Trigger
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) >= 0.5f)
        {
            shooting = 1;
            shoot();
        }

    }

    void FixedUpdate()
    {
        OVRInput.FixedUpdate();

    }

    void angryMode()
    {

    }

    void passiveMode()
    {

    }

    void shoot()
    {

    }
}


