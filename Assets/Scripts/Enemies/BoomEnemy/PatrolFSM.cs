using FSMs;
using Steerings;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolFSM", menuName = "Finite State Machines/PatrolFSM", order = 1)]
public class PatrolFSM : FiniteStateMachine
{
    private WanderAroundPlusAvoid m_wanderAround;
    private SteeringContext m_steeringContext;
    private DroneBlackboard m_blackboard;

    private float m_elapsedTime = 0.0f;

    public override void OnEnter()
    {
        m_blackboard = GetComponent<DroneBlackboard>();
        m_steeringContext = GetComponent<SteeringContext>();
        m_wanderAround = GetComponent<WanderAroundPlusAvoid>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        m_steeringContext.seekWeight = m_blackboard.m_initialSeekWeight;
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
         */
        State goingA = new State("GOING A",
            () => { m_elapsedTime = 0f; m_wanderAround.attractor = m_blackboard.m_pointA; m_wanderAround.enabled = true; }, // write on enter logic inside {}
            () => { m_elapsedTime += Time.deltaTime; }, // write in state logic inside {}
            () => { m_wanderAround.enabled = false; }  // write on exit logic inisde {}  
        );

        State goingB = new State("GOING B",
            () => { m_elapsedTime = 0f; m_wanderAround.attractor = m_blackboard.m_pointB; m_wanderAround.enabled = true; },
            () => { m_elapsedTime += Time.deltaTime; },
            () => { m_wanderAround.enabled = false; }
        );



        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        */
        Transition locationAReached = new Transition("LocationAReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboard.m_pointA) < m_blackboard.m_locationReachedRadius; }, // write the condition checkeing code in {}
            () => { m_steeringContext.seekWeight = m_blackboard.m_initialSeekWeight; }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition locationBReached = new Transition("LocationBReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboard.m_pointB) < m_blackboard.m_locationReachedRadius; },
            () => { m_steeringContext.seekWeight = m_blackboard.m_initialSeekWeight; }
        );

        Transition TimeOut = new Transition("TimeOut",
            () => { return m_elapsedTime > m_blackboard.m_intervalBetweenTimeouts; },
            () => { m_steeringContext.seekWeight += m_blackboard.m_seekIncrement; }
        );

        Transition Recall = new Transition("Recall",
            () => { return m_wanderAround.attractor == null; }
            );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
         */ 
        AddStates(goingA,goingB);

        AddTransition(goingA, locationAReached, goingB);
        AddTransition(goingA, TimeOut, goingA);
        AddTransition(goingB, locationBReached, goingA);
        AddTransition(goingB, TimeOut, goingB);
        AddTransition(goingA, Recall, goingA);


        /* STAGE 4: set the initial state
         
         */
        initialState = goingA; 


    }
}
