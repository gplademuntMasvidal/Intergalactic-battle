using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSMEnemies2p1", menuName = "Finite State Machines/FSMEnemies2p1", order = 1)]
public class FSMEnemies2p1 : FiniteStateMachine
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
        m_blackboardEnemies.m_HUDHealth.maxValue = m_blackboardEnemies.m_bigEnemiesLife;
        m_blackboardEnemies.m_HUD.SetActive(true);

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        State Merging = new State("Merging",
           () =>
           {
               Debug.Log("estas diins");
               m_blackboardEnemies.m_isBigEnemy = true;
               m_enemieController.MergeAlternateEnemies();
           },
           () => { 
           },
           () => { /*m_seekP1.enabled = false;*/ m_blackboardEnemies.m_startFlocking = true; }
       );

        State FlockingToPlayer = new State("FlockingToPlayer",
            () =>
            {
                /*m_enemieController.MergeAlternateEnemies();
                m_blackboardEnemies.m_isBigEnemy = true;*/
                m_elapsedTime = 0;
                m_steeringContext.cohesionThreshold = m_blackboardEnemies.m_originalCohesionThreshold;
                //m_steeringContext.maxSpeed = m_blackboardEnemies.m_originalSpeed ;
                m_flockingAroundP1.enabled = true;
            },
            () =>
            {
                m_elapsedTime += Time.deltaTime;
                 m_steeringContext.repulsionThreshold = Mathf.Lerp(0, m_blackboardEnemies.m_originalRepulsionThreshold , m_elapsedTime / 5);

            },
            () => {  }
        );

        State ApproachingPlayer = new State("AproachingPlayer",
            () =>
            {
                

                m_steeringContext.repulsionThreshold = 0;
                m_steeringContext.cohesionThreshold = 0;
                // m_steeringContext.maxSpeed = m_blackboardEnemies.m_originalSpeed * 2;
                m_elapsedTime = 0;

            },
            () => { m_elapsedTime += Time.deltaTime; },
            () =>
            {
                m_flockingAroundP1.enabled = false;
                // m_flockingAroundOA.enabled = false;
            }
        );



        State KeepPositionWithThePlayer = new State("KeepPositionWithThePlayer",
            () =>
            {
                m_steeringContext.maxSpeed = m_blackboardEnemies.m_originalSpeed;

                m_keepPositionP1.enabled = true;
                m_keepPositionP1.m_target = m_blackboardEnemies.m_target;
                m_keepPositionP1.m_formationRadius = m_blackboardEnemies.m_formationRadius + 20;
                m_keepPositionP1.m_formationIndex = m_blackboardEnemies.m_formationIndex;
                m_keepPositionP1.m_totalEnemies = m_blackboardEnemies.m_totalEnemies;
                m_elapsedTime = 0;

            },
            () =>
            {
                m_elapsedTime += Time.deltaTime;
            },
            () => { m_keepPositionP1.enabled = false; }
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

        Transition StartFlocking = new Transition("StartFlocking",
            () => { return m_blackboardEnemies.m_startFlocking == true; }, 
            () => { }
        );

        Transition AlmostCloseToPlayer = new Transition("AlmostCloseToPlayer",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboardEnemies.m_target) <= m_blackboardEnemies.m_closeToPlayer + 50; },
            () => { }
        );

        Transition CloseToPlayer = new Transition("CloseToPlayer",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboardEnemies.m_target) <= m_blackboardEnemies.m_closeToPlayer + 30; },
            () => { }
        );

        Transition FarFromPlayer = new Transition("FarFromPlayer",
            () => { return SensingUtils.DistanceToTarget(gameObject, m_blackboardEnemies.m_target) > m_blackboardEnemies.m_closeToPlayer + 50; },
            () => { }
        );

        Transition StartAttackingPlayer = new Transition("StartAttackingPlayer",
           () => { return m_elapsedTime >= 3; },
           () => { m_enemieController.m_makingDamage = true; }
       );

        




        //

        AddStates(Merging, FlockingToPlayer, ApproachingPlayer, KeepPositionWithThePlayer, AttackingPlayer);

        AddTransition(Merging, StartFlocking, FlockingToPlayer);
        AddTransition(FlockingToPlayer, AlmostCloseToPlayer, ApproachingPlayer);
        AddTransition(ApproachingPlayer, FarFromPlayer, FlockingToPlayer);
        AddTransition(ApproachingPlayer, CloseToPlayer, KeepPositionWithThePlayer);
        AddTransition(KeepPositionWithThePlayer, FarFromPlayer, FlockingToPlayer);
        AddTransition(KeepPositionWithThePlayer, StartAttackingPlayer, AttackingPlayer);
        AddTransition(ApproachingPlayer, StartAttackingPlayer, AttackingPlayer);
        AddTransition(AttackingPlayer, StartAttackingPlayer, AttackingPlayer);
        AddTransition(AttackingPlayer, FarFromPlayer, FlockingToPlayer);

       


        initialState = Merging;

    }
}
