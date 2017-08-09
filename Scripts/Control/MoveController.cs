using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour {

    public static MoveController instance;// 웨폰 공격 상태 채크에 사용
    public GameObject player;
    public float jumpPower = 10;
    public float moveSpeed = 8f;
    private float currentSpeed = 0;
    public float aimSpeed = 5;
    public float dashSpeed = 30;
    public float rotSpeed = 30;
    bool isDashMode = false;
    bool isPossibleDashMode = true;
    public bool isAttackMode = false;

    public bool isGrounded = true;
    public bool isJumping = false;

    protected Rigidbody rgb;
    public Animator animator;
    protected Transform camTransform;
    public Transform AttackArea;
    public GameObject currentGruond;
    private Quaternion deltaQ = Quaternion.identity;
    private void Awake()
    {
        instance = this;
        player = GameObject.Find("Player");
        rgb = GetComponent<Rigidbody>();
        animator = player.GetComponent<Animator>();
        camTransform = GameObject.Find("CamTransform").GetComponent<Transform>();
        AttackArea = GameObject.Find("AttackArea").GetComponent<Transform>();
        StartLogic();
    }
    void StartLogic()
    {
        StartCoroutine(Move());
        StartCoroutine(Rotate());
        StartCoroutine(CheckGround());
    }
    IEnumerator Move()
    {
        while(true)
        {
            if (StateManager.keySet.jump && isGrounded && !isJumping) StartCoroutine(Jump(jumpPower));
            if (isJumping) animator.SetBool("Jump", true);
            else animator.SetBool("Jump", false);
            if (!StateManager.keySet.aim) Walk_Default();
            else Walk_Aimed();
            if(StateManager.keySet.aim)
            {
                /*if(!isJumping)*/animator.SetBool("aimMode", true);
            }
            else animator.SetBool("aimMode", false);
            if (StateManager.keySet.moveKey)
            {
                if (isGrounded & !StateManager.isFlying)
                {
                    if (!isJumping) IncreaseSpeed();
                    else DecreaseSpeed();
                    animator.SetBool("Run", true);
                }
                else
                {
                    DecreaseSpeed();
                    animator.SetBool("Run", false);
                }
            }
            else
            {
                if (!StateManager.keySet.aim)
                    UpdateDeltaQ();//new Vector3(0, transform.eulerAngles.y - camTransform.eulerAngles.y, 0);
                DecreaseSpeed();
                if (currentSpeed == 0)
                {
                    animator.SetBool("Run", false);
                    animator.SetBool("aimMode", false);
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator Rotate()
    {
        StartCoroutine(CheckGround());
        StartCoroutine(Move());
        //StartCoroutine(DashMode());
        while (true)
        {

            Debug.DrawRay(Vector3.zero, transform.forward, Color.cyan);
            if (isJumping || StateManager.isFlying)
            {
                //UpdateDeltaQ();
                yield return null;
                continue;
            }
            if (StateManager.keySet.aim)
            {
                DoRotate_Aimed();
            }
            else
            {
                DoRotate_Default();
            }
            yield return null;
            
        }
    }

    IEnumerator DashMode()
    {
        float originSpeed = moveSpeed;
        float dashSpeed = moveSpeed * 6;
        while(true)
        {
            if (Input.GetKey(KeyCode.Space)) moveSpeed = dashSpeed;
            else moveSpeed = originSpeed;
            yield return null;
        }
        /*isPossibleDashMode = false;
        float duration = 0.14f;
        float delay = 1f;
        float moveSpeed = this.moveSpeed;
        isDashMode = true;
        this.currentSpeed = dashSpeed;
        yield return new WaitForSecondsRealtime(duration);
        this.moveSpeed = moveSpeed;
        isDashMode = false;
        yield return new WaitForSecondsRealtime(delay);
        isPossibleDashMode = true;*/

    }

    void Attack()
    {
        //if (ItemShortCut.instance.currentWeapon.itemObject.name.Equals("none") || ItemShortCut.instance.currentCapsule.itemObject.name.Equals("none")) return;
        //if (ItemShortCut.instance.currentCapsule.itemCount == 0) return;
        //StartCoroutine(ItemShortCut.instance.currentWeapon.itemObject.GetComponent<Item_Weapon>().Attack());//AttackMode());

    }
    void Dash()
    {
        if (!StateManager.keySet.moveKey) return;
        if(StateManager.keySet.dash && isPossibleDashMode)
            StartCoroutine(DashMode());
    }
    void DoRotate_Default()
    {
        if (!StateManager.keySet.moveKey) return;

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


        if (Quaternion.Angle(q, deltaQ) < 100)
        {
            IncreaseSpeed();
        }
        else
        {
            DecreaseSpeed();
            if (currentSpeed > 5) return;
        }
        // transform.rotation = camTransform.rotation * q;
        //transform.localEulerAngles = new Vector3(tempX, transform.localEulerAngles.y, tempZ);
        // deltaQ.eulerAngles = Vector3.Lerp(deltaQ.eulerAngles, q.eulerAngles, rotSpeed * Time.deltaTime);


        player.transform.localEulerAngles = new Vector3(tempX, (camTransform.localRotation * deltaQ).eulerAngles.y, tempZ);
        deltaQ = Quaternion.Lerp(deltaQ, q, rotSpeed * Time.smoothDeltaTime);

        //transform.rotation = Quaternion.Lerp(transform.rotation, camTransform.rotation * q, rotSpeed * Time.deltaTime);
        //transform.localEulerAngles = new Vector3(tempX, transform.localEulerAngles.y, tempZ);


    }
    void Walk_Default()
    {
        //Vector3 moveDir = Vector3.forward;
       // transform.Translate(moveDir.normalized * currentSpeed * Time.smoothDeltaTime, Space.Self);
        
        Vector3 v = player.transform.rotation * Vector3.forward;
        rgb.MovePosition(transform.position + (v.normalized * currentSpeed * Time.deltaTime));
        animator.SetFloat("runSpeed", currentSpeed / 30.0f + 0.5f);
    }
    void DoRotate_Aimed()
    {
        float tempX = player.transform.localEulerAngles.x;
        float tempZ = player.transform.localEulerAngles.z;
        Quaternion q = Quaternion.identity;
        q.eulerAngles = Vector3.zero;

        player.transform.localEulerAngles = new Vector3(tempX, (camTransform.localRotation * deltaQ).eulerAngles.y, tempZ);
        deltaQ = Quaternion.Lerp(deltaQ, q, rotSpeed * Time.smoothDeltaTime);

        
        /*float tempX = transform.localEulerAngles.x;
        float tempZ = transform.localEulerAngles.z;
        //transform.rotation = Quaternion.Lerp(transform.rotation, camTransform.rotation, Time.smoothDeltaTime * rotSpeed);
        transform.rotation = camTransform.rotation;*/
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, camTransform.localEulerAngles.y, transform.localEulerAngles.z);
    }
    void Walk_Aimed()
    {
        Vector3 moveDir = transform.rotation*  ((Vector3.forward * StateManager.keySet.v) + (Vector3.right * StateManager.keySet.h));
        if(currentSpeed > aimSpeed) currentSpeed -= Time.deltaTime * 70;
        if (currentSpeed < aimSpeed) currentSpeed += Time.deltaTime * 30;
        rgb.MovePosition(transform.position + (moveDir.normalized * currentSpeed * Time.deltaTime));
        //transform.Translate(moveDir.normalized * currentSpeed * Time.smoothDeltaTime, Space.Self);
    }

    public IEnumerator Jump(float power)
    {
        isJumping = true;
        float speed = currentSpeed;
        animator.SetFloat("JumpSpeed", 1 +  (currentSpeed / moveSpeed));
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("JUMP00"));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.15f);// wait for animation sink
        Vector3 v = new Vector3(rgb.velocity.x, power, rgb.velocity.z);
        Vector3 f = new Vector3(rgb.velocity.x, rgb.velocity.y, speed * 60);//(transform.rotation * Vector3.forward) * 25 * speed;
        f.y = 0;
        rgb.AddForce(transform.rotation * v, ForceMode.Impulse);
        yield return new WaitWhile(() => isGrounded);
        rgb.AddForce(player.transform.rotation * f, ForceMode.Force);
        //yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.3f);
        bool isStoppedAnim = false;
        while (!isGrounded)
        {

            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 > 0.3f && !isStoppedAnim)
            {
                animator.SetFloat("JumpSpeed", 0);
                isStoppedAnim = true;
            }
            yield return null;
            //if(Mathf.Abs(rgb.velocity.y) < 4f) rgb.AddForce(v * -1, ForceMode.Impulse);
        }
        isJumping = false;
        //rgb.velocity = new Vector3(rgb.velocity.x, jumpPower* -1, rgb.velocity.z);
    }
    IEnumerator CheckGround()
    {
        while(true)
        {
            float dist = transform.lossyScale.y;
            RaycastHit hit;
            while (true)
            {
                Debug.DrawRay(player.transform.position + player.transform.rotation * Vector3.up*0.1f, player.transform.rotation * Vector3.down * dist * 0.1f, Color.red, Time.deltaTime);
                if (Physics.Raycast(player.transform.position + player.transform.rotation * Vector3.up * 0.1f, player.transform.rotation * Vector3.down * dist * 0.1f, out hit, dist))
                {
                    Debug.Log(hit.normal.normalized + "," + transform.up.normalized);
                    if (hit.collider.tag == "Ground")
                    {

                        if(Vector3.Angle(hit.normal.normalized,transform.up.normalized) != 0) StartCoroutine(GroundChanger.instance.ChangeGroundOnWalk(hit.transform, hit.normal));
                        if (hit.collider.gameObject.Equals(currentGruond)) isGrounded = true;
                        else if (!StateManager.isFlying && !StateManager.isGroundChanging)
                        {
                            //Debug.Log(currentGruond.name);
                            StartCoroutine(GroundChanger.instance.ChangeGroundOnWalk(hit.transform,hit.normal));
                        }
                        else if (StateManager.isGroundChanging) isGrounded = true;
                    }
                }
                else if (StateManager.isGroundChanging) isGrounded = true;
                else
                {
                    isGrounded = false;
                    currentGruond = null;
                }
                if (Vector3.Distance(player.transform.position, hit.point) != 0 && !StateManager.isFlying && !isJumping) addGravityForce();
                yield return null;
            }
        }
    }
    public void addGravityForce()
    {
        //if (StateManager.isFlying) return;
        rgb.velocity += transform.rotation * Vector3.up * (-9.81f * Time.deltaTime);
    }
    void IncreaseSpeed()
    {
        if (isDashMode) return;
        currentSpeed += Time.deltaTime * 15;
        if (currentSpeed > moveSpeed) DecreaseSpeed();//currentSpeed = moveSpeed;
    }
    void DecreaseSpeed()
    {
        if (isDashMode) return;
        currentSpeed -= Time.deltaTime * 100;
        if (currentSpeed < 0) currentSpeed = 0;
    }

    public void UpdateDeltaQ()
    {
        deltaQ.eulerAngles = Vector3.up * (transform.localEulerAngles.y - camTransform.localEulerAngles.y);
    }

    public void resetVelocity()
    {
        rgb.velocity = Vector3.zero;
    }
    /*Vector3 ForwardAngles()
    {
        return new Vector3(this.transform.localEulerAngles.x, camTransform.position.y, this.transform.localEulerAngles.z);
    }*/
}
