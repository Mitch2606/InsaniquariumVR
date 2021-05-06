using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Species", menuName = "Boids/Species", order = 1)]
public class Species : ScriptableObject
{
    public float speed = 10f;
    public float localFlockRadius = 5f;
    public float separationRadius = 1f;
    public float preferredFlockCount = 20;
    public float rayDistance = 2;
    public float obstacleFactor = 1f;
}
