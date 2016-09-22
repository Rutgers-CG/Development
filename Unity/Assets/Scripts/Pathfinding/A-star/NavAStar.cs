using UnityEngine;
using System.Collections;
using Pathfinding;
using System;
using System.Collections.Generic;

using NPC;

public class NavAStar : MonoBehaviour, IPathfinder, INPCModule {

    bool g_NPCModuleEnabled = true;

    public bool ComputeCost(NavNode n) {
        throw new NotImplementedException();
    }

    public List<Vector3> FindPath(Vector3 from, Vector3 to) {
        throw new NotImplementedException();
    }

    public bool IsEnabled() {
        return g_NPCModuleEnabled;
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
        g_NPCModuleEnabled = e;
    }
}
