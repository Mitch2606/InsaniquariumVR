using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Orbiting : MonoBehaviour
{

    public float RotationSpeed = 100f;
    public float OrbitSpeed = 50f;
    public float DesiredMoonDistance = 10;
    public Transform target;
    public bool randomize = false;

    void Start()
    {
        DesiredMoonDistance = Vector3.Distance(target.position, transform.position);
    }

    void Update()
    {
        if (randomize == true)
        {
            RotationSpeed = UnityEngine.Random.Range(50f, 100f);
            OrbitSpeed = UnityEngine.Random.Range(50f, 100f);
        }

        transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        transform.RotateAround(target.position, Vector3.up, OrbitSpeed * Time.deltaTime);

        //fix possible changes in distance
        float currentMoonDistance = Vector3.Distance(target.position, transform.position);
        Vector3 towardsTarget = transform.position - target.position;

        transform.position += (DesiredMoonDistance - currentMoonDistance) * towardsTarget.normalized;
    }
}