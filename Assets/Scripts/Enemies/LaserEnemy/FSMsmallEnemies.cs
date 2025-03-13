using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSMsmallEne", menuName = "Finite State Machines/FSMsmallEne", order = 1)]
public class FSMsmallEne : FiniteStateMachine
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
        

        //m_blackboardEnemies.m_start = true;
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        FiniteStateMachine SmallEnemiesBehaviour = ScriptableObject.CreateInstance<FSMEnemieP1>();
        //FEED.Name = "FEED";

         State Dead = new State("Dead",
           () =>{m_enemieController.Dead(); },
           () =>{},
           () =>{}
       );


        Transition EnemyIsDead = new Transition("EnemyIsDead",
           () => { return m_blackboardEnemies.m_smallEnemiesLife == 0; },
           () => { }
       );

        AddStates(SmallEnemiesBehaviour, Dead);

        AddTransition(SmallEnemiesBehaviour, EnemyIsDead, Dead);
        
        initialState = SmallEnemiesBehaviour;


    }
}
