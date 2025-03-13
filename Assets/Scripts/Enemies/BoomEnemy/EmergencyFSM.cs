using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "EmergencyFSM", menuName = "Finite State Machines/EmergencyFSM", order = 1)]
public class EmergencyFSM : FiniteStateMachine
{
    private SteeringContext m_steeringContext;
    private DroneBlackboard m_blackboard;
    private Seek m_seek;
    private Arrive m_arrive;
    private Evade m_evade;
    private float m_normalSpeed;

    public override void OnEnter()
    {
        m_blackboard = GetComponent<DroneBlackboard>();
        m_steeringContext = GetComponent<SteeringContext>();
        m_arrive = GetComponent<Arrive>();
        m_evade = GetComponent<Evade>();
        m_normalSpeed = m_steeringContext.maxSpeed;
        m_steeringContext.maxSpeed *= 1.5f;
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        m_steeringContext.maxSpeed = m_normalSpeed;
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         */

        State aggressiveAttack = new State("Agressive Attack",
            () => { m_arrive.target = m_blackboard.m_player; m_arrive.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { m_arrive.enabled = false; }  // write on exit logic inisde {}  
        );

        State evasiveEscape = new State("Evasive Escape",
            () => { m_evade.target = m_blackboard.m_player; m_evade.enabled = true; },
            () => { },
            () => { m_evade.enabled = false; }
        );

        State exploding = new State("Exploding",
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
            () => { }
        );




        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
        */

        Transition goEvassive = new Transition("Go Evassive",
            () => { return m_blackboard.m_shouldEscape; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition backToCombat = new Transition("Back To Combat",
            () => { return !m_blackboard.m_shouldEscape; },
            () => { }
        );
        Transition explodeIfPlayerClose = new Transition("Explode If Player Close",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboard.m_player) < m_blackboard.m_droneRangeRadius; },
            () => { }
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
         */

        AddStates(aggressiveAttack, evasiveEscape, exploding);

        AddTransition(aggressiveAttack, goEvassive, evasiveEscape);
        AddTransition(evasiveEscape, backToCombat, aggressiveAttack);
        AddTransition(aggressiveAttack, explodeIfPlayerClose, exploding);
        AddTransition(evasiveEscape, explodeIfPlayerClose, exploding);



        /* STAGE 4: set the initial state
         */

        initialState = aggressiveAttack;

    }
}
