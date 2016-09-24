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
        public NavNode(Vector3 position, Vector2 gridPos, bool walkable, float radius, NavGrid grid) {
            g_Radius = radius;
            g_Position = position;
            g_Walkable = walkable;
            g_GridPosition = gridPos;
            g_Grid = grid;
        }
        public NavNode(Vector3 position, Vector2 gridPos, Vector3 up, bool walkable, float radius, NavGrid grid) 
            : this(position,gridPos, walkable, radius, grid) {
            g_Up = up;
        }
        public NavNode(Vector3 position, Vector2 gridPos, Vector3 up, float blockingHeight, bool walkable, float radius, NavGrid grid) 
            : this(position, gridPos, up, walkable, radius, grid) {
            g_BlockingHeight = blockingHeight;
        }
        #endregion

        #region Properties

        public bool Available;

        public float Radius {
            get { return g_Radius; }
        }
        public Vector3 Position {
            get { return g_Position; }
        }
        public Vector2 GridPosition {
            get {  return g_GridPosition; }             
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
        private GameObject  g_Tile;
        private NavGrid     g_Grid;
        private float       g_Radius;
        private Vector3     g_Position;
        private Vector3     g_Up;
        private float       g_BlockingHeight;
        private bool        g_Walkable;
        private int         g_Weight= 1;
        private Vector2     g_GridPosition;
        #endregion

        #region Public_Functions

        public void SetHighlightTile(bool h, Color c) {
            if(h) {
                g_Tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
                g_Tile.GetComponent<BoxCollider>().isTrigger = true;
                Material m = new Material(Shader.Find("Standard"));
                m.color = c;
                g_Tile.GetComponent<Renderer>().material = m;
                g_Tile.layer = LayerMask.NameToLayer("Ignore Raycast");
                g_Tile.transform.position = Position + Up * 0.05f;
                g_Tile.transform.localScale = new Vector3(Radius * 2f, 0.01f, Radius * 2f);
                g_Tile.transform.parent = g_Grid.transform;
            } else {
                if(g_Tile != null) {
                    GameObject.DestroyImmediate(g_Tile);
                    g_Tile = null;
                }
            }
        }

        public override string ToString() {
            return "NavNode @ ["+g_Position+"]";
        }

        public bool IsWalkable() {
            if(g_Up != null) {
                Ray ray = new Ray(g_Position, g_Up);
                RaycastHit hit;
                // nothing is on top of the node nor colliding by its radius
                Available =  !(Physics.Raycast(Position, Up, out hit, g_BlockingHeight) ||
                          Physics.Raycast(Position + new Vector3(Radius,0,0), Up, g_BlockingHeight) ||
                          Physics.Raycast(Position + new Vector3(-Radius, 0, 0), Up, g_BlockingHeight) ||
                          Physics.Raycast(Position + new Vector3(0, 0, Radius), Up, g_BlockingHeight) ||
                          Physics.Raycast(Position + new Vector3(0, 0, -Radius), Up, g_BlockingHeight));
                if(hit.collider) {
                    IPathfinder ipf = hit.collider.GetComponent<IPathfinder>();
                    if (ipf != null) {
                        g_Grid.SetIPathfinderNode(ipf, this);
                    }
                }
                return Available;
            } else return true;
        }
        #endregion
    }
}