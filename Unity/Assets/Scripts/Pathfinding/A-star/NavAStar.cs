using UnityEngine;
using System.Collections;
using Pathfinding;
using System;
using System.Collections.Generic;

using NPC;

public class NavAStar : MonoBehaviour, IPathfinder, INPCModule {

    public bool EnableNPCModule = true;

    public bool ComputeCost(NavNode n) {
        throw new NotImplementedException();
    }

    public List<Vector3> FindPath(Vector3 from, Vector3 to) {
        List<Vector3> list = new List<Vector3>();
        list.Add(to);
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
}
