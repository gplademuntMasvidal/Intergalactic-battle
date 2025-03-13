using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSMbigEnemies", menuName = "Finite State Machines/FSMbigEnemies", order = 1)]
public class FSMbigEnemies : FiniteStateMachine
{
    private BlackboardEnemies m_blackboardEnemies;
    private EnemieController m_enemieController;

    public override void OnEnter()
    {

        m_blackboardEnemies = GetComponent<BlackboardEnemies>();
        m_enemieController = GetComponent<EnemieController>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {

        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        FiniteStateMachine BigEnemiesBehaviour = ScriptableObject.CreateInstance<FSMEnemies2p1>();
        //FEED.Name = "FEED";

        State Dead = new State("Dead",
          () => { m_enemieController.Dead(); },
          () => { },
          () => { }
      );


        Transition EnemyIsDead = new Transition("EnemyIsDead",
           () => { return m_blackboardEnemies.m_bigEnemiesLife == 0; },
           () => { }
       );

        AddStates(BigEnemiesBehaviour, Dead);

        AddTransition(BigEnemiesBehaviour, EnemyIsDead, Dead);

        initialState = BigEnemiesBehaviour;


    }
}
