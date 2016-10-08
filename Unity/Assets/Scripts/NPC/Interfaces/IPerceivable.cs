using UnityEngine;
using System.Collections;

namespace NPC {

    public enum NPC_PERCEIVABLE_TYPE {
        NPC,
        OBJECT
    }

    public enum PERCEIVE_WEIGHT {
        NONE,
        WEIGHTED,
        TOTAL
    }

    public interface IPerceivable {
        PERCEIVE_WEIGHT GetPerceptionWeightType();
        Transform GetTransform();
        Vector3 CalculateAgentRepulsionForce(IPerceivable p);
        Vector3 CalculateAgentSlidingForce(IPerceivable p);
        Vector3 CalculateRepulsionForce(IPerceivable p);
        Vector3 CalculateSlidingForce(IPerceivable p);
        float GetCurrentVelocity();
        Vector3 GetPosition();
        Vector3 GetForwardDirection();
        float GetAgentRadius();
    }

}
