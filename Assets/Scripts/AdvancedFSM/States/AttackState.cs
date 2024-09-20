using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : FSMState
{
    private EnemyTankController controller;
    private Transform currentTarget;

    // Always define stateId in the constructor
    public AttackState(){
        stateId = StateID.Attack;
    }

    public override void RunState(Transform player, Transform agent){
        controller = agent.GetComponent<EnemyTankController>();
        if(controller == null){
            Debug.LogError("Make sure agent has EnemyTankController");
            return;
        }
        currentTarget = player;
        controller.AttackTarget(currentTarget);
    }

    public override void CheckTransitionRules(Transform player, Transform agent){
        if(controller == null){
            Debug.LogError("Make sure agent has EnemyTankController");
            return;
        }

        if(Vector3.Distance(agent.position, player.position) >= controller.ChaseDistance){
            controller.PerformTransition(TransitionID.LostPlayer);
        }

        if(controller.CurHealth <= 0.0f){
            controller.PerformTransition(TransitionID.NoHealth);
        }
    }
}
