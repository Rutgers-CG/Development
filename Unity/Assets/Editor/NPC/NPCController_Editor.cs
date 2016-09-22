using UnityEditor;
using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

namespace NPC {

    [CustomEditor(typeof(NPCController))]
    public class NPCController_Editor : Editor {

        NPCController gController;

        #region Constants
        private const string label_ViewAngle = "View Angle";
        private const string label_PerceptionRadius = "Perception Radius";
        private const string label_BodyNavigation = "Navigation";
        private const string label_IKEnabled = "Enable IK";
        private const string label_MainAgent = "Main Agent";
        private const string label_SelectHighlight = "Enable Selection Indicator";
        private const string label_AnimatorEnabled = "Use Animator";
        private const string label_UseAnimCurves = "Use Animation Curves";
        private const string label_NavStoppingThresh = "Breaking Threshold";
        private const string label_AIPathfind = "Pathfinder";
        private const string label_NPCLoadedMods = "Loaded NPC Modules";
        #endregion

        #region Insperctor_GUI
        private bool gGeneralSettings = true;
        private bool gShowPerception = true;
        private bool gShowBody = true;
        private bool gShowAI = true;
        private bool gShowMods = true;
        #endregion

        public override void OnInspectorGUI() {


            gController = (NPCController) target;
            
            /*
             * Look for added compatible added components, extend with interface later on
             */
            /**/

            EditorGUI.BeginChangeCheck();

            
            if (gController.GetComponent<INPCModule>() != null) {
                gShowMods = EditorGUILayout.Foldout(gShowMods, "NPC Modules");
                if (gShowMods) {
                    INPCModule[] modules = gController.GetComponents<INPCModule>();
                    foreach(INPCModule m in modules) {
                        if (!gController.ContainsModule(m)) {
                            Debug.Log("Loading NPC Module -> " + m.NPCModuleName());
                            if(!gController.AddNPCModule(m)) {
                                DestroyImmediate((Object) m);
                            }
                        }
                    }
                
                    INPCModule[] mods = gController.NPCModules;
                    foreach(INPCModule m in mods) {
                        EditorGUILayout.LabelField(m.NPCModuleName());
                    }
                }
            } else EditorGUILayout.LabelField("No NPC Modules Loaded");

            gGeneralSettings = EditorGUILayout.Foldout(gGeneralSettings, "General Settings");
            if(gGeneralSettings) { 
                gController.MainAgent = (bool)EditorGUILayout.Toggle(label_MainAgent, (bool)gController.MainAgent);
                gController.DisplaySelectedHighlight = (bool)EditorGUILayout.Toggle(label_SelectHighlight, (bool)gController.DisplaySelectedHighlight);
            }

            /* Perception Sliders */
            gShowPerception = EditorGUILayout.Foldout(gShowPerception, "Perception") && gController.Perception != null;
            if(gShowPerception) {
                gController.Perception.ViewAngle = (float) EditorGUILayout.IntSlider(label_ViewAngle, (int) gController.Perception.ViewAngle, 
                    (int) NPCPerception.MIN_VIEW_ANGLE, 
                    (int) NPCPerception.MAX_VIEW_ANGLE);
                gController.Perception.PerceptionRadius = (float) EditorGUILayout.IntSlider(label_PerceptionRadius, (int) gController.Perception.PerceptionRadius, 
                    (int) NPCPerception.MIN_PERCEPTION_FIELD, 
                    (int) NPCPerception.MAX_PERCEPTION_FIELD);
            }

            gShowAI = EditorGUILayout.Foldout(gShowAI, "AI") && gController.AI != null;

            if(gShowAI) {
                if(gController.AI.Pathfinders != null) {
                    string[] pfds = new string[gController.AI.Pathfinders.Count];
                    gController.AI.Pathfinders.Keys.CopyTo(pfds, 0);
                    int selected = 0;
                    if(gController.AI.CurrentPathfinder != null) {
                        for (int i = 0; i < pfds.Length; ++i) { 
                            if (pfds[i] == ((INPCModule)gController.AI.CurrentPathfinder).NPCModuleName())
                                selected = i;
                        }
                    }
                    selected = EditorGUILayout.Popup("Pathfinders", selected , pfds);
                    gController.AI.CurrentPathfinder = gController.AI.Pathfinders[pfds[selected]];
                }
            }

            gShowBody = EditorGUILayout.Foldout(gShowBody, "Body") && gController.Body != null;
            if(gShowBody) {
                // gController.Body.SteeringNavigation = (bool)EditorGUILayout.Toggle(label_BodyNavigation, (bool)gController.Body.SteeringNavigation);
                // gController.Body.NavMeshNavigation = (bool)EditorGUILayout.Toggle(label_NavMeshNavigation, (bool)gController.Body.NavMeshNavigation);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(label_BodyNavigation);
                gController.Body.Navigation = (NAV_STATE)EditorGUILayout.EnumPopup((NAV_STATE)gController.Body.Navigation);
                EditorGUILayout.EndHorizontal();
                if(gController.Body.Navigation != NAV_STATE.DISABLED)
                    gController.Body.NavDistanceThreshold = (float) EditorGUILayout.FloatField(label_NavStoppingThresh, (float) gController.Body.NavDistanceThreshold);
                gController.Body.IKEnabled = (bool)EditorGUILayout.Toggle(label_IKEnabled, (bool)gController.Body.IKEnabled);
                gController.Body.UseAnimatorController = (bool)EditorGUILayout.Toggle(label_AnimatorEnabled, (bool)gController.Body.UseAnimatorController);
                gController.Body.UseCurves = (bool)EditorGUILayout.Toggle(label_UseAnimCurves, (bool)gController.Body.UseCurves);
            }

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(gController, "Parameter Changed");
                EditorUtility.SetDirty(gController);
            }
        }

        private void OnSceneGUI() {
            if(gController != null) {
                if(gShowPerception) {
                    Transform t = gController.Perception.PerceptionField.transform;
            
                    /* Draw View Angle */
                    float angleSplit = gController.Perception.ViewAngle / 2;
                    Debug.DrawRay(t.position,
                        Quaternion.AngleAxis(angleSplit, Vector3.up) * t.rotation * Vector3.forward * gController.Perception.PerceptionRadius, Color.red);
                    Debug.DrawRay(t.position, 
                        Quaternion.AngleAxis((-1) * angleSplit, Vector3.up) * t.rotation * Vector3.forward * gController.Perception.PerceptionRadius, Color.red);
                }
            }
        }

    }

}
