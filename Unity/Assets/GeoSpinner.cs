using UnityEngine;
using System.Collections;

using NPC;
using System;

public class GeoSpinner : MonoBehaviour, IPerceivable {

    public PERCEIVE_WEIGHT GetPerceptionWeightType() {
        return PERCEIVE_WEIGHT.WEIGHTED;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(transform.position, transform.up, 1f);
	}
}
