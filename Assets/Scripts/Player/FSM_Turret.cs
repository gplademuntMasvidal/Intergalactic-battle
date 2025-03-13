using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Turret", menuName = "Finite State Machines/FSM_Turret", order = 1)]
public class FSM_Turret : FiniteStateMachine
{
    private BlackboardTurret m_blackBoardTurret;
    private SteeringContext m_steeringContext;
    private FaceMouse m_faceMouse;
    private AudioSource m_audioSource;

    public override void OnEnter()
    {
        m_blackBoardTurret = GetComponent<BlackboardTurret>();
        m_steeringContext = GetComponent<SteeringContext>();
        m_faceMouse = GetComponent<FaceMouse>();
        m_audioSource = GetComponent<AudioSource>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        State NotSooting = new State("NotSooting",
           () =>
           {
               m_faceMouse.enabled = true;
           },
           () => { },
           () => { }
       );

        State Death = new State("Death",
            () =>
            {
                m_faceMouse.enabled = false;
            },
            () => { },
            () => { }
        );

        State ShootBullet = new State("Shooting",
            () => { },
            () =>
            {
                if (m_blackBoardTurret.m_firePoint == null)
                {
                    Debug.LogError("FirePoint no está asignado. Asegúrate de asignarlo en el inspector.");
                    return;
                }


                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0;

                Vector3 direction = (mousePosition - m_blackBoardTurret.m_firePoint.position).normalized;


                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion bulletRotation = Quaternion.Euler(0, 0, angle);


                GameObject bullet = Instantiate(m_blackBoardTurret.m_bulletPrefab, m_blackBoardTurret.m_firePoint.position, bulletRotation);

                m_audioSource.clip = m_blackBoardTurret.m_shootSound;
                m_audioSource.Play();

                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

                if (bulletRb != null)
                {
                    bulletRb.velocity = direction * m_blackBoardTurret.m_bulletSpeed;
                }

                Destroy(bullet, m_blackBoardTurret.m_bulletLifeTime);

                m_blackBoardTurret.m_shoot = false;
            },
            () => { }
            );


        //------------------------------------------------------
        Transition DeathTarnsition = new Transition("DeathTarnsition",
            () => { return m_blackBoardTurret.m_HUDHealth.value <= 1; }
        );

        Transition Shooting = new Transition("ShootBullet",
            () => { return m_blackBoardTurret.m_shoot; }
            );

        Transition StopShooting = new Transition("StopShootin",
            () => { return !m_blackBoardTurret.m_shoot; }
        );

        //---------------------------------------------------------
        AddStates(NotSooting, Death, ShootBullet);

        AddTransition(NotSooting, DeathTarnsition, Death);
        AddTransition(NotSooting, Shooting, ShootBullet);
        AddTransition(ShootBullet, StopShooting, NotSooting);

        //--------------------------------------------------------
        initialState = NotSooting;

    }
}
