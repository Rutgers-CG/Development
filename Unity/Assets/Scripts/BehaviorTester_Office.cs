using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using System;
using NPC;

public class BehaviorTester_Office : MonoBehaviour {

    private static string SPAWN_POINT_TAG = "Spawn_Point";
    private static string BUILDING_PATROL_POINTS_TAG = "Building_Patrol_Points";
    private static string ACTOR_STREET_TAG = "Actor_Street";
    private static string ACTOR_TAG = "Actor";
    private static string GUARD_A_BUILDING = "O'Hara";
    private static string POLICEMAN = "Policeman_Charles";

    [SerializeField]
    public Transform[] OutBuildingPatrolPoints;
    public Transform[] PolicemanPatrolPoints;

    static int g_LastSpawPoint = 0;

    public int MaximumNPCAgents = 5;
    private int g_CurrentNPCs = 0;
    
    public Transform targetLocation;
    public Transform BusSitLocation;
    public Transform BusSitLocationB;
    public Vector3 originalLocation;
    private NPCBehavior g_Agent, g_AgentB, g_Guard_A, g_Policeman, g_BusTaker, g_BusTakerB;
    public GameObject agent;
    public GameObject secondAgent;
    public GameObject FirstOrientation;

    [SerializeField]
    public bool Enabled = false;

    public GameObject[] NPCStreetAgents;
    GameObject[] g_SpawningPoints;
    HashSet<NPCBehavior> g_ActiveAgents;

    Dictionary<GameObject, NPCBehavior> g_NPCBehaviorActors;

    private BehaviorAgent behaviorAgent;

    #region Unity_Methods

    void Start() {
        NPCStreetAgents = GameObject.FindGameObjectsWithTag(ACTOR_STREET_TAG);
        g_SpawningPoints = GameObject.FindGameObjectsWithTag(SPAWN_POINT_TAG);
        g_Guard_A = GameObject.Find(GUARD_A_BUILDING).GetComponent<NPCBehavior>();
        g_Policeman = GameObject.Find(POLICEMAN).GetComponent<NPCBehavior>();
        g_BusTaker = GameObject.Find("Jordan").GetComponent<NPCBehavior>();
        g_BusTakerB = GameObject.Find("Alfred").GetComponent<NPCBehavior>();
        InitializeBehaviors();
    }

    private void InitializeBehaviors() {
        if (Enabled) {

            /* Just for testing */
            g_ActiveAgents = new HashSet<NPCBehavior>();
            g_NPCBehaviorActors = new Dictionary<GameObject, NPCBehavior>();
            g_Agent = agent.GetComponent<NPCBehavior>();
            g_AgentB = secondAgent.GetComponent<NPCBehavior>();
            BehaviorEvent behEvent = new BehaviorEvent(doEvent => this.BuildTreeRoot(), new IHasBehaviorObject[] { g_Agent });
            behEvent.StartEvent(1f);
            /* ---------------- */

            /* Outside guard */
            BehaviorEvent guarding = new BehaviorEvent(action => g_Guard_A.NPCBehavior_PatrolRandomPoints(OutBuildingPatrolPoints),
                new IHasBehaviorObject[] { g_Guard_A });
            guarding.StartEvent(1f);

            BehaviorEvent patroling = new BehaviorEvent(action => g_Policeman.NPCBehavior_PatrolRandomPoints(PolicemanPatrolPoints),
                new IHasBehaviorObject[] { g_Policeman });
            patroling.StartEvent(1f);

            BehaviorEvent busTaker = new BehaviorEvent(action => g_BusTaker.NPCBehavior_TakeSit(BusSitLocation),
                new IHasBehaviorObject[] { g_BusTaker });
            busTaker.StartEvent(1f);

            BehaviorEvent busTakerB = new BehaviorEvent(action => g_BusTakerB.NPCBehavior_TakeSit(BusSitLocationB),
                new IHasBehaviorObject[] { g_BusTakerB });
            busTakerB.StartEvent(1f);
        }
    }

    void FixedUpdate() {
        if(Enabled) {
            foreach (GameObject b in NPCStreetAgents) {
                NPCBehavior behAgent = b.GetComponent<NPCBehavior>();
                Transform point = g_SpawningPoints[g_LastSpawPoint].transform;
                g_LastSpawPoint = ((int) (UnityEngine.Random.value * 1399))  % g_SpawningPoints.Length;
                if (!g_ActiveAgents.Contains(behAgent) && behAgent.Behavior.Status != BehaviorStatus.InEvent) {
                    Transform targetLoc = g_SpawningPoints[g_LastSpawPoint].transform;
                    BehaviorEvent e = new BehaviorEvent(doEvent => behAgent.ApproachAndWait(targetLoc, (g_LastSpawPoint % 2 == 0)), new IHasBehaviorObject[] { (IHasBehaviorObject) behAgent });
                    e.StartEvent(1.0f);
                    g_ActiveAgents.Add(behAgent);
                }
                if(behAgent.Behavior.CurrentEvent != null) {
                    g_ActiveAgents.Remove(behAgent);
                }
            }
        }
    }

    #endregion
    
    protected Node BuildTreeRoot() {
        originalLocation = agent.transform.position;
        Func<bool> act = () => false;
        Node tree = new Sequence(
                            g_Agent.NPCBehavior_OrientTowards(FirstOrientation.transform.position),
                            g_Agent.NPCBehavior_LookAt(secondAgent.transform, true),
                            new LeafWait(2000),
                            g_Agent.NPCBehavior_LookAt(null, false),
                            new LeafWait(2000),
                            g_Agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.DISSAPOINTMENT),
                            g_Agent.ApproachAndWait(targetLocation, false),
                            g_Agent.NPCBehavior_OrientTowards(secondAgent.transform.position),
                            new SequenceParallel(
                                g_Agent.NPCBehavior_DoGesture(GESTURE_CODE.GREET_AT_DISTANCE),
                                g_AgentB.NPCBehavior_GoTo(g_Agent.transform,false)
                                ),
                            new SequenceParallel(
                                    g_Agent.NPCBehavior_LookAt(g_AgentB.transform,true),
                                    g_AgentB.NPCBehavior_LookAt(g_Agent.transform, true)
                                ),
                            g_Agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG),
                            g_AgentB.NPCBehavior_DoTimedGesture(GESTURE_CODE.HURRAY),
                            g_AgentB.NPCBehavior_DoTimedGesture(GESTURE_CODE.TALK_LONG)
                        );
        return tree;
    }
}