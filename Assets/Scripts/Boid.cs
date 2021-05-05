using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private MommyBoid mommy;

    void Awake()
    {
        this.mommy = FindObjectOfType<MommyBoid>();
    }

    // Update is called once per frame
    void Update()
    {
        var thisPosition = this.transform.position; // Cache these values because the properties are slow to call.
        var thisForward = this.transform.forward;

        var flockSqr = mommy.localFlockRadius * mommy.localFlockRadius;
        var sepSqr = mommy.separationRadius * mommy.separationRadius;

        var centerOfMass = Vector3.zero;
        var heading = Vector3.zero;
        var separation = Vector3.zero;

        int flockSize = 0;
        var flock = new List<(float sqrDist, Boid boid)>();
        for(int i = 0; i < mommy.bubbies.Length; i++) {
            var bPosition = mommy.bubbies[i].transform.position;
            var bForward = mommy.bubbies[i].transform.forward;
            
            var sqrDist = Vector3.SqrMagnitude(bPosition - thisPosition);
            if(sqrDist > flockSqr) {
                continue;
            }
            // The boid is within the local flock.
            flockSize++;

            centerOfMass += bPosition;
            heading += bForward;

            // The boid is too close, we should try to avoid it.
            if(sqrDist < sepSqr) {
                separation += thisPosition - bPosition;
            }
        }

        // flockSize will always be > 0, as a boid will count itself as part of its flock.
        centerOfMass /= flockSize;
        heading /= flockSize;

        var toCenterOfMass = (centerOfMass - thisPosition).normalized;
        toCenterOfMass *= (mommy.preferredFlockCount - flockSize) / mommy.preferredFlockCount;


        var dir = thisForward + toCenterOfMass + heading + separation;
        if(Physics.Raycast(new Ray(thisPosition, thisForward), out var hit, mommy.rayDistance)) {
            var norm = hit.normal;

            if(Physics.Raycast(hit.point, norm * mommy.rayDistance / 4, out var hit2)) {
                norm = Vector3.Slerp(norm, hit2.normal, 0.25f);
                norm = hit2.normal;
            }

            var factor = hit.distance / mommy.rayDistance;
            dir += hit.normal * dir.magnitude / factor;
        }

        dir = dir.normalized;

        if(!mommy.box.bounds.Contains(thisPosition)) {
            dir = mommy.box.bounds.center - thisPosition;
        }

        dir = Vector3.Slerp(thisForward, dir, 0.1f);

        transform.rotation = Quaternion.LookRotation(dir);
        transform.Translate(Vector3.forward * mommy.speed * Time.deltaTime);
    }
}
