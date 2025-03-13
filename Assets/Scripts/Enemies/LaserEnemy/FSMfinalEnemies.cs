using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSMfinalEnemies", menuName = "Finite State Machines/FSMfinalEnemies", order = 1)]
public class FSMfinalEnemies : FiniteStateMachine
{

    public BlackboardEnemies m_blackboardEnemies;

    public override void OnEnter()
    {

        m_blackboardEnemies = GetComponent<BlackboardEnemies>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {

        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        FiniteStateMachine SmallEnemiesBehaviour = ScriptableObject.CreateInstance<FSMsmallEne>();

        FiniteStateMachine BigEnemiesBehaviour = ScriptableObject.CreateInstance<FSMbigEnemies>();


        Transition TurnIntoBigEnemies = new Transition("TurnIntoBigEnemies",
         () => {
             m_blackboardEnemies.m_timer += Time.deltaTime;

             return GuillemGroupManager.m_instance.members.Count <= 10 && GuillemGroupManager.m_instance.members.Count % 2 == 0 && m_blackboardEnemies.m_timer >= m_blackboardEnemies.m_timeToChange;
         },
         () => {
             m_blackboardEnemies.m_timer = 0f; 
         }
     );

        AddStates(SmallEnemiesBehaviour, BigEnemiesBehaviour);

        AddTransition(SmallEnemiesBehaviour, TurnIntoBigEnemies, BigEnemiesBehaviour);

        initialState = SmallEnemiesBehaviour;


    }
}
