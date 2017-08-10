using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour {

    public static MoveController instance;// 웨폰 공격 상태 채크에 사용

    public float jumpPower = 10;
    public float runSpeed = 10;
    public float walkSpeed = 5;
    public float rotSpeed = 30;

    private float moveSpeed = 0f;
    private float m_currentSpeed = 0;
    private float currentSpeed
    {
        get { return m_currentSpeed; }
        set
        {
            if (value < 0) value = 0;
            if (value > moveSpeed) value = moveSpeed;
            m_currentSpeed = value;
        }
    }


    public GameObject player;
    private Rigidbody rgb;
    private Transform camTransform;
    public Animator animator;
    public GameObject currentGruond;
    private Quaternion deltaQ = Quaternion.identity;

    delegate void WorkList();

    private void Awake()
    {
        instance = this;
        camTransform = GameObject.Find("CamTransform").GetComponent<Transform>();
        player = GameObject.Find("Player");
        rgb = GetComponent<Rigidbody>();
        animator = player.GetComponent<Animator>();

        StartLogic();
    }


    IEnumerator MainLoop()
    {
        WorkList workList;
        while (true)
        {
            workList = null;

            workList += animatorUpdate;
            workList += Walk_Default;
            workList += UpdateDeltaQ;
            workList += DecreaseSpeed;

            if (StateManager.keySet.moveKey)
            {
                workList -= UpdateDeltaQ;
                workList -= DecreaseSpeed;

                workList += IncreaseSpeed;
                workList += DoRotate_Default;
            }

            if (StateManager.keySet.aim)
            {
                workList -= UpdateDeltaQ;
                workList -= Walk_Default;
                workList -= DoRotate_Default;

                workList += Walk_Aimed;
                workList += DoRotate_Aimed;
            }

            if(StateManager.isJumping)
            {
                workList -= IncreaseSpeed;
                workList += DecreaseSpeed;
            }

            if(StateManager.keySet.jump && !StateManager.isJumping)
            {
                workList += () => StartCoroutine(JumpLogic(jumpPower));
            }

            workList();
            yield return null;
        }
    }
    IEnumerator CheckGround()
    {
        while (true)
        {
            float dist = transform.lossyScale.y * 2;
            RaycastHit hit;
            while (true)
            {
                Debug.DrawRay(player.transform.position + player.transform.rotation * Vector3.up * dist, player.transform.rotation * Vector3.down * (dist + 0.5f), Color.red, Time.deltaTime);
                if (Physics.Raycast(player.transform.position + player.transform.rotation * Vector3.up * dist, player.transform.rotation * Vector3.down, out hit, dist + 0.5f))
                {
                    if (hit.collider.tag == "Ground" && !StateManager.isFlying)
                    {
                        if (Vector3.Angle(hit.normal.normalized, transform.up.normalized) != 0) StartCoroutine(GroundChanger.ChangeGroundOnWalk(this.transform,hit.transform, hit.normal));
                        if (hit.collider.gameObject.Equals(currentGruond)) StateManager.isGrounded = true;
                        else if (!StateManager.isFlying && !StateManager.isGroundChanging)
                        {
                            StartCoroutine(GroundChanger.ChangeGroundOnWalk(this.transform, hit.transform, hit.normal));
                        }
                        else if (StateManager.isGroundChanging) StateManager.isGrounded = true;
                    }
                }
                else if (StateManager.isGroundChanging) StateManager.isGrounded = true;
                else
                {
                    StateManager.isGrounded = false;
                    currentGruond = null;
                }
                if (Vector3.Distance(player.transform.position, hit.point) != 0 && !StateManager.isFlying && !StateManager.isJumping) addGravityForce();
                yield return null;
            }
        }
    }
    public IEnumerator JumpLogic(float power)
    {
        StateManager.isJumping = true;
        float speed = currentSpeed;
        animator.SetFloat("JumpSpeed", 1 + (currentSpeed / moveSpeed));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("JUMP00"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.15f);// wait for animation sinc
        Vector3 v = new Vector3(rgb.velocity.x, power, rgb.velocity.z);
        Vector3 f = new Vector3(rgb.velocity.x, rgb.velocity.y, speed * 60);//(transform.rotation * Vector3.forward) * 25 * speed;
        f.y = 0;
        rgb.AddForce(transform.rotation * v, ForceMode.Impulse);
        rgb.AddForce(player.transform.rotation * f, ForceMode.Force);
        yield return new WaitWhile(() => StateManager.isGrounded);
        //yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.3f);
        bool isStoppedAnim = false;
        while (!StateManager.isGrounded || StateManager.isFlying)
        {

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.4f && !isStoppedAnim)
            {
                animator.SetFloat("JumpSpeed", 0);
                isStoppedAnim = true;
            }
            yield return null;
        }
        UpdateDeltaQ();
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7f);
        StateManager.isJumping = false;
    }
    void StartLogic()
    {
        StartCoroutine(MainLoop());
        StartCoroutine(CheckGround());
    }
    void DoRotate_Default()
    {
        float tempX = player.transform.localEulerAngles.x;
        float tempZ = player.transform.localEulerAngles.z;

        Quaternion q = Quaternion.identity;

        if (StateManager.keySet.front)
        {
            q.eulerAngles += Vector3.up * 360;
        }

        else if (StateManager.keySet.back)
        {
            q.eulerAngles += Vector3.up * 180;
        }

        if (StateManager.keySet.left)
        {
            q.eulerAngles += Vector3.up * 270;
        }

        if (StateManager.keySet.right)
        {
            q.eulerAngles += Vector3.up * 90;
        }
        if ((StateManager.keySet.front || StateManager.keySet.back) && (StateManager.keySet.left || StateManager.keySet.right))
        {
            q.eulerAngles *= 0.5f;
            if (StateManager.keySet.left) q.eulerAngles += Vector3.up * 180;
        }


        if (Quaternion.Angle(q, deltaQ) < 100) IncreaseSpeed();
        else
        {
            DecreaseSpeed();
            if (currentSpeed > 5) return;
        }

        player.transform.localEulerAngles = new Vector3(tempX, (camTransform.localRotation * deltaQ).eulerAngles.y, tempZ);
        deltaQ = Quaternion.Lerp(deltaQ, q, rotSpeed * Time.smoothDeltaTime);
    }
    void DoRotate_Aimed()
    {
        float tempX = player.transform.localEulerAngles.x;
        float tempZ = player.transform.localEulerAngles.z;
        Quaternion q = Quaternion.identity;
        q.eulerAngles = Vector3.zero;
        player.transform.localEulerAngles = new Vector3(tempX, (camTransform.localRotation * deltaQ).eulerAngles.y, tempZ);
        deltaQ = Quaternion.Lerp(deltaQ, q, rotSpeed * Time.smoothDeltaTime);
    }
    void Walk_Aimed()
    {
        moveSpeed = walkSpeed;
        Vector3 moveDir = player.transform.rotation * ((Vector3.forward * StateManager.keySet.v) + (Vector3.right * StateManager.keySet.h));
        rgb.MovePosition(transform.position + (moveDir.normalized * currentSpeed * Time.deltaTime));
    }
    void Walk_Default()
    {
        moveSpeed = runSpeed;
        Vector3 v = player.transform.rotation * Vector3.forward;
        rgb.MovePosition(transform.position + (v.normalized * currentSpeed * Time.deltaTime));
        animator.SetFloat("runSpeed", currentSpeed / 30.0f + 0.5f);
    }

    void animatorUpdate()
    {
        animator.SetBool("aimMode", currentSpeed != 0 && StateManager.keySet.aim);
        animator.SetBool("Run", currentSpeed != 0 || StateManager.keySet.moveKey);
        animator.SetBool("Jump", StateManager.isJumping);
    }
    void IncreaseSpeed()
    {
        currentSpeed += Time.deltaTime * 15;
    }
    void DecreaseSpeed()
    {
        currentSpeed -= Time.deltaTime * 100;
    }
    public void addGravityForce()
    {
        rgb.velocity += transform.rotation * Vector3.up * (-9.81f * Time.deltaTime);
    }
    public void UpdateDeltaQ()
    {
        deltaQ.eulerAngles = Vector3.up * (player.transform.localEulerAngles.y - camTransform.localEulerAngles.y);
    }
    public void UpdateDeltaQ(Vector3 euler)
    {
        deltaQ.eulerAngles = euler;
    }
    public void UpdateDeltaQ(Quaternion rot)
    {
        deltaQ = rot;
    }
    public void resetVelocity(Vector3 v)
    {
        rgb.velocity = v;
    }
}
