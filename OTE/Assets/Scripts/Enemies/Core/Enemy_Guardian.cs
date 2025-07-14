using UnityEngine;

public class Enemy_Guardian : MonoBehaviour
{
    private enum State { Patrolling, Chasing, Attacking }
    private State currentState;

    // УДАЛИЛИ loseSightRange. Больше никаких лишних параметров!

    [Header("Component References")]
    [SerializeField] private AI_Patrol_Ground patrolModule;
    [SerializeField] private AI_Chase_Ground chaseModule;
    [SerializeField] private AI_Attack_Melee attackModule;
    [SerializeField] private AI_Detection_Sight sightModule;

    private Transform playerTarget;

    private void Start()
    {
        SwitchState(State.Patrolling);
    }

    private void Update()
    {
        // Мы просто доверяем модулю зрения. Он сам решит, видит он цель или нет.
        playerTarget = sightModule.DetectedTarget;

        switch (currentState)
        {
            case State.Patrolling:
                // Если модуль зрения говорит, что цель есть...
                if (playerTarget != null)
                {
                    SwitchState(State.Chasing);
                }
                break;

            case State.Chasing:
                // Если модуль зрения говорит, что цели больше нет...
                if (playerTarget == null)
                {
                    SwitchState(State.Patrolling);
                }
                else if (attackModule.IsTargetInRange(playerTarget))
                {
                    SwitchState(State.Attacking);
                }
                break;

            case State.Attacking:
                // Если цели больше нет ИЛИ она вышла из зоны атаки...
                if (playerTarget == null || !attackModule.IsTargetInRange(playerTarget))
                {
                    // ...возвращаемся к преследованию.
                    // Если цели нет, то в следующем кадре мы сразу перейдем из Chasing в Patrolling.
                    SwitchState(State.Chasing);
                }
                else
                {
                    attackModule.PerformAttack();
                }
                break;
        }
    }

    private void SwitchState(State newState)
    {
        currentState = newState;

        patrolModule.enabled = (newState == State.Patrolling);
        chaseModule.enabled = (newState == State.Chasing);
        
        if (chaseModule.enabled)
        {
            chaseModule.target = playerTarget;
        }
    }
}