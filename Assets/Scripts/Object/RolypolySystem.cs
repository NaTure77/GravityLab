using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RolypolySystem : MonoBehaviour {

    private void Start()
    {
        StateManager.currentPosition = transform.position;
        StateManager.currentRotation = transform.rotation;
        StartCoroutine(Rolypoly(transform,transform.GetChild(0)));
    }
    public IEnumerator Rolypoly(Transform playerObj, Transform player)
    {
        Vector3 currentVector = MoveController.instance.transform.up;
        while(true)
        {
            RaycastHit hit;
            /*if(playerObj.parent != null)
            {
                yield return null;
                continue;
            }*/
            // Debug.DrawRay(player.transform.position, -player.transform.up * 3, Color.red);
            if (Physics.SphereCast(player.transform.position + player.transform.rotation * Vector3.up * 2.5f, 0.7f, -player.transform.up, out hit, 2.5f) || Physics.SphereCast(player.transform.position + player.transform.rotation * Vector3.up * 2.5f, 0.7f, player.transform.up, out hit, 1.9f))
            {

                if (hit.collider.CompareTag("Ground")&& !StateManager.isJumping )
                {
                    if(StateManager.isFlying)
                    {
                        //ChangePlayerAngles_H(playerObj.transform,hit.normal);
                        //ChangePlayerAngles_V(playerObj.transform, hit.normal);
                        //playerObj.transform.position = hit.point;
                        StartCoroutine(GroundChanger.ChangeGroundOnWalk(playerObj.transform, hit.transform, hit.normal));
                    }
                    else if (Vector3.Angle(hit.normal.normalized, playerObj.up.normalized) != 0 && !StateManager.isGroundChanging)
                    {
                        //Debug.Log("111");
                        Debug.Log(Vector3.Angle(hit.normal.normalized, playerObj.up.normalized));
                        currentVector = hit.normal;
                        StartCoroutine(GroundChanger.ChangeGroundOnWalk(playerObj.transform, hit.transform, hit.normal));
                    }
                    StateManager.isFloating = false;
                    StateManager.isGrounded = true;
                }
                else if (hit.collider.CompareTag("DeadWall"))
                {
                    StateManager.isFlying = false;
                    StartCoroutine(StageUI.instance.ShowTryAgain());
                    transform.SetParent(null);
                    playerObj.transform.position = StateManager.currentPosition;
                    playerObj.transform.rotation = StateManager.currentRotation;
                    MoveController.instance.resetVelocity(Vector3.zero);
                }
            }
            else
            {
                StateManager.isGrounded = false;
                //StateManager.isColliderGrounded = false;
                StateManager.isFloating = true;
                if(!StateManager.isFlying) StartCoroutine(FlySystem.instance.Floating(playerObj.transform));
                //StartCoroutine(FlySystem.instance.Floating(playerObj.transform));
                //StateManager.currentGround = null;
            }
            if (StateManager.isGroundChanging) StateManager.isGrounded = true;

            if((StateManager.isFloating || !StateManager.isColliderGrounded) && !StateManager.isFlying) MoveController.instance.addGravityForce();// 완전밀착을 위해 콜라이더 그라운디드까지 채크
            yield return null;
        }
    }
    private IEnumerator fallDown(Transform playerObj, Vector3 normal)
    {

        float angle = Vector3.Angle(playerObj.GetChild(0).forward.normalized, -normal.normalized);
        Vector3 outerProduct = Vector3.Cross(playerObj.GetChild(0).forward.normalized, -normal.normalized);//playerObj.transform.InverseTransformPoint(Vector3.Cross(playerObj.up.normalized, normal.normalized)).normalized;
        Quaternion fromRotation = playerObj.rotation;
        Quaternion toRotation = Quaternion.AngleAxis(angle, outerProduct) * fromRotation;
        float fromToRate = 0;
        float totalTime = 0.6f;

        while (fromToRate < 1)// && StateManager.currentGround == (groundT.gameObject))
        {
            playerObj.rotation = Quaternion.LerpUnclamped(fromRotation, toRotation, fromToRate);
            fromToRate += Time.deltaTime / totalTime;
            yield return null;
        }
        //if (StateManager.currentGround == (groundT.gameObject))
        {
            playerObj.rotation = toRotation;

            StateManager.isGrounded = true;
        }
    }
    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        RaycastHit hit;
        //Gizmos.DrawRay(transform.position + transform.rotation * Vector3.up * 1.2f, -transform.up * 3);
        //Gizmos.DrawWireSphere(transform.position + transform.rotation * Vector3.up * 1.2f, 1.2f);
        if (Physics.SphereCast(transform.position + transform.rotation * Vector3.up * 2.5f, 0.7f, -transform.up, out hit, 2.5f))
        {
            Gizmos.DrawRay(transform.position + transform.rotation * Vector3.up * 2.5f, -transform.up * 1.8f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * Vector3.up* 2.5f + -transform.up * hit.distance, 0.7f);
        }
        if(Physics.SphereCast(transform.position + transform.rotation * Vector3.up * 2.5f, 0.7f, transform.up, out hit, 1.9f))
        {
            Gizmos.DrawRay(transform.position + transform.rotation * Vector3.up * 2.5f, transform.up * 1.8f);
            Gizmos.DrawWireSphere(transform.position + transform.rotation * Vector3.up * 2.5f + transform.up * hit.distance, 0.7f);
        }
    }*/

    private void ChangePlayerAngles_H(Transform playerObj, Vector3 normal)
    {
        float angle = Vector3.Angle(playerObj.GetChild(0).forward.normalized, -normal.normalized);
        Vector3 outerProduct = Vector3.Cross(playerObj.GetChild(0).forward.normalized, -normal.normalized);//playerObj.transform.InverseTransformPoint(Vector3.Cross(playerObj.up.normalized, normal.normalized)).normalized;
        Quaternion fromRotation = playerObj.rotation;
        Quaternion toRotation = Quaternion.AngleAxis(angle, outerProduct) * fromRotation;
        playerObj.rotation = toRotation;
    }
    private void ChangePlayerAngles_V(Transform playerObj, Vector3 normal)
    {
        float angle = Vector3.Angle(playerObj.up.normalized, normal.normalized);
        Vector3 outerProduct = Vector3.Cross(playerObj.up.normalized, normal.normalized);//playerObj.transform.InverseTransformPoint(Vector3.Cross(playerObj.up.normalized, normal.normalized)).normalized;
        Quaternion fromRotation = playerObj.rotation;
        Quaternion toRotation = Quaternion.AngleAxis(angle, outerProduct) * fromRotation;
        playerObj.rotation = toRotation;
    }
    /*protected void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter");
        if(!StateManager.isStanding && !StateManager.isGroundChanging)
        {
            ChangePlayerAngles(transform, collision.contacts[0].normal);
            transform.position = collision.contacts[0].point;
            MoveController.instance.resetVelocity(Vector3.zero);

        }
        StateManager.isGrounded = true;
        //StartCoroutine(GroundChanger.ChangeGroundOnWalk(this.transform,collision.transform,collision.contacts[0].normal));
    }
    */
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") && !StateManager.isGroundChanging)
        {
            MoveController.instance.resetVelocity(Vector3.zero);
            //if(StateManager.isFlying)
            StateManager.isColliderGrounded = true;
            Debug.Log("CollisionEnter");
            StartCoroutine(GroundChanger.ChangeGroundOnWalk(this.transform, collision.transform, collision.contacts[0].normal));
        }
        else if(collision.transform.CompareTag("DeadWall"))
        {
            Debug.Log("DeadWall");
            transform.SetParent(null);
            StartCoroutine(StageUI.instance.ShowTryAgain());
            transform.position = StateManager.currentPosition;
            transform.rotation = StateManager.currentRotation;
        }
        if (collision.transform.GetComponent<GroundInfo>())
        {
            MoveController.instance.runSpeed = collision.transform.GetComponent<GroundInfo>().playerSpeed;
        }
        if (collision.transform.GetComponent<AudioManager>())
        {
            transform.GetComponentInChildren<AudioManager>().FootStep = collision.transform.GetComponent<AudioManager>().FootStep;
        }



    }
    protected void OnCollisionExit(Collision collision)
     {
         //Debug.Log("Exit");
         if (collision.transform.CompareTag("Ground")) StateManager.isColliderGrounded = false;
     }
    
    protected void OnCollisionStay(Collision collision)
    {
        //Debug.Log("Stay");
        if (collision.transform.CompareTag("Ground") && !StateManager.isJumping)// && !StateManager.isGroundChanging)
        {
            StateManager.isColliderGrounded = true;
        }
    }
}
