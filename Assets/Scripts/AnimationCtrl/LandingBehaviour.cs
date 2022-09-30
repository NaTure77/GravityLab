using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingBehaviour : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateManager.successLanding = false;
        StateManager.isStanding = true;
        StageUI.instance.SetSuccessMessage();
        StageUI.instance.ShowSecMessage();

    }
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateManager.isMoveable = true;
        StateManager.isFlyable = true;
	}

}
