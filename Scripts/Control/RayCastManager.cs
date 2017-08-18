using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastManager : MonoBehaviour {


    public IEnumerator StartRay(GameObject player,Camera cam)
    {
        yield return new WaitUntil(() => StateManager.isPlayerMaked);
        StartCoroutine(CheckGround(player));
        StartCoroutine(awareWorld(cam));
    }
    IEnumerator CheckGround(GameObject player)
    {
        var endOfFrame = new WaitForEndOfFrame();
        while (true)
        {
            float dist = player.transform.lossyScale.y * 2;
            RaycastHit hit;
            while (true)
            {
                /*if (transform.parent != null)
                {
                    //MoveController.instance.resetVelocity(Vector3.zero);
                    yield return null;
                    continue;
                }*/
                // Debug.DrawRay(player.transform.position + player.transform.rotation * Vector3.up * dist, player.transform.rotation * Vector3.down * (dist + 0.5f), Color.red, Time.deltaTime);
                if (Physics.Raycast(player.transform.position + player.transform.rotation * Vector3.up * dist, player.transform.rotation * Vector3.down, out hit, dist + 0.5f))
                {

                    if (hit.collider.tag == "Ground" && !StateManager.isFlying)
                    {
                        if (Vector3.Angle(hit.normal.normalized, player.transform.up.normalized) % 360 != 0)
                        {
                            StartCoroutine(GroundChanger.ChangeGroundOnWalk(player.transform, hit.transform, hit.normal));
                        }
                        if (hit.collider.gameObject.Equals(StateManager.currentGround)) StateManager.isGrounded = true;
                        else if (!StateManager.isFlying && !StateManager.isGroundChanging)
                        {
                            StartCoroutine(GroundChanger.ChangeGroundOnWalk(player.transform, hit.transform, hit.normal));
                            Debug.Log("Case 2");
                        }
                        else if (StateManager.isGroundChanging) StateManager.isGrounded = true;
                    }
                }
                else if (StateManager.isGroundChanging) StateManager.isGrounded = true;
                else
                {
                    StateManager.isGrounded = false;
                    //StateManager.currentGround = null;
                }
                if (Vector3.Distance(player.transform.position, hit.point) != 0 && !StateManager.isFlying && !StateManager.isJumping) MoveController.instance.addGravityForce();
                yield return endOfFrame;
            }
        }
    }
    IEnumerator awareWorld(Camera cam)
    {
        float enabledDir = 200;
        Vector2 input = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
        RaycastHit hit;
        while (true)
        {
            Ray ray = cam.ScreenPointToRay(input);
            
            if (Physics.Raycast(ray, out hit, enabledDir) && !hit.collider.tag.Equals("Untagged"))
            {
                if (hit.collider.tag.Equals("Panel"))
                {
                    if (!StateManager.useSmallUI)
                    {
                        StageUI.instance.EnableTargetInfo("Interact (Q)");
                        StageUI.instance.TargetInfo.transform.position = input + Vector2.up * Screen.height * 0.2f;
                    }
                    else StageUI.instance.DisableTargetInfo();
                    if (StateManager.keySet.interact && !StateManager.useSmallUI)
                    {
                        StateManager.useSmallUI = true;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        //Debug.Log("USeSmallUI");
                    }

                }

                else if (!StateManager.useSmallUI && hit.collider.CompareTag("Ground")&& hit.distance > 20)// && !StateManager.isFlying && !hit.transform.gameObject.Equals(StateManager.currentGround))
                {
                    StageUI.instance.EnableTargetInfo(hit.collider.name);
                    StageUI.instance.ShowDistance((int)hit.distance);
                    StageUI.instance.TargetInfo.transform.position = input + Vector2.up * Screen.height * 0.2f;
                    Debug.DrawRay(Vector3.zero, hit.transform.forward, Color.yellow);
                    if (Input.GetKey(KeyCode.Z) && !StateManager.isFlying)
                    {
                        Debug.Log("StartFly");
                        StartCoroutine(GroundChanger.ChangeGroundOnJump(MoveController.instance.transform, hit.transform, hit.normal, hit.point, () => StartCoroutine(MoveController.instance.JumpLogic(4))));
                    }
                }
                else StageUI.instance.DisableTargetInfo();
            }
            else if (!(StateManager.isPaused))
            {
                StageUI.instance.DisableTargetInfo();
            }
            if (!(StateManager.isPaused))
            {
                Cursor.lockState = StateManager.useSmallUI ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = StateManager.useSmallUI;


            }
            yield return new WaitForEndOfFrame();
        }
    }
}
