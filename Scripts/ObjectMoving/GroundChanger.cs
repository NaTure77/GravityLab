using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChanger : MonoBehaviour {

    public static IEnumerator ChangeGroundOnJump(Transform playerObj, Transform groundT,Vector3 normal, Vector3 destination, System.Action Jump)
    {
        StateManager.isFlying = true;

        //Jump and wait Until isGround == false
        MoveController.instance.resetVelocity(Vector3.zero);
        if (!StateManager.isJumping) Jump();
        yield return new WaitWhile(() => StateManager.isGrounded);
        StateManager.currentGround = groundT.gameObject;

        //Set 
        float angle = Vector3.Angle(playerObj.up.normalized, normal.normalized);
        Vector3 outerProduct = Vector3.Cross(playerObj.up.normalized, normal.normalized);

        Vector3 fromPosition = playerObj.position;
        Quaternion fromRotation = playerObj.rotation;
        Quaternion toRotation = Quaternion.AngleAxis(angle, outerProduct) * fromRotation;

        //Debug.DrawRay(Vector3.zero, playerObj.up * 3, Color.green, 3);
        //Debug.DrawRay(Vector3.zero, groundT.transform.up * 3, Color.red,3);
        //Debug.DrawRay(Vector3.zero, Vector3.Cross(playerObj.up, groundT.up), Color.blue,3);

        float currentDistance = Vector3.Distance(playerObj.position, destination);
        float totalTime_D = 1 + Vector3.Distance(playerObj.position, destination) * 0.01f;
        float fromToRate_D = 0;
        //Debug.Log("FirstRotateStart");

        while(totalTime_D - fromToRate_D * totalTime_D > 0.6f && StateManager.currentGround == groundT.gameObject)
        {
            currentDistance = Vector3.Distance(playerObj.position, destination);
            playerObj.position = Vector3.LerpUnclamped(fromPosition, destination, fromToRate_D);
            fromToRate_D += Time.deltaTime / totalTime_D;
            yield return null;
        }
        if(StateManager.currentGround == groundT.gameObject)
        {
            yield return new WaitUntil(() => MoveController.instance.animator.GetFloat("JumpSpeed") == 0);
            MoveController.instance.animator.SetFloat("JumpSpeed", 1);
            float fromToRate_R = 0;
            float totalTime_R = totalTime_D - fromToRate_D * totalTime_D;
            while (fromToRate_D <= 1)
            {
                //Debug.DrawRay(Vector3.zero, playerObj.up, Color.black, 5);
                //Debug.DrawRay(Vector3.zero, playerObj.forward, Color.cyan, 5);


                playerObj.rotation = Quaternion.LerpUnclamped(fromRotation, toRotation, fromToRate_R);
                playerObj.position = Vector3.LerpUnclamped(fromPosition, destination, fromToRate_D);
                fromToRate_D += Time.deltaTime / totalTime_D;
                fromToRate_R += Time.deltaTime / totalTime_R;
                yield return null;
            }
            //last Looping
            playerObj.position = destination;
            playerObj.rotation = toRotation;

            MoveController.instance.resetVelocity(Vector3.zero);
            MoveController.instance.addGravityForce();
            StateManager.isFlying = false;
            Debug.Log("FlyFin");
        }
       
    }
    public static IEnumerator ChangeGroundOnWalk(Transform playerObj, Transform groundT,Vector3 normal)
    {
        StateManager.isGroundChanging = true;
        StateManager.currentGround = groundT.gameObject;

        float angle = Vector3.Angle(playerObj.up.normalized, normal.normalized);
        Vector3 outerProduct = Vector3.Cross(playerObj.up.normalized, normal.normalized);
        Quaternion fromRotation = playerObj.rotation;
        Quaternion toRotation = Quaternion.AngleAxis(angle, outerProduct) * fromRotation;
        float fromToRate = 0;
        float totalTime = 0.5f;

        while (fromToRate < 1 && StateManager.currentGround == (groundT.gameObject))
        {
            playerObj.rotation = Quaternion.LerpUnclamped(fromRotation, toRotation, fromToRate);
            fromToRate += Time.deltaTime / totalTime;
            yield return null;
        }
        if (StateManager.currentGround == (groundT.gameObject))
        {
            playerObj.rotation = toRotation;
            StateManager.isGroundChanging = false;
        }
    }
}
