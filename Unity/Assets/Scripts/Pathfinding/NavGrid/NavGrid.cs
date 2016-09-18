using UnityEngine;
using System.Collections;


namespace Pathfinding {
    public class NavGrid : MonoBehaviour {

        #region Properties
        #endregion

        #region Members
        public bool RedrawGrid;
        public LayerMask UnwalkableMask;
        public Vector2 GridDimensions;
        public float GridTransparency = 1.0f;
        public float NodeRadius = 0.5f;
        public float BlockingHeight = 2.0f;
        NavNode[,] g_Grid;
        public int AvailableWeight = 1;
        public int MediumWeight = 50;
        public int NotAvailableWeight = 100;
        private bool g_TileSelected = false;
        private NavNode g_SelectedTile;
        public Vector2 SelectedTile;
        public int SelectedTileWeight = 1;
        #endregion

        #region Private_Functions
        private void PopulateGrid() {
            int tilesX = (int) GridDimensions.x,
                tilesY = (int) GridDimensions.y;
            float nodeDiameter = NodeRadius * 2;
            g_Grid = new NavNode[tilesX, tilesY];
            Vector3 gridWorldBottom = transform.position - (transform.right * GridDimensions.x / 2) - 
                (transform.forward * GridDimensions.y/2) + new Vector3(NodeRadius,0.0f,NodeRadius);
            for(int row = 0; row < tilesX; ++row) {
                for (int col = 0; col < tilesY; ++col) {
                    NavNode node = new NavNode(
                        gridWorldBottom + (transform.right * (nodeDiameter) * row) + transform.forward * (nodeDiameter) * col,
                        transform.up,
                        BlockingHeight,
                        true,
                        NodeRadius);
                    g_Grid[row, col] = node;
                }
            }

        }

        #endregion Private_Functions

        #region Unity_Methods
        // Use this for initialization
        void Reset() {
            float nodeDiameter = NodeRadius * 2;
            GridDimensions.x = Mathf.RoundToInt(transform.localScale.x / nodeDiameter);
            GridDimensions.y = Mathf.RoundToInt(transform.localScale.z / nodeDiameter);
            PopulateGrid();
        }

        void Awake () {
            float nodeDiameter = NodeRadius * 2;
            int   gridSizeX = Mathf.RoundToInt(GridDimensions.x / nodeDiameter),
                  gridSizeY = Mathf.RoundToInt(GridDimensions.y / nodeDiameter);
            PopulateGrid();
        }


        // Update is called once per frame
        void Update () { }

        void OnDrawGizmos() {

            if(AvailableWeight <= 0) {
                AvailableWeight = 1;
            }

            if(NotAvailableWeight < MediumWeight) {
                NotAvailableWeight = MediumWeight + 1;
            } else if(MediumWeight < AvailableWeight) {
                MediumWeight = AvailableWeight + 1;
            }

            // x for rows, y for cols
            Gizmos.DrawWireCube(transform.position, new Vector3(GridDimensions.x, transform.position.y , GridDimensions.y));
            if(g_Grid == null || g_Grid.GetLength(0) != GridDimensions.x || g_Grid.GetLength(1) != GridDimensions.y || RedrawGrid) {
                Reset();
                RedrawGrid = false;
            } else {

                if (g_SelectedTile != null) {
                    g_SelectedTile.Weight = SelectedTileWeight > 0 ? SelectedTileWeight : 1;
                    g_SelectedTile.Selected = false;
                }
                NavNode tmp = g_Grid[(int)SelectedTile.x, (int)SelectedTile.y];
                if (g_SelectedTile != tmp) {
                    SelectedTileWeight = tmp.Weight;
                }
                g_SelectedTile = tmp;
                g_SelectedTile.Selected = true;
                g_SelectedTile = tmp;

                foreach(NavNode node in g_Grid) {
                    bool avail = node.IsWalkable();
                    Color c;
                    if(node.Selected) {
                        c = Color.white;
                        c.a = 1.0f * GridTransparency;
                    } else {
                        if (node.Weight >= NotAvailableWeight || !avail) {
                            c = Color.red;
                            c.a = 0.5f * GridTransparency;
                        } else if (node.Weight >= MediumWeight) {
                            c = Color.yellow;
                            c.a = 0.3f * GridTransparency;
                        } else {
                            c = Color.green;
                            c.a = 0.1f * GridTransparency;
                        }
                    }
                    Gizmos.color = c;
                    Gizmos.DrawWireCube(node.Position, new Vector3(NodeRadius, transform.position.y, NodeRadius));
                    Gizmos.color = Color.white;
                }
            }
        }
        #endregion
    }
}
