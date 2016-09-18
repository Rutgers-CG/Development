using UnityEngine;
using System.Collections;

namespace Pathfinding {

    public class NavNode {

        #region Enums
        public enum NODE_STATE {
            WALKABLE,
            NONWALKABLE
        }
        #endregion

        #region Constructor
        public NavNode(Vector3 position, bool walkable, float radius) {
            g_Radius = radius;
            g_Position = position;
            g_Walkable = walkable;
        }
        public NavNode(Vector3 position, Vector3 up, bool walkable, float radius) : this(position, walkable, radius) {
            g_Up = up;
        }
        public NavNode(Vector3 position, Vector3 up, float blockingHeight, bool walkable, float radius) : this(position, up, walkable, radius) {
            g_BlockingHeight = blockingHeight;
        }
        #endregion

        #region Properties
        public float Radius {
            get { return g_Radius; }
        }
        public Vector3 Position {
            get { return g_Position; }
        }
        public float BlockingHeight {
            get { return g_BlockingHeight; }
        }
        public Vector3 Up {
            get { return g_Up; }
        }
        public bool Walkable {
            get { return g_Walkable; }
        }
        public int Weight {
            get { return g_Weight; }
            set { g_Weight = value;  }
        }
        public bool Selected;
        #endregion

        #region Members
        private float    g_Radius;
        private Vector3  g_Position;
        private Vector3  g_Up;
        private float    g_BlockingHeight;
        private bool     g_Walkable;
        private int      g_Weight= 1;
        #endregion

        #region Public_Functions
        public bool IsWalkable() {
            if(g_Up != null) {
                Ray ray = new Ray(g_Position, g_Up);
                // nothing is on top of the node nor colliding by its radius
                return  !(Physics.Raycast(Position, Up, g_BlockingHeight) ||
                          Physics.Raycast(Position + new Vector3(Radius,0,0), Up, g_BlockingHeight) ||
                          Physics.Raycast(Position + new Vector3(-Radius, 0, 0), Up, g_BlockingHeight) ||
                          Physics.Raycast(Position + new Vector3(0, 0, Radius), Up, g_BlockingHeight) ||
                          Physics.Raycast(Position + new Vector3(0, 0, -Radius), Up, g_BlockingHeight));
            } else return true;
        }
        #endregion
    }
}