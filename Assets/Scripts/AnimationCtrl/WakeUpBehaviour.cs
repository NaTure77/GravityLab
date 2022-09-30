using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUpBehaviour : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        StateManager.isStanding = true;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateManager.isFlyable = true;
    }
}
