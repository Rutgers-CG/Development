using UnityEngine;
using System.Collections;
using System;

namespace NPC {

    public class NPCObject : MonoBehaviour, IPerceivable {

        #region Properties
        [SerializeField]
        public Transform MainInteractionPoint;

        [SerializeField]
        public PERCEIVE_WEIGHT PerceptionWeightType;

        [SerializeField]
        public string Name;
        #endregion

        #region Unity_Methods
        void Reset() {
            MainInteractionPoint = transform;
            PerceptionWeightType = PERCEIVE_WEIGHT.TOTAL;
        }
        #endregion


        #region IPerceivable
        public Vector3 GetMainLookAtPoint() {
            return transform.position;
        }

        public PERCEIVEABLE_TYPE GetNPCEntityType() {

            return PERCEIVEABLE_TYPE.OBJECT;
        }

        public PERCEIVE_WEIGHT GetPerceptionWeightType() {
            return PerceptionWeightType;
        }

        public Transform GetTransform() {
            return transform;
        }

        public Vector3 GetCurrentVelocity() {
            return Vector3.zero;  // assume always static for now.
        }

        public Vector3 GetPosition() {
            return transform.position;
        }

        public Vector3 GetForwardDirection() {
            return transform.forward;
        }

        /// <summary>
        /// Colliders need to be taken into consideration for objects, hence
        /// simple radius will not work.
        /// </summary>
        /// <returns>No Implementation Exception</returns>
        public float GetAgentRadius() {
            throw new NotImplementedException();
        }
        #endregion

        #region Utilities
        public override string ToString() {
            return Name;
        }
        #endregion
    }

}