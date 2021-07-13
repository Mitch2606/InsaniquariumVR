using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateBraclet : MonoBehaviour
{
    public GameObject deathCrystal;
    public GameObject feedCrystal;
    public Color[] colors = { Color.grey, Color.red , Color.blue };

    public Renderer thisRendP;
    public Renderer thisRendA;

    public int controllerState;

    // Start is called before the first frame update
    void Start()
    {
       // controllerInputBoi = GameObject.Find("ControllerCube");
        deathCrystal = GameObject.Find("CrystalDeath");
        feedCrystal = GameObject.Find("CrystalFeed");

        thisRendP = feedCrystal.GetComponent<Renderer>(); 
        thisRendA = deathCrystal.GetComponent<Renderer>(); 
        
    }

    // Update is called once per frame
    void Update()
    {
        controllerState = controllerInputs.controllerIn;

        if ((controllerState & 1) == 1)
        {
            //AngryMode
            //Changes death Crystal to Red
            //Changes Feed crystal to grey
            thisRendA.material.color = colors[1];
            thisRendP.material.color = colors[0];
        }
        else
        {
            //PassiveMode
            //Changes Death Crystal grey
            //Changes Feed Crystal blue
            thisRendA.material.color = colors[0];
            thisRendP.material.color = colors[2];
        }
        
    }
}
