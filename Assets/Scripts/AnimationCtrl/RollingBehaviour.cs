using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBehaviour : StateMachineBehaviour{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateManager.successLanding = false;
        StateManager.isStanding = true;
        StateManager.isMoveable = true;
        StageUI.instance.SetSuccessMessage();
        StageUI.instance.ShowSecMessage();
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateManager.isFlyable = true;
    }

}
