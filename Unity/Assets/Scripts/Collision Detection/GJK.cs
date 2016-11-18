using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using NPC;
using System;

namespace Collisions {
        
    [RequireComponent(typeof(MeshFilter))]
    public class GJK : MonoBehaviour, INPCModule {

        #region Members
        private bool g_Colliding;
        private MeshFilter g_MeshFilter;
        private Vector3[] g_Vertices;
        private List<List<Vector3>> g_Triangles;
        private Vector3[] g_Directions = { Vector3.up, Vector3.right, Vector3.forward };
        private Dictionary<GJK, bool> g_CurrentCollisions;
        #endregion

        #region Properties
        public Transform TestGJK;
        public bool Enabled = true;
        public List<List<Vector3>> Triangles;
        #endregion

        #region Unity_Functions
        void LateUpdate() {
            GJK[] bodies = FindObjectsOfType<GJK>();
            foreach (GJK b in bodies) {
                if(b != this) {
                    if (DetectCollision(b)) {
                        g_Colliding = true;
                        Debug.Log(g_Colliding);
                    }
                }
                else g_Colliding = false;
            }
        }

        void Awake() {
            Reset();
        }

        void Reset() {
            g_MeshFilter = GetComponent<MeshFilter>();
            g_CurrentCollisions = new Dictionary<GJK, bool>();
            if (g_MeshFilter == null) {
                Debug.Log("No Mesh Filter found for object, please add one");
                enabled = false;
            } else {
                g_Vertices = g_MeshFilter.sharedMesh.vertices;
                for (int i = 0; i < g_MeshFilter.sharedMesh.triangles.Length; i+=3) {
                    int t = i / 3;
                    g_Triangles.Add(new List<Vector3>());
                    g_Triangles[t].Add(g_Vertices[g_MeshFilter.sharedMesh.triangles[0 * i  ]]);
                    g_Triangles[t].Add(g_Vertices[g_MeshFilter.sharedMesh.triangles[0 * i+1]]);
                    g_Triangles[t].Add(g_Vertices[g_MeshFilter.sharedMesh.triangles[0 * i+2]]);
                }
            }
        }

        void OnDrawGizmos() {
            GJK test = TestGJK == null ? null : TestGJK.GetComponent<GJK>();
            if (g_Colliding) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, transform.localScale * 1.05f);
            }
            if(test != null) {

            }
            
        }
        #endregion

        #region Public_Functions

        public bool DetectCollision(GJK body) {
            Simplex simplex = new Simplex();
            Vector3 direction = Vector3.Normalize(transform.position - body.transform.position);
            simplex.AddVertex(SupportFunction(this,body, direction));
            direction = -1 * direction;
            while (true) {
                simplex.AddVertex(SupportFunction(this, body, direction));
                if(Vector3.Dot(simplex.GetLastVertex(),direction) <= 0) {
                    return false;
                } else {
                    if (simplex.ContainsOrigin(out direction)) return true;
                }
            }
        }

        public Vector3 FurthestPoint(Vector3 direction) {
            Vector3 furthestPoint = Vector3.zero;
            if (g_Vertices.Length > 0) {
                furthestPoint = g_Vertices[0];
                float maxDot = Vector3.Dot(transform.TransformPoint(furthestPoint), direction);
                foreach (Vector3 p in g_Vertices) {
                    float curDot = Vector3.Dot(transform.TransformPoint(p), direction);
                    if (curDot > maxDot) {
                        maxDot = curDot;
                        furthestPoint = p;

                    }
                }
            }
            return furthestPoint;
        }
        #endregion

        #region Private_Functions
        private Vector3 SupportFunction(GJK p1, GJK p2, Vector3 direction) {
            Vector3 minkPoint = Vector3.zero;
            Vector3 mP1 = this.FurthestPoint(direction);
            Vector3 mP2 = p2.FurthestPoint(-1 * direction);
            Vector3 diff = mP1 - mP2;
            minkPoint = new Vector3(diff.x, diff.y, diff.z);
            return minkPoint;
        }
    #endregion

    #region INPCModule
    public bool IsEnabled() {
            return Enabled;
        }

        public string NPCModuleName() {
            return "GJK_Module";
        }

        public NPC_MODULE_TARGET NPCModuleTarget() {
            return NPC_MODULE_TARGET.BODY;
        }

        public NPC_MODULE_TYPE NPCModuleType() {
            throw new NotImplementedException();
        }

        public void RemoveNPCModule() {
            throw new NotImplementedException();
        }

        public void SetEnable(bool e) {
            Enabled = e;
        }
        #endregion

        #region Support_Classes
        class Simplex {

            public List<Vector3> Vertices;

            public Simplex( ) {
                Vertices = new List<Vector3>();
            }

            public void AddVertex(Vector3 point) {
                Vertices.Add(point);
            }

            public Vector3 GetLastVertex() {
                return Vertices[Vertices.Count - 1];
            }

            public bool ContainsOrigin(out Vector3 d) {
                // get the last point added to the simplex
                Vector3 a = GetLastVertex();
                Vector3 pA, pB, pC;
                Vector3 lineA0, lineAB, lineAC, ABperp, ACperp;
                
                // compute AO (same thing as -A)
                pA = GetLastVertex();
                lineA0 = -1 * pA;

                if (Vertices.Count == 3) {
                    // then its the triangle case
                    // get b and c
                    pB = Vertices[1];
                    pC= Vertices[0];

                    lineAB = new Vector3(pB.x - pA.x, 0, pB.z - pA.z);
                    lineAC = new Vector3(pC.x - pA.x, 0, pC.z - pA.z);
                    // compute the edges
                    ABperp = TripleProduct(lineAC, lineAB, lineAB);
                    // compute the normals
                    ACperp = TripleProduct(lineAB, lineAC, lineAC);

                    // is the origin in R4
                    if (Vector3.Dot(ABperp,lineA0) > 0) {
                        // remove point c
                        Vertices.Remove(pC);
                        // set the new direction to abPerp
                        d = ABperp;
                    } else {
                        // is the origin in R3
                        d = ACperp;
                        if (Vector3.Dot(ACperp, lineA0) > 0) {
                            // remove point b
                            Vertices.Remove(pB);
                            // set the new direction to acPerp
                        } else {
                            // otherwise we know its in R5 so we can return true
                            return true;
                        }
                    }
                } else {
                    // then its the line segment case
                    pB = Vertices[0];
                    // compute AB
                    lineAB = new Vector3(pB.x - pA.x, 0f, pB.z - pA.z);
                    // get the perp to AB in the direction of the origin
                    ABperp = TripleProduct(lineAB, lineA0, lineA0);
                    // set the direction to abPerp
                    d = ABperp;
                }
                return false;
            }

            private Vector3 TripleProduct(Vector3 lineA, Vector3 lineB, Vector3 lineC) {
                Vector3 tripleProduct = new Vector3(
                    lineB.x * Vector3.Dot(lineC, lineA) - lineA.x * Vector3.Dot(lineC, lineB),
                    0f,
                    lineB.z * Vector3.Dot(lineC, lineA) - lineA.z * Vector3.Dot(lineC, lineB));
                return tripleProduct;
            }

        }
        #endregion
    }
}
