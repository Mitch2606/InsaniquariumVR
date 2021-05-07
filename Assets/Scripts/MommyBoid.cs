using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MommyBoid : MonoBehaviour
{
    struct BoidInfo {
        public Vector3 pos;
        public Vector3 forward;
    }

    public Species species;

    public GameObject boidPrefab;

    [Range(1, 5000)]
    public uint babyCount = 15;

    public Collider box;

    private BoidInfo[] bubbies;
    private GameObject[] bubbyObjs;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        box.transform.localScale = new Vector3(10, box.transform.localScale.y, 10);
#endif

        if (bubbies == null) {
            bubbies = new BoidInfo[babyCount];
            bubbyObjs = new GameObject[babyCount];
            var bounds = box.bounds;
            for(int i = 0; i < babyCount; i++) {
                bubbies[i] = new BoidInfo {
                    pos = randomInBox(bounds),
                    forward = Random.onUnitSphere,
                };
                bubbyObjs[i] = Instantiate(boidPrefab);
            }
        }
    }

    Vector3 randomInBox(Bounds bounds)
    {
        return new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z));
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_ANDROID
        var playArea = OVRManager.boundary.GetDimensions(OVRBoundary.BoundaryType.PlayArea);
        box.transform.localScale = new Vector3(playArea.x, box.transform.localScale.y, playArea.z);
#endif

        // Handle dynamic resizing of baby count.
        if (bubbies.Length > babyCount) {
            var oldBubbies = bubbies;
            var oldObjs = bubbyObjs;
            bubbies = new BoidInfo[babyCount];
            bubbyObjs = new GameObject[babyCount];

            for(uint i = 0; i < babyCount; i++) {
                bubbies[i] = oldBubbies[i];
                bubbyObjs[i] = oldObjs[i];
            }
            for(uint i = babyCount; i < oldBubbies.Length; i++) {
                Destroy(oldObjs[i].gameObject);
            }
        }

        if(babyCount > bubbies.Length) {
            var oldBubbies = bubbies;
            var oldObjs = bubbyObjs;
            bubbies = new BoidInfo[babyCount];
            bubbyObjs = new GameObject[babyCount];
            for(int i = 0; i < oldBubbies.Length; i++) {
                bubbies[i] = oldBubbies[i];
                bubbyObjs[i] = oldObjs[i];
            }
            var bounds = box.bounds;
            for(int i = oldBubbies.Length; i < babyCount; i++) {
                bubbies[i] = new BoidInfo {
                    pos = randomInBox(bounds),
                    forward = Random.onUnitSphere,
                };
                bubbyObjs[i] = Instantiate(boidPrefab);
            }
        }

        unsafe {
            var oldBubbies = stackalloc BoidInfo[bubbies.Length];
            for(int i = 0; i < bubbies.Length; i++) {
                oldBubbies[i] = bubbies[i];
            }
            
            var flockSqr = species.localFlockRadius * species.localFlockRadius;
            var sepSqr = species.separationRadius * species.separationRadius;

            for(int i = 0; i < bubbies.Length; i++) {
                BoidStep(ref bubbies[i], oldBubbies, flockSqr, sepSqr);
                
                bubbyObjs[i].transform.forward = bubbies[i].forward;
                bubbyObjs[i].transform.position = bubbies[i].pos;
            }
        }
    }

    unsafe void BoidStep(ref BoidInfo self, BoidInfo* bubbies, float flockSqr, float sepSqr) {
        var babyCount = (int)this.babyCount;

        var thisPosition = self.pos;
        var thisForward = self.forward;
        //
        // If it's within the bounding box, use normal rules.
        if(box.bounds.Contains(thisPosition)) {

            var centerOfMass = Vector3.zero;
            var heading = Vector3.zero;
            var separation = Vector3.zero;
                    
            int flockSize = 0;
            for(int j = 0; j < babyCount; j++) {
                var bPosition = bubbies[j].pos;
                
                var awayFrom = thisPosition - bPosition;
                var sqrDist = awayFrom.sqrMagnitude;
                if(sqrDist > flockSqr) {
                    continue;
                }
                // The boid is within the local flock.
                flockSize++;

                centerOfMass += bPosition;
                heading += bubbies[j].forward;

                // The boid is too close, we should try to avoid it.
                if(sqrDist < sepSqr) {
                    separation += awayFrom;
                }
            }
            
            separation = Vector3.ClampMagnitude(separation, species.maxSeparationAmt);
                    
            // flockSize will always be > 0, as a boid will count itself as part of its flock.
            centerOfMass /= flockSize;
            heading /= flockSize;

            var toCenterOfMass = (centerOfMass - thisPosition).normalized;
            toCenterOfMass *= Mathf.Clamp(1 - (float)flockSize / species.preferredFlockCount, -3, 1);
            toCenterOfMass *= species.flockFactor;

            var dir = thisForward + toCenterOfMass + heading + separation;
            if(Physics.Raycast(new Ray(thisPosition, thisForward), out var hit, species.rayDistance)) {
                var norm = hit.normal;

                if(Physics.Raycast(hit.point, norm * species.rayDistance / 4, out var hit2)) {
                    norm = hit2.normal;
                }

                var distFactor = hit.distance / species.rayDistance;
                dir += hit.normal * dir.magnitude / distFactor * species.obstacleFactor;
            }

            dir = dir.normalized;
            dir = Vector3.Slerp(thisForward, dir, 0.1f);

            self.forward = dir;
            self.pos += dir * species.speed * Time.deltaTime;
        }
        //
        // If it's outside of the bounding box, teleport it to another random boid.
        else {
            const float TeleportDist = 1f;
            var closestPoint = box.bounds.ClosestPoint(thisPosition);
            var towardsBox = closestPoint - thisPosition;
            if(Vector3.SqrMagnitude(towardsBox) > TeleportDist * TeleportDist) {
                var newPos = bubbies[Random.Range(0, babyCount)].pos;
                if(!box.bounds.Contains(newPos)) {
                    // If the new point is still out of bounds, try again
                    newPos = bubbies[Random.Range(0, babyCount)].pos;
                            
                    if(!box.bounds.Contains(newPos)) {
                        // If it's still out of bounds,
                        // give up and just teleport to a random point in the box.
                        newPos = randomInBox(box.bounds);
                    }
                }

                Debug.Log("escapee!");

                self.pos = newPos;
                self.forward = Random.onUnitSphere;
            } else {
                self.forward = Vector3.Slerp(thisForward, towardsBox.normalized, 0.1f);
                self.pos += self.forward * species.speed * Time.deltaTime;
            }
        }
    }
}
