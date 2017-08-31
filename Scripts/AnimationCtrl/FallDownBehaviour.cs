using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDownBehaviour : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        StateManager.successLanding = false;
        StageUI.instance.SetFailedMessage();
        StageUI.instance.ShowSecMessage();
    }
}
