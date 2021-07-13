using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabObjects : MonoBehaviour
{
    public Transform self;

    public float speed = 1;
    public int controllerState;

    public Vector3 translationVector = new Vector3(0,0,1);
    public Vector3 scaleVector = new Vector3(0, 0.7f, 0);

    public float minDistance;
    public float maxDistance;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Transform>();

        minDistance = self.localPosition.z;
        maxDistance = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        controllerState = controllerInputs.controllerIn;

        updateHand(controllerState);



    }

    //Collision Stuff
    void OnCollisionEnter(Collision col)
    {
        //Check Mode and grasp Buttons
        if ((controllerState & 16) == 16)
        {
            //Peaceful Mode
            if ((controllerState & 1) == 0)
            {
                /*
                if (col.tag == "Fish")
                {
                    
                }

                else if (col.tag == "Enemy")
                {
                    Debug.log("Enemy Gets Picked Up");
                }
                */
                if (col.gameObject.tag == "wall")
                {
                    return;
                }
                else
                {
                    grab(col.gameObject);
                }
            }
            //Angry Mode
            else
            {
                if (col.gameObject.tag == "fish")
                {
                    // Debug.log("Kill fish");
                }

                else if (col.gameObject.tag == "enemy")
                {
                    //Debug.log("Enemy Gets Hurt");
                }

                else
                {

                }
            }
        }
    }

    void grab(GameObject Object)
    {
        var ObjectsTransform = Object.GetComponent<Transform>();

        var newPosition = self.Find("EndOfTentacle").GetComponent<Transform>().position;

        ObjectsTransform.position = newPosition;
    }

    //MoveTenticle
    void updateHand(int controllerState)
    {
        if(self.localPosition.z > maxDistance)
        {
            self.localPosition = maxDistance * translationVector;
            self.localScale = new Vector3(1, maxDistance - 0.3f, 1);
        }
        if ((controllerState & 2) == 2)
        {
            self.localPosition += translationVector * Time.deltaTime * speed;
            self.localScale += scaleVector * Time.deltaTime * speed;
        }
        else if (self.localPosition.z > minDistance)
        {
            self.localPosition -= translationVector * Time.deltaTime * speed;
            self.localScale -= scaleVector * Time.deltaTime * speed;
        }
        else
        {
            self.localPosition = minDistance * translationVector;
            self.localScale = new Vector3(1, minDistance - 0.3f, 1);
        }
    }
}
