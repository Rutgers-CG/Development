using UnityEngine;
using System.Collections;

using NPC;
using System;

public class GeoSpinner : MonoBehaviour, IPerceivable {
    public float GetAgentRadius() {
        throw new NotImplementedException();
    }

    public Vector3 GetCurrentVelocity() {
        throw new NotImplementedException();
    }

    public Vector3 GetForwardDirection() {
        throw new NotImplementedException();
    }

    public PERCEIVE_WEIGHT GetPerceptionWeightType() {
        return PERCEIVE_WEIGHT.WEIGHTED;
    }

    public Vector3 GetPosition() {
        throw new NotImplementedException();
    }

    public Transform GetTransform() {
        throw new NotImplementedException();
    }

    public Vector3 CalculateAgentRepulsionForce(IPerceivable p) {
        return Vector3.zero;
    }

    public Vector3 CalculateAgentSlidingForce(IPerceivable p) {
        return Vector3.zero;
    }

    public Vector3 CalculateRepulsionForce(IPerceivable p) {
        return Vector3.zero;
    }

    public Vector3 CalculateSlidingForce(IPerceivable p) {
        return Vector3.zero;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position, transform.up, 1f);
	}

    public PERCEIVEABLE_TYPE GetNPCEntityType() {
        return PERCEIVEABLE_TYPE.OBJECT;
    }
}
