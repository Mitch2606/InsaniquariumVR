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
        var flockSqr = mommy.localFlockRadius * mommy.localFlockRadius;

        var flock = new List<(float sqrDist, Boid boid)>();
        foreach(var b in mommy.bubbies) {
            var sqrDist = Vector3.SqrMagnitude(b.transform.position - this.transform.position);
            if(sqrDist < flockSqr) {
                flock.Add((sqrDist, b));
            }
        }

        var sepSqr = mommy.separationRadius * mommy.separationRadius;
        var separation = Vector3.zero;
        for(int i = 0; i < flock.Count; i++) {
            if(flock[i].sqrDist < sepSqr) {
                separation += this.transform.position - flock[i].boid.transform.position;
            }
        }

        var centerOfMass = Vector3.zero;
        for(int i = 0; i < flock.Count; i++) {
            centerOfMass += flock[i].boid.transform.position;
        }
        centerOfMass /= flock.Count;


        var toCenterOfMass = (centerOfMass - this.transform.position).normalized;
        /*if(flock.Count > mommy.preferredFlockCount) {
            toCenterOfMass = -toCenterOfMass;
        }*/
        toCenterOfMass *= (mommy.preferredFlockCount - flock.Count) / mommy.preferredFlockCount;

        var heading = Vector3.zero;
        for(int i = 0; i < flock.Count; i++) {
            heading += flock[i].boid.transform.forward;
        }
        if(flock.Count > 0) {
            heading /= flock.Count;
        }



        var dir = transform.forward + toCenterOfMass + heading + separation;
        if(Physics.Raycast(new Ray(transform.position, transform.forward.normalized), out var hit, mommy.rayDistance)) {
            var norm = hit.normal;

            if(Physics.Raycast(hit.point, norm * mommy.rayDistance / 4, out var hit2)) {
                norm = Vector3.Slerp(norm, hit2.normal, 0.25f);
                norm = hit2.normal;
            }

            var factor = hit.distance / mommy.rayDistance;
            dir += hit.normal * dir.magnitude / factor;
        }

        dir = dir.normalized;

        if(!mommy.box.bounds.Contains(transform.position)) {
            dir = mommy.box.bounds.center - this.transform.position;
        }

        dir = Vector3.Slerp(transform.forward, dir, 0.1f);

        transform.rotation = Quaternion.LookRotation(dir);
        transform.Translate(Vector3.forward * mommy.speed * Time.deltaTime);
    }
}