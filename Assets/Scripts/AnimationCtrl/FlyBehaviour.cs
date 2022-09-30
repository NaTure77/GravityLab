using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBehaviour : StateMachineBehaviour {

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateManager.isStanding = false;
        StateManager.isMoveable = false;
	}
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MoveController.instance.resetVelocity(Vector3.zero);
	}

}
