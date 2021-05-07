using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Species", menuName = "Boids/Species", order = 1)]
public class Species : ScriptableObject
{
    [Header("Movement speed")]
    public float speed = 10f;

    [Header("Rule #1: Separation")]
    public float separationRadius = 1f;
    public float maxSeparationAmt = 3f;

    [Header("Rule #3: Cohesion")]
    public float localFlockRadius = 5f;
    public float flockFactor = 1f;
    public float preferredFlockCount = 20;
    
    [Header("Obstacle avoidance")]
    public float rayDistance = 2;
    public float obstacleFactor = 1f;
}
