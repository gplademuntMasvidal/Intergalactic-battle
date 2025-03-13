using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Player", menuName = "Finite State Machines/FSM_Player", order = 1)]
public class FSM_Player : FiniteStateMachine
{
    private PlayerBlackBoard m_blackBoard;
    private SteeringContext m_steeringContext;
    private GoWhereYouLook m_goWhereYouLook;
    private KBDRotate m_KBDRotate;

    public override void OnEnter()
    {
        m_steeringContext = GetComponent<SteeringContext>();
        m_blackBoard = GetComponent<PlayerBlackBoard>();
        m_goWhereYouLook = GetComponent<GoWhereYouLook>();
        m_KBDRotate = GetComponent<KBDRotate>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {

        State Alive = new State("Alive",
            () => { },
            () =>
            {
                if (!GameManager.m_instance.m_isPaused)
                {
                    if (Input.GetKey(KeyCode.W))
                    {
                        m_steeringContext.maxSpeed += m_blackBoard.m_addMaxSpeed * Time.deltaTime;
                        m_steeringContext.maxSpeed = Mathf.Clamp(m_steeringContext.maxSpeed, m_blackBoard.m_minSpeed, m_blackBoard.m_maxSpeed);
                    }
                    else
                    {
                        if (m_steeringContext.maxSpeed > m_blackBoard.m_minSpeed)
                        {
                            m_steeringContext.maxSpeed -= m_blackBoard.m_addMaxSpeed * Time.deltaTime;
                        }
                    }

                }
                m_blackBoard.m_takingDamage = false;
            },
            () => { }
        );

        State Death = new State("Death",
            () =>
            {
                m_steeringContext.maxSpeed = 0.0f;
                gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                m_goWhereYouLook.enabled = false;
                m_KBDRotate.enabled = false;
                GameManager.m_instance.GameOver();
            },
            () => { },
            () => { }
        );


        //------------------------------------------------------
        Transition DeathTarnsition = new Transition("DeathTarnsition",
            () => { return m_blackBoard.m_HUDHealth.value <= 1; },
            () => { }
        );

        Transition DamageTransition = new Transition("DamageTransition",
            () => { return m_blackBoard.m_takingDamage; },
            () => { m_blackBoard.m_currentHealt -= m_blackBoard.m_damage; }
        );

        //---------------------------------------------------------
        AddStates(Alive, Death);

        AddTransition(Alive, DeathTarnsition, Death);
        AddTransition(Alive, DamageTransition, Alive);

        //--------------------------------------------------------
        initialState = Alive;

    }
}
