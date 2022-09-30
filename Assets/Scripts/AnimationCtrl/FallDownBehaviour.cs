using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDownBehaviour : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        {
            Debug.Log("successLanding: " + StateManager.successLanding);
            Debug.Log("isGrounded: " + StateManager.isGrounded);
            Debug.Log("isColliderGrounded: " + StateManager.isColliderGrounded);
        }
        StateManager.successLanding = false;
        StageUI.instance.SetFailedMessage();
        StageUI.instance.ShowSecMessage();
    }
}
