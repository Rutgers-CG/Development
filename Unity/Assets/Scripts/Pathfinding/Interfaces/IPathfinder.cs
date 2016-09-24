using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding {
    
    public interface IPathfinder {

        string ObjectIdentifier();

        List<Vector3> FindPath(Vector3 from, Vector3 to);

        bool IsReachable(Vector3 from, Vector3 to);

        bool ComputeCost(NavNode n);
	
    }
}
