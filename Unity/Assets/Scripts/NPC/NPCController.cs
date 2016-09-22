using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace NPC {

    public class NPCController : MonoBehaviour, IPerceivable {

        [SerializeField]
        Dictionary<string, INPCModule> g_NPCModules;

        [SerializeField]
        GameObject g_SelectedEffect;

        private bool g_Selected;

        public bool DisplaySelectedHighlight = true;

        private static string SELECTION_EFFECT = "SelectionEffect";

        #region AI
        [SerializeField]
        private NPCAI gAI;
        #endregion

        #region BODY
        [SerializeField]
        private NPCBody gBody;
        #endregion

        #region PERCEPTION
        [SerializeField]
        private NPCPerception gPerception;
        #endregion

        [SerializeField]
        private bool gMainAgent = false;

        [SerializeField]
        private bool gInitialized = false;

        #region Properties
        public INPCModule[] NPCModules {
            get {
                if (g_NPCModules == null) return new INPCModule[0];
                INPCModule[] mods = new INPCModule[g_NPCModules.Count];
                g_NPCModules.Values.CopyTo(mods, 0);
                return mods;
            }
        }

        [SerializeField]
        public NPCPerception Perception {
            get { return gPerception; }
        }

        [SerializeField]
        public NPCAI AI {
            get { return gAI; }
        }

        [SerializeField]
        public NPCBody Body {
            get { return gBody; }
        }

        [SerializeField]
        public bool MainAgent {
            get { return gMainAgent; }
            set { gMainAgent = value; }
        }
        #endregion

        #region Public_Functions
        public void SetSelected(bool sel) {
            g_Selected = sel;
            g_SelectedEffect.SetActive(sel && DisplaySelectedHighlight);
        }

        public bool ContainsModule(INPCModule mod) {
            return g_NPCModules != null && g_NPCModules.ContainsKey(mod.NPCModuleName());
        }

        public void GoTo(Vector3 t) {
            List<Vector3> path = gAI.FindPath(t);
            if (path.Count <= 1)
                gBody.GoTo(t);
            else
                gBody.GoTo(path);
        }

        public bool AddNPCModule(INPCModule mod) {
            if (g_NPCModules == null) g_NPCModules = new Dictionary<string, INPCModule>();
            if (g_NPCModules.ContainsKey(mod.NPCModuleName())) return false;
            switch(mod.NPCModuleTarget()) {
                case NPC_MODULE_TARGET.AI:
                    gAI.SetNPCModule(mod);
                    break;
            }
            g_NPCModules.Add(mod.NPCModuleName(), mod);
            return true;
        }
        #endregion 

        #region Unity_Runtime
        // Use this for initialization
        void Start () {
            gBody = gameObject.GetComponent<NPCBody>();
            gPerception = gameObject.GetComponent<NPCPerception>();
            g_SelectedEffect = transform.FindChild(SELECTION_EFFECT).gameObject;
            SetSelected(MainAgent);
        }
	
        void FixedUpdate() {
            gPerception.UpdatePerception();
            gBody.UpdateBody();
        }

	    // Update is called once per frame
	    void Update () {
            if(g_Selected) {
                g_SelectedEffect.transform.Rotate(gameObject.transform.up, 1.0f);        
            }
        }

        // When script is added to GameObject or Reset
        void Reset() {
            if(!gInitialized) {
                g_NPCModules = new Dictionary<string, INPCModule>();
                Debug.Log("Creating NPCController");
                gMainAgent = false;
                if (GetComponent<NPCBody>() != null) DestroyImmediate(GetComponent<NPCBody>());
                if (GetComponent<NPCPerception>() != null) DestroyImmediate(GetComponent<NPCPerception>());
                InitializeNPCComponents();
                gInitialized = true;
            } else {
                Debug.Log("Loading existing NPCController settings");
            }
        }

        #endregion

        #region Private_Functions

        private void InitializeNPCComponents() {
            gAI = new NPCAI(this);
            gPerception = gameObject.AddComponent<NPCPerception>();
            gBody = gameObject.AddComponent<NPCBody>();
            CreateSelectedEffect();
            // hide flags
            gBody.hideFlags = HideFlags.HideInInspector;
            gPerception.hideFlags = HideFlags.HideInInspector;
        }

        private void CreateSelectedEffect() {
            Material m = Resources.Load<Material>("Materials/NPCSelectedCircle");
            if (m != null) {
                g_SelectedEffect = GameObject.CreatePrimitive(PrimitiveType.Plane);
                g_SelectedEffect.transform.parent = gameObject.transform;
                g_SelectedEffect.layer = LayerMask.NameToLayer("Ignore Raycast");
                g_SelectedEffect.GetComponent<MeshCollider>().enabled = false;
                g_SelectedEffect.transform.localScale = new Vector3(0.3f, 1.0f, 0.3f);
                g_SelectedEffect.transform.rotation = transform.rotation;
                g_SelectedEffect.name = SELECTION_EFFECT;
                g_SelectedEffect.transform.localPosition = new Vector3(0f, 0.2f, 0f);
                g_SelectedEffect.AddComponent<MeshRenderer>();
                MeshRenderer mr = g_SelectedEffect.GetComponent<MeshRenderer>();
                mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                mr.material = m;
            } else {
                Debug.Log("NPCController --> Couldn't load NPC materials, do not forget to add the to the Resources folder");
            }
        }

        #endregion

        #region IPerceivable
        PERCEIVE_WEIGHT IPerceivable.GetPerceptionWeightType() {
            return PERCEIVE_WEIGHT.WEIGHTED;
        }
        #endregion

    }

}
