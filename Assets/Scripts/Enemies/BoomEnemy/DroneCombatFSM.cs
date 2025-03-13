using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "DroneCombatFSM", menuName = "Finite State Machines/DroneCombatFSM", order = 1)]
public class DroneCombatFSM : FiniteStateMachine
{
    private Seek m_seek;
    private Arrive m_arrive;
    private DroneBlackboard m_blackboard;

    public override void OnEnter()
    {
        m_blackboard = GetComponent<DroneBlackboard>();
        m_seek = GetComponent<Seek>();
        m_arrive = GetComponent<Arrive>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         */

        State followingPlayer = new State("Following Player",
            () => { m_seek.target = m_blackboard.m_player; m_seek.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { m_seek.enabled = false; }  // write on exit logic inisde {}  
        );

        State attackingPlayer = new State("Attacking Player",
            () =>
            {
                m_arrive.target = m_blackboard.m_player;
                m_arrive.enabled = true;
                m_blackboard.m_audioSource.Play();
                GetComponent<SteeringContext>().maxSpeed = 0.0f;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<CapsuleCollider2D>().enabled = false;
                GameObject fireEffect = Instantiate(m_blackboard.m_fireEffectPrefab, transform.position, Quaternion.identity, this.gameObject.transform);
                m_blackboard.m_render.SetActive(false);
                Destroy(gameObject, m_blackboard.m_destructionTime);
                m_blackboard.m_player.GetComponent<PlayerBlackBoard>().Damage(m_blackboard.m_damageMaked);
                GameManager.m_instance.m_passLevel -= 1;
            },
            () => { },
            () => { m_seek.enabled = false; }
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
        */

        Transition playerOnRange = new Transition("Player On Range",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboard.m_player) < m_blackboard.m_droneRangeRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
         */

        AddStates(followingPlayer, attackingPlayer);

        AddTransition(followingPlayer, playerOnRange, attackingPlayer);



        /* STAGE 4: set the initial state
         */

        initialState = followingPlayer;


    }
}
