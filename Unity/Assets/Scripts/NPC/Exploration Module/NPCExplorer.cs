using UnityEngine;
using System.Collections;
using System.Diagnostics;
using NPC;
using System;

public class NPCExplorer : MonoBehaviour, INPCModule {

    #region Members
    public float UpdateInSeconds = 1;
    private long g_UpdateCycle;
    private Stopwatch g_Stopwatch;
    private NPCController g_NPCController;
    private bool g_Enabled = true;
    #endregion

    #region Public_Functions
    #endregion

    #region Unity_Methods
    // Use this for initialization
    void Start () {
        g_NPCController = GetComponent<NPCController>();
        g_UpdateCycle = (long) (UpdateInSeconds * 1000);
        g_Stopwatch = System.Diagnostics.Stopwatch.StartNew();
        g_Stopwatch.Start();
	}
	
	/// <summary>
    /// No npc module should be updated from here but from its TickModule method
    /// which will be only called from the NPCController on FixedUpdate
    /// </summary>
	void Update () { }

    #endregion

    #region NPCModule
    public bool IsEnabled() {
        return g_Enabled;
    }

    public string NPCModuleName() {
        return "NavGrid Exporation";
    }

    public NPC_MODULE_TARGET NPCModuleTarget() {
        return NPC_MODULE_TARGET.AI;
    }

    public NPC_MODULE_TYPE NPCModuleType() {
        return NPC_MODULE_TYPE.EXPLORATION;
    }

    public void RemoveNPCModule() {
        // destroy components in memroy here
    }

    public void SetEnable(bool e) {
        g_Enabled = e;
    }

    public bool IsUpdateable() {
        return true;
    }

    public void TickModule() {
        if(g_Enabled) { 
            if(g_Stopwatch.ElapsedMilliseconds > g_UpdateCycle) {
                g_NPCController.Debug("Updating NPC Module: " + NPCModuleName());
                g_Stopwatch.Reset();
                g_Stopwatch.Start();
            }
        }
    }

    #endregion
}
