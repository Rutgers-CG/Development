using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Pathfinding;


namespace NPC {

    [System.Serializable]
    public class NPCAI {
        
        public NPCAI(NPCController pController) {
            this.gNPCController = pController;
            gPathfinders = new Dictionary<string, IPathfinder>();
            gPathfinders.Add("None", null);
        }

        #region Properties

        public IPathfinder CurrentPathfinder;

        public Dictionary<string,IPathfinder> Pathfinders {
            get {
                if (gPathfinders == null) InitPathfinders();
                return gPathfinders;
            }
        }

        #endregion

        #region NPC_Modules
        private NPCController gNPCController;
        #endregion

        #region NPC_Goals
        private Stack<NPCGoal> gGoals;
        #endregion

        #region Members
        private Dictionary<string, NPCAttribute> gAttributes;

        [SerializeField]
        private Dictionary<string, IPathfinder> gPathfinders;
        #endregion

        #region Public_Functions

        public void SetNPCModule(INPCModule mod) {
            if (gPathfinders == null) InitPathfinders();
            switch(mod.NPCModuleType()) {
                case NPC_MODULE_TYPE.PATHFINDER:
                    gPathfinders.Add(mod.NPCModuleName(),mod as IPathfinder);
                    break;
            }
        }

        public List<Vector3> FindPath(Vector3 target) {
            List<Vector3> path = new List<Vector3>();
            if(CurrentPathfinder == null) {
                path.Add(target);
                return path;
            } else {
                return CurrentPathfinder.FindPath(gNPCController.transform.position, target);
            }
        }

        #endregion

        #region Traits

        #region Private_Functions
        private void InitPathfinders() {
            gPathfinders = new Dictionary<string, IPathfinder>();
            gPathfinders.Add("None", null);
        }
        #endregion

        /* For the purpose of initialization */
        private bool gRandomizeTraits;

        bool RandomizeTraits {

            get {
                return gRandomizeTraits;
            }
            set {
                gRandomizeTraits = value;
            }
        }

        #endregion

        protected void InitializeTraits() {
            foreach (PropertyInfo pi in this.GetType().GetProperties()) {
                object[] attribs = pi.GetCustomAttributes(true);
                if(attribs.Length > 0) {

                }
            }
        }

        #region Attributes
    
        [NPCAttribute("NPC",typeof(bool))]
        public bool NPC { get; set; }

        [NPCAttribute("Charisma",typeof(float))]
        public float Charisma { get; set; }

        [NPCAttribute("Friendliness",typeof(float))]
        public float Friendliness { get; set; }
    
        [NPCAttribute("Strength",typeof(int))]
        public int Strength { get; set; }

        [NPCAttribute("Intelligence",typeof(int))]
        public int Intelligence { get; set; }

        [NPCAttribute("Dexterity",typeof(int))]
        public int Dexterity { get; set; }

        [NPCAttribute("Constitution",typeof(int))]
        public int Constitution { get; set; }

        [NPCAttribute("Hostility", typeof(float))]
        public float Hostility { get; set; }

        [NPCAttribute("Location",typeof(Vector3))]
        public Vector3 Location { get; set; }

        #endregion

    }
}