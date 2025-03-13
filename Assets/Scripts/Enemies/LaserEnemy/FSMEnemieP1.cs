using FSMs;
using Steerings;
using UnityEngine;

[CreateAssetMenu(fileName = "FSMEnemieP1", menuName = "Finite State Machines/FSMEnemieP1", order = 1)]
public class FSMEnemieP1 : FiniteStateMachine
{
    private BlackboardEnemies m_blackboardEnemies;
    private FlockingAroundP1 m_flockingAroundP1;
    private KeepPositionP1 m_keepPositionP1;
    private SteeringContext m_steeringContext;
    private EnemieController m_enemieController;

    private float m_elapsedTime;

    public override void OnEnter()
    {
        m_blackboardEnemies = GetComponent<BlackboardEnemies>();
        m_flockingAroundP1 = GetComponent<FlockingAroundP1>();
        m_keepPositionP1 = GetComponent<KeepPositionP1>();
        m_steeringContext = GetComponent<SteeringContext>();
        m_enemieController = GetComponent<EnemieController>();

        //m_originalSpeed = GetComponent<SteeringContext>().maxSpeed;
        //m_originalRepulsionThreshold = GetComponent<SteeringContext>().repulsionThreshold;
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        State FlockingToPlayer = new State("FlockingToPlayer",
            () =>
            {
                m_elapsedTime = 0;
                m_steeringContext.cohesionThreshold = m_blackboardEnemies.m_originalCohesionThreshold;
               // m_steeringContext.seekWeight += 0.2f;
                m_flockingAroundP1.enabled = true;
            },
            () =>
            {
                m_elapsedTime += Time.deltaTime;
                //m_steeringContext.repulsionThreshold = Mathf.Lerp(0, m_blackboardEnemies.m_originalRepulsionThreshold, m_elapsedTime / 5); 
                m_steeringContext.repulsionThreshold = m_blackboardEnemies.m_originalRepulsionThreshold;

            },
            () => { }
        );

        State ApproachingPlayer = new State("AproachingPlayer",
            () =>
            {
                m_steeringContext.repulsionThreshold = 0;
                m_steeringContext.cohesionThreshold = 0;
                // m_steeringContext.maxSpeed = m_blackboardEnemies.m_originalSpeed * 2;
            },
            () => { },
            () =>
            {
                m_flockingAroundP1.enabled = false;
            }
        );



        State KeepPositionWithThePlayer = new State("KeepPositionWithThePlayer",
            () =>
            {
                m_steeringContext.maxSpeed = m_blackboardEnemies.m_originalSpeed;

                m_keepPositionP1.enabled = true;
                m_keepPositionP1.m_target = m_blackboardEnemies.m_target;
                m_keepPositionP1.m_formationRadius = m_blackboardEnemies.m_formationRadius;
                m_keepPositionP1.m_formationIndex = m_blackboardEnemies.m_formationIndex;
                m_keepPositionP1.m_totalEnemies = m_blackboardEnemies.m_totalEnemies;
                m_elapsedTime = 0;

            },
            () =>
            {
                m_elapsedTime += Time.deltaTime;
            },
            () => {
                m_keepPositionP1.enabled = false;
            }
        );

        State AttackingPlayer = new State("AttackingPlayer",
            () =>
            {
                m_keepPositionP1.enabled = true;
                m_elapsedTime = 0;
                m_blackboardEnemies.m_timer = 0;
            },
            () =>
            {
                m_enemieController.ShootPlayer(m_blackboardEnemies.m_target);
                m_elapsedTime += Time.deltaTime;
            },
            () =>
            {
                m_enemieController.m_lineRenderer.enabled = false;
                m_keepPositionP1.enabled = false;

            }
        );





        //
        Transition AlmostCloseToPlayer = new Transition("AlmostCloseToPlayer",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboardEnemies.m_target) <= m_blackboardEnemies.m_closeToPlayer + 30; },
            () => { }
        );

        Transition CloseToPlayer = new Transition("CloseToPlayer",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboardEnemies.m_target) <= m_blackboardEnemies.m_closeToPlayer; },
            () => { }
        );

        Transition FarFromPlayer = new Transition("FarFromPlayer",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboardEnemies.m_target) > m_blackboardEnemies.m_closeToPlayer + 20; },
            () => { }
        );

        Transition StartAttackingPlayer = new Transition("StartAttackingPlayer",
           () => { return m_elapsedTime >= 6; },
           () => { m_enemieController.m_makingDamage = true; }
       );




        //

        AddStates(FlockingToPlayer, ApproachingPlayer, KeepPositionWithThePlayer, AttackingPlayer);

        AddTransition(FlockingToPlayer, AlmostCloseToPlayer, ApproachingPlayer);
        AddTransition(ApproachingPlayer, FarFromPlayer, FlockingToPlayer);
        AddTransition(ApproachingPlayer, CloseToPlayer, KeepPositionWithThePlayer);
        AddTransition(KeepPositionWithThePlayer, FarFromPlayer, FlockingToPlayer);
        AddTransition(KeepPositionWithThePlayer, StartAttackingPlayer, AttackingPlayer);
        AddTransition(AttackingPlayer, StartAttackingPlayer, AttackingPlayer);
        AddTransition(AttackingPlayer, FarFromPlayer, FlockingToPlayer);


        initialState = FlockingToPlayer;

    }
}
