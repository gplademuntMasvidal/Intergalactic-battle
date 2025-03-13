using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "DroneBombFSM", menuName = "Finite State Machines/DroneBombFSM", order = 1)]
public class DroneBombFSM : FiniteStateMachine
{
    private DroneBlackboard m_blackboard;

    public override void OnEnter()
    {
        m_blackboard = GetComponent<DroneBlackboard>();
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {

        FiniteStateMachine guard = ScriptableObject.CreateInstance<PatrolFSM>();
        guard.Name = "GUARD";

        FiniteStateMachine combat = ScriptableObject.CreateInstance<DroneCombatFSM>();
        combat.Name = "CombatMode";

        FiniteStateMachine emergencyState = ScriptableObject.CreateInstance<EmergencyFSM>();
        emergencyState.Name = "Emergency State";

        State Death = new State("Death",
            () =>
            {
                m_blackboard.m_audioSource.Play();
                GetComponent<SteeringContext>().maxSpeed = 0.0f;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<CapsuleCollider2D>().enabled = false;
                GameObject fireEffect = Instantiate(m_blackboard.m_fireEffectPrefab, transform.position, Quaternion.identity, this.gameObject.transform);
                m_blackboard.m_render.SetActive(false);
                Destroy(gameObject, m_blackboard.m_destructionTime);
                GameManager.m_instance.m_passLevel -= 1;
            },
            () => { },
            () => { }
            );

        // ---------------------------------------------------
        Transition playerDetected = new Transition("Player Detected",
            () => { return SensingUtils.FindInstanceWithinRadius(gameObject, "Player", m_blackboard.m_droneDetectionRadius); },
            () => { }
        );

        Transition playerLost = new Transition("Player Lost",
            () =>
            { //return SensingUtils.DistanceToTarget(gameObject, blackboard.player) < blackboard.droneDetectionRadius;
                return !SensingUtils.FindInstanceWithinRadius(gameObject, "Player", m_blackboard.m_droneDetectionRadius);
            },
            () => { }
        );

        Transition enterEmergencyMode = new Transition("Enter Emergency Mode",
            () => { return m_blackboard.m_lives == 1; },
            () => { }
        );

        Transition DeathTransition = new Transition("DeathTransition",
            () => { return m_blackboard.m_lives <= 0; }
            );

        // ----------------------------------------------
        AddStates(guard, combat, emergencyState, Death);

        AddTransition(guard, playerDetected, combat);
        AddTransition(combat, playerLost, guard);
        AddTransition(combat, enterEmergencyMode, emergencyState);
        AddTransition(guard, DeathTransition, Death);
        AddTransition(combat, DeathTransition, Death);
        AddTransition(emergencyState, DeathTransition, Death);

        initialState = guard;
    }
}