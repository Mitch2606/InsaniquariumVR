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

                var thisPosition = oldBubbies[i].pos;
                var thisForward = oldBubbies[i].forward;

                var centerOfMass = Vector3.zero;
                var heading = Vector3.zero;
                var separation = Vector3.zero;
                
                int flockSize = 0;
                for(int j = 0; j < bubbies.Length; j++) {
                    var bPosition = oldBubbies[j].pos;
                    var bForward = oldBubbies[j].forward;
                    
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
                toCenterOfMass *= (species.preferredFlockCount - flockSize) / species.preferredFlockCount;

                var dir = thisForward + toCenterOfMass + heading + separation;
                if(Physics.Raycast(new Ray(thisPosition, thisForward), out var hit, species.rayDistance)) {
                    var norm = hit.normal;

                    if(Physics.Raycast(hit.point, norm * species.rayDistance / 4, out var hit2)) {
                        norm = Vector3.Slerp(norm, hit2.normal, 0.25f);
                        norm = hit2.normal;
                    }

                    var factor = hit.distance / species.rayDistance;
                    dir += hit.normal * dir.magnitude / factor;
                }

                dir = dir.normalized;

                if(!box.bounds.Contains(thisPosition)) {
                    dir = box.bounds.center - thisPosition;
                }

                dir = Vector3.Slerp(thisForward, dir, 0.1f);

                bubbies[i].forward = dir;
                bubbies[i].pos = oldBubbies[i].pos + dir * species.speed * Time.deltaTime;
                
                bubbyObjs[i].transform.forward = dir;
                bubbyObjs[i].transform.position = bubbies[i].pos;
            }
        }
    }
}
