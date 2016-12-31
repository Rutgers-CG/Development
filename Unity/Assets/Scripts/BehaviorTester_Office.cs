using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using System;
using NPC;

public class BehaviorTester_Office : MonoBehaviour {

    private static string SPAWN_POINT_TAG = "Spawn_Point";
    private static string ACTOR_STREET_TAG = "Actor_Street";
    private static string ACTOR_TAG = "Actor";

    static int g_LastSpawPoint = 0;

    public int Maximum_NPCs = 5;
    private int g_CurrentNPCs = 0;

    public float Spawn_Probability = 0.05f;
    public Transform targetLocation;
    public Vector3 originalLocation;
    private NPCBehavior g_Agent, g_AgentB;
    public GameObject agent;
    public GameObject secondAgent;
    public GameObject FirstOrientation;

    [SerializeField]
    public bool Enabled = false;

    public GameObject[] NPCStreet_Agents;
    GameObject[] g_SpawningPoints;
    HashSet<NPCBehavior> g_ActiveAgents;
    HashSet<NPCBehavior> g_InstantiatedBehaviorAgents;

    Dictionary<GameObject, NPCBehavior> g_NPCBehaviorActors;

    private BehaviorAgent behaviorAgent;

    #region Unity_Methods
    void Start() {

        g_InstantiatedBehaviorAgents = new HashSet<NPCBehavior>();
        g_SpawningPoints = GameObject.FindGameObjectsWithTag(SPAWN_POINT_TAG);

        if(Enabled) {
            /* Just for testing */
            g_ActiveAgents = new HashSet<NPCBehavior>();
            g_NPCBehaviorActors = new Dictionary<GameObject, NPCBehavior>();
            g_Agent = agent.GetComponent<NPCBehavior>();
            g_AgentB = secondAgent.GetComponent<NPCBehavior>();
            BehaviorEvent behEvent = new BehaviorEvent(doEvent => this.BuildTreeRoot(), new IHasBehaviorObject[] { (IHasBehaviorObject) g_Agent });
            behEvent.StartEvent(1f);
            /* ---------------- */
        }

    }

    void FixedUpdate() {
        if(Enabled) {
            //foreach (NPCBehavior b in g_InstantiatedBehaviorAgents) {
            //    Transform targetLoc = g_SpawningPoints[g_LastSpawPoint].transform;
            //    g_LastSpawPoint = (g_LastSpawPoint + 2) % g_SpawningPoints.Length;
            //    BehaviorAgent bAgent = new BehaviorAgent(ApproachAndWait(b, targetLoc));

            //    if(b.Behavior.CurrentEvent == null) {
            //        IHasBehaviorObject[] agents = { b };
            //        BehaviorEvent e = new BehaviorEvent(doEvent => ApproachAndWait(b, targetLoc), agents);
            //        e.StartEvent(1.0f);
            //    }

            //}
            // SpawnPedestrian();   
        }
    }

    #endregion

    private void SpawnPedestrian() {
        //if(UnityEngine.Random.value < Spawn_Probability && g_CurrentNPCs < Maximum_NPCs) {
        //    int val = (int)(UnityEngine.Random.value * (NPCStreet_Agents.Length - 1));
        //    g_CurrentNPCs++;
        //    NPCBehavior agent = NPCStreet_Agents[val].GetComponent<NPCBehavior>();
        //    Transform point = g_SpawningPoints[g_LastSpawPoint].transform;
        //    g_LastSpawPoint = (g_LastSpawPoint + 1) % g_SpawningPoints.Length;
        //    Instantiate(agent, point.position, point.rotation);
        //    g_InstantiatedBehaviorAgents.Add(agent);
        //}
    }

    protected Node ApproachAndWait(NPCBehavior agent, Transform target) {
        return new Sequence(agent.NPCBehavior_GoTo(target, true), new LeafWait(1000));
    }

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
                            ApproachAndWait(g_Agent,targetLocation),
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