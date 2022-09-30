using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlySystem : MonoBehaviour {

    public float flyDuration = 5;
    public float flySpeed = 30;
    public bool isStageGame = false;
    public float AwarableDistance_MAX = 20;
    public float AwarableDistance_MIN = 1;
    public static FlySystem instance;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator Fly(Transform playerObj, Transform player, Vector3 dir)
    {
        StateManager.isFlying = true;
        StateManager.isFlyable = false;

        if ((StateManager.isGrounded || StateManager.isColliderGrounded) && !isStageGame)
        {
            StateManager.currentPosition = playerObj.transform.position;
            StateManager.currentRotation = playerObj.transform.rotation;
        }

        Quaternion camLocalRotation = CamCtrl.instance.transform.localRotation;

        StateManager.isJumping = true;
        MoveController.instance.resetVelocity(Vector3.zero);
        playerObj.GetComponent<Rigidbody>().AddForce(dir.normalized * flySpeed, ForceMode.Impulse);
        float groundedTime = 0;
        while ((StateManager.isGrounded || StateManager.isColliderGrounded) && groundedTime < 0.5f)
        {
            groundedTime += Time.deltaTime;
            yield return null;
        }
        //Debug.Log("Grounded" + StateManager.isGrounded);
        //Debug.Log("ColliderGrounded" + StateManager.isColliderGrounded);
        StateManager.isJumping = false;

        if (groundedTime < 0.5f)
        {
            MoveController.instance.resetVelocity(Vector3.zero);
            playerObj.GetComponent<Rigidbody>().AddForce(dir.normalized * flySpeed, ForceMode.Impulse);
            StartCoroutine(SetFlyAngle(playerObj, player, dir,camLocalRotation));
            StartCoroutine(CheckForward(player));
            float flyT = 0;
            while (!(StateManager.isGrounded || StateManager.isColliderGrounded) && flyT < flyDuration && StateManager.isFlying)
            //while(StateManager.isFlying)
            {

                //rgb.MovePosition(transform.position + forwardVector * flySpeed * Time.deltaTime);
                //transform.Translate(forwardVector.normalized * flySpeed * Time.deltaTime, Space.World);
                flyT += Time.deltaTime;
                yield return null;
            }
            if (flyT > flyDuration)
            //if(!(StateManager.isGrounded || StateManager.isColliderGrounded))
            {
                playerObj.position = StateManager.currentPosition;
                playerObj.rotation = StateManager.currentRotation;
                StateManager.successLanding = false;
            } 
        }
        MoveController.instance.resetVelocity(Vector3.zero);
        StateManager.isFloating = false;
        StateManager.isFlying = false;

    }

    public IEnumerator Floating(Transform playerObj)
    {
        StateManager.isFloating = true;
        StateManager.successLanding = true;//false;
        float t = 0;
        while (t < flyDuration && !(StateManager.isGrounded || StateManager.isColliderGrounded))
        {
            t += Time.deltaTime;
            yield return null;
        }
        MoveController.instance.resetVelocity(Vector3.zero);
        if (t > flyDuration)
        //if(!(StateManager.isGrounded || StateManager.isColliderGrounded))
        {
            playerObj.position = StateManager.currentPosition;
            playerObj.rotation = StateManager.currentRotation;
        }
        StateManager.successLanding = false;
        StateManager.isFloating = false;
        StateManager.isFlying = false;

    }
    IEnumerator SetFlyAngle(Transform playerObj, Transform player, Vector3 normal, Quaternion camLocalRot)
    {
        float angle = Vector3.Angle(playerObj.up.normalized, normal.normalized) - 90;
        Vector3 outerProduct = Vector3.Cross(playerObj.up.normalized, normal.normalized);
        Quaternion fromRot = playerObj.rotation;
        Quaternion toRot = Quaternion.AngleAxis(angle, outerProduct) * playerObj.rotation;

        Quaternion _fromRot = player.localRotation;
        Quaternion q = Quaternion.identity;
        Quaternion _toRot = Quaternion.identity;
        _toRot.eulerAngles = Vector3.up * (camLocalRot * q).eulerAngles.y;//= new Quaternion(0, (CamCtrl.instance.transform.localRotation * q).y, 0, 0);

        float endTime = 0.4f;
        float fromToRate = 0;
        while (fromToRate < 1 && !StateManager.isGroundChanging)
        {
            playerObj.rotation = Quaternion.LerpUnclamped(fromRot, toRot, fromToRate);
            player.localRotation = Quaternion.LerpUnclamped(_fromRot, _toRot, fromToRate);
            fromToRate += Time.deltaTime / endTime;
            yield return null;
        }
        if(!StateManager.isGroundChanging)
        {
            playerObj.rotation = toRot;
            player.localRotation = _toRot;
        }

    }
    IEnumerator FlyTimeCounter()
    {
        yield return new WaitForSecondsRealtime(flyDuration);
        StateManager.isFlying = false;
    }

    IEnumerator CheckForward(Transform player)
    {
        RaycastHit hit;
        bool failLanding = false;
        while (!(StateManager.isGrounded || StateManager.isColliderGrounded))
        {
            //Debug.DrawRay(player.position + player.rotation * Vector3.up * 2.5f, player.forward * AwarableDistance_MAX, Color.red);
            if (Physics.SphereCast(player.position + player.rotation * Vector3.up * 2.5f,2.5f, player.forward, out hit, AwarableDistance_MAX) && hit.distance >= AwarableDistance_MIN && hit.collider.CompareTag("Ground"))
            {
                StageUI.instance.EnableDistanceInfo();
                if (Input.GetKeyDown(KeyCode.LeftShift) && !failLanding)
                {
                    StateManager.successLanding = true;
                }
            }
            else
            {
                StageUI.instance.DisableDistanceInfo();
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    StateManager.successLanding = false;
                     failLanding = true;
                    //failLanding = false;
                    break;
                }
            }
            yield return null;
        }
    }

}
