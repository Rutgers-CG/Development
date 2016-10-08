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
    }

}
