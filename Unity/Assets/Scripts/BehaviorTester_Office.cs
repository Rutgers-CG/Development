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

    Dictionary<GameObject, NPCBehavior> g_NPCBehaviorActors;

    private BehaviorAgent behaviorAgent;

    #region Unity_Methods
    void Start() {

        if(Enabled) {
            /* Just for testing */
            g_ActiveAgents = new HashSet<NPCBehavior>();
            g_NPCBehaviorActors = new Dictionary<GameObject, NPCBehavior>();
            g_Agent = agent.GetComponent<NPCBehavior>();
            g_AgentB = secondAgent.GetComponent<NPCBehavior>();
            behaviorAgent = new BehaviorAgent(this.BuildTreeRoot());
            BehaviorManager.Instance.Register(behaviorAgent);
            behaviorAgent.StartBehavior();
            /* ---------------- */
        }

        g_SpawningPoints = GameObject.FindGameObjectsWithTag(SPAWN_POINT_TAG);
        Debug.Log("Behavior Tester - " + NPCStreet_Agents.Length + " actors found");
        Debug.Log("Behavior Spawning Points - " + g_SpawningPoints.Length + " spawn points found");
    }

    void FixedUpdate() {
        if(Enabled) {
            SpawnPedestrian();
        }
    }

    #endregion

    private void SpawnPedestrian() {
        if(UnityEngine.Random.value < Spawn_Probability) {
            int val = (int)(UnityEngine.Random.value * (NPCStreet_Agents.Length - 1));
            NPCBehavior agent = NPCStreet_Agents[val].GetComponent<NPCBehavior>();
            Transform p1 = g_SpawningPoints[g_LastSpawPoint].transform;
            g_LastSpawPoint = (g_LastSpawPoint + 2) % g_SpawningPoints.Length;
            Transform p2 = g_SpawningPoints[g_LastSpawPoint].transform;
            g_LastSpawPoint = (g_LastSpawPoint + 2) % g_SpawningPoints.Length;
            Instantiate(agent.gameObject, p1.position, p1.rotation);
            agent.GetComponent<NPCController>().Debug(agent.name + " spawned at " + p1.position);
            BehaviorAgent behaviorAgent = new BehaviorAgent(ApproachAndWait(p2));
            BehaviorManager.Instance.Register(behaviorAgent);
            behaviorAgent.StartBehavior();
        }
    }

    protected Node ApproachAndWait(Transform target) {
        return new Sequence(g_Agent.NPCBehavior_GoTo(target, true), new LeafWait(1000));
    }

    protected Node BuildTreeRoot() {
        originalLocation = agent.transform.position;
        Func<bool> act = () => (Vector3.Distance(originalLocation, targetLocation.position) > 5);
        Node goTo = new Sequence(
                            g_Agent.NPCBehavior_OrientTowards(FirstOrientation.transform.position),
                            g_Agent.NPCBehavior_LookAt(secondAgent.transform, true),
                            new LeafWait(2000),
                            g_Agent.NPCBehavior_LookAt(null, false),
                            new LeafWait(2000),
                            g_Agent.NPCBehavior_DoTimedGesture(GESTURE_CODE.DISSAPOINTMENT),
                            ApproachAndWait(targetLocation),
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
        Node trigger = new DecoratorLoop(new LeafAssert(act));
        Node root = goTo;
        return root;
    }
}