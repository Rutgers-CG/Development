using UnityEngine;
using System.Collections;
using Pathfinding;
using System;
using System.Collections.Generic;

using NPC;

[System.Serializable]
public class NavAStar : MonoBehaviour, IPathfinder, INPCModule {

    private NPCController g_NPCController;

    [SerializeField]
    public bool EnableNPCModule = true;

    public bool ComputeCost(NavNode n) {
        throw new NotImplementedException();
    }

    public List<Vector3> FindPath(Vector3 from, Vector3 to) {
        List<Vector3> list = new List<Vector3>();
        RaycastHit hit;
        if(Physics.Raycast(new Ray(transform.position + (transform.up * 0.2f), -1 * transform.up), out hit)) {
            NavGrid grid = hit.collider.GetComponent<NavGrid>();
            NavNode rootNode = grid.GetOccupiedNode(this);
            g_NPCController.Debug("NavAStar --> A* working for " + this + ", starting from: " + rootNode);
            list.Add(to);
        } else {
            g_NPCController.Debug("NavAStar --> Pathfinder not on grid");    
        }
        return list;
    }

    public bool IsEnabled() {
        return EnableNPCModule;
    }

    public bool IsReachable(Vector3 from, Vector3 target) {
        throw new NotImplementedException();
    }

    public string NPCModuleName() {
        return "A* Algorithm";
    }

    public NPC_MODULE_TYPE NPCModuleType() {
        return NPC_MODULE_TYPE.PATHFINDER;
    }

    public NPC_MODULE_TARGET NPCModuleTarget() {
        return NPC_MODULE_TARGET.AI;
    }

    public void SetEnable(bool e) {
        EnableNPCModule = e;
    }

    public string ObjectIdentifier() {
        return gameObject.name;
    }

    public void RemoveNPCModule() {
        GetComponent<NPCController>().RemoveNPCModule(this);
    }

    #region Unity_methods
    void Reset() {
        g_NPCController = GetComponent<NPCController>();
    }

    void Start() {
        Reset();
    }

    void OnDestroy() {
        RemoveNPCModule();
    }
    #endregion
}
