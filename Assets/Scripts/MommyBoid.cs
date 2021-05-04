using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MommyBoid : MonoBehaviour
{
    public float speed = 10f;

    public float localFlockRadius = 5f;

    public float separationRadius = 1f;

    public float preferredFlockCount = 20;

    public float rayDistance = 2;

    public Boid boidPrefab;

    public uint babyCount = 15;

    public Collider box;

    [System.NonSerialized]
    public Boid[] bubbies;

    [System.NonSerialized]
    public Vector3 centerOfMass;

    [System.NonSerialized]
    public Vector3 averageHeading;

    // Start is called before the first frame update
    void Start()
    {
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
        var bounds = box.bounds;
        if (bubbies == null) {
            bubbies = new Boid[babyCount];
            var mid = (float)babyCount / 2;
            for(int i = 0; i < babyCount; i++) {
                bubbies[i] = Instantiate(boidPrefab, randomInBox(bounds), Random.rotation, this.transform);
            }
        }

        if (bubbies.Length > babyCount) {
            var oldBubbies = bubbies;
            bubbies = new Boid[babyCount];
            for(uint i = 0; i < babyCount; i++) {
                bubbies[i] = oldBubbies[i];
            }
            for(uint i = babyCount; i < oldBubbies.Length; i++) {
                Destroy(oldBubbies[i].gameObject);
            }
        }

        if(babyCount > bubbies.Length) {

            var oldBubbies = bubbies;
            bubbies = new Boid[babyCount];
            for(int i = 0; i < oldBubbies.Length; i++) {
                bubbies[i] = oldBubbies[i];
            }
            var mid = (float)babyCount / 2;
            for(int i = oldBubbies.Length; i < babyCount; i++) {
                bubbies[i] = Instantiate(boidPrefab, randomInBox(bounds), Random.rotation, this.transform);
            }
        }

        var com = Vector3.zero;
        var head = Vector3.zero;
        foreach(var b in bubbies) {
            com += b.transform.position;
            head += b.transform.forward;
        }
        this.centerOfMass = com / babyCount;
        this.averageHeading = head / babyCount;
    }
}
