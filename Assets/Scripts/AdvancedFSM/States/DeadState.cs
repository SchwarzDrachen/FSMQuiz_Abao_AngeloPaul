using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : FSMState
{
    private EnemyTankController controller;
    private Transform currentTarget;

    // Always define stateId in the constructor
    public DeadState(){
        stateId = StateID.Dead;
    }

    public override void RunState(Transform player, Transform agent){
        controller = agent.GetComponent<EnemyTankController>();
        if(controller == null){
            Debug.LogError("Make sure agent has EnemyTankController");
            return;
        }
        
        controller.Death();
    }

    public override void CheckTransitionRules(Transform player, Transform agent){
        if(controller == null){
            Debug.LogError("Make sure agent has EnemyTankController");
            return;
        }

        
    }
}
