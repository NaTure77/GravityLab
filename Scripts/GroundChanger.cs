using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChanger : MonoBehaviour {

    public static GroundChanger instance;
    GameObject cam;
    GameObject player;

    GameObject parent;
    public float flySpeed = 10.0f;
    public float rotateSpeed  = 3;
    private void Awake()
    {
        instance = this;
        cam = GameObject.Find("CamTransform");
        player = GameObject.Find("Player");
        parent = GameObject.Find("PlayerObj");
    }

    public IEnumerator ChangeGroundOnJump(Transform groundT, Vector3 destination)
    {
        StateManager.isFlying = true;
        float distance = Vector3.Distance(player.transform.position, destination);
        //StartCoroutine(MoveController.instance.Jump(Vector3.Distance(MoveController.instance.transform.position, destination)*0.05f));
        MoveController.instance.resetVelocity();

        if (!MoveController.instance.isJumping) StartCoroutine(MoveController.instance.Jump(4));
        yield return new WaitWhile(() => MoveController.instance.isGrounded);
        MoveController.instance.currentGruond = groundT.gameObject;

        Debug.Log("PlayerUp: "+ player.transform.up);
        Debug.Log("GroundUp" + groundT.transform.up);
        float tx = player.transform.up.x - groundT.transform.up.x;
        float ty = player.transform.up.y - groundT.transform.up.y;
        float tz = player.transform.up.z - groundT.transform.up.z;
        Debug.Log("subractResult: " + tx + "," + ty + "," + tz);

        ChangeParent(null);
        //parent.transform.SetParent(null);

        //Quaternion toRot = Quaternion.identity;
        //toRot.eulerAngles = groundT.transform.rotation * Vector3.up;


        Quaternion fromRotation = player.transform.localRotation;
        Vector3 fromPosition = player.transform.position;
        /*float y = player.transform.localEulerAngles.y;
        float x = player.transform.localEulerAngles.x;
        float z = player.transform.localEulerAngles.z;

        float a = groundT.transform.localEulerAngles.x - player.transform.localEulerAngles.x;
        float b = groundT.transform.localEulerAngles.z - player.transform.localEulerAngles.z;*/
        //Debug.Log(a +","+b);
        //Debug.Log(player.transform.eulerAngles.y);
        //Debug.Log(Vector3.Angle(player.transform.up, groundT.transform.up));

        //Vector3 tempForward1 =player.transform.forward;


        //#1
        //player.transform.up = groundT.transform.up;
        //player.transform.forward = tempForward1;
        //Debug.Log(y - player.transform.rotation.y);
        //player.transform.Rotate(0,y - Quaternion.Euler(player.transform.localEulerAngles).y,0,Space.Self);
        //if(z - player.transform.eulerAngles.z >= 90) player.transform.Rotate(0, 180, 0, Space.Self);
        //var rot = transform.rotation;
        //player.transform.rotation = rot * Quaternion.Euler(0, temp1.y - player.transform.rotation.y, 0);
        //#2
        //Vector3 angle = Vector3.up * (groundT.transform.eulerAngles.y - y);
        //player.transform.rotation = player.transform.rotation * Quaternion.FromToRotation(player.transform.up.normalized, groundT.up.normalized);

        //#4
        /*Vector3 backUp = player.transform.up;
        player.transform.up = Quaternion.AngleAxis(Vector3.Angle(player.transform.up, groundT.up), Vector3.Cross(player.transform.up, groundT.up)) * player.transform.up;

        if (Vector3.Angle(player.transform.up, groundT.transform.up) > 0.1f)
        {
            player.transform.up = backUp;
            player.transform.up = Quaternion.AngleAxis(Vector3.Angle(player.transform.up, groundT.up) * -1, Vector3.Cross(player.transform.up, groundT.up)) * player.transform.up;
        }*/


        //#3
        //player.transform.Rotate(GetOuterProduct(groundB.transform.up, groundT.transform.up), Vector3.Angle(groundB.transform.up, groundT.transform.up));
        //player.transform.Rotate(GetOuterProduct(groundB.transform.rotation * Vector3.up, groundT.transform.rotation * Vector3.up), Vector3.Angle(groundB.transform.up, groundT.transform.up));


        //Debug.Log(groundT.transform.rotation.x + groundT.transform.rotation.z);
        //if(Mathf.Abs(groundT.transform.localEulerAngles.x - x) != 90)

        //player.transform.Rotate(GetOuterProduct(player.transform.up,groundT.transform.up), Vector3.Angle(player.transform.up, groundT.transform.up),Space.Self);

        //player.transform.Rotate(0,( a + b), 0);
        //player.transform.Rotate(groundT.transform.eulerAngles.x, 0, groundT.transform.eulerAngles.z, Space.World);

        Quaternion toRotation = Quaternion.AngleAxis(Vector3.Angle(player.transform.up.normalized, groundT.up.normalized), Vector3.Cross(player.transform.up, groundT.up)) * player.transform.localRotation;

        //player.transform.rotation = fromRotation;

        cam.transform.SetParent(player.transform);
        CamCtrl.instance.resetHeading();
        Debug.DrawRay(Vector3.zero, player.transform.up * 3, Color.green, 3);
        Debug.DrawRay(Vector3.zero, groundT.transform.up * 3, Color.red,3);
        //Debug.DrawRay(Vector3.zero, GetOuterProduct(groundB.rotation * Vector3.up, groundT.rotation * Vector3.up), Color.blue,3);
        Debug.DrawRay(Vector3.zero, Vector3.Cross(player.transform.up, groundT.up), Color.blue,3);
        float currentDistance = 0;
        float fromToRate = 0;
        //Debug.Log("FirstRotateStart");
        while(fromToRate < 1)//while (!MoveController.instance.isGrounded)
        {
            Debug.DrawRay(Vector3.zero, player.transform.up, Color.black, 5);
            Debug.DrawRay(Vector3.zero, player.transform.forward, Color.cyan, 5);

            currentDistance = Vector3.Distance(player.transform.position, destination);
            fromToRate += (flySpeed / distance);
            player.transform.localRotation = Quaternion.LerpUnclamped(fromRotation, toRotation, fromToRate);
            //player.transform.position = Vector3.MoveTowards(player.transform.position, destination, flySpeed * Time.deltaTime);
            player.transform.position =Vector3.LerpUnclamped(fromPosition, destination, fromToRate);
            //player.transform.position += player.transform.rotation * Vector3.up * distance * 0.02f *(Mathf.Sin((t / distance) * Mathf.PI* 2));
            //t += Time.deltaTime * flySpeed;
            if(currentDistance < 130) MoveController.instance.animator.SetFloat("JumpSpeed", 1);
            yield return null;
        }
       // Debug.Log("firstRotateFin");
        MoveController.instance.resetVelocity();
        /*while (Quaternion.Angle(player.transform.rotation, toRotation) != 0 && MoveController.instance.currentGruond == groundT.gameObject)
        {
            fromToRate += (flySpeed / distance);
            player.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, fromToRate);
            yield return null;
        }*/
        MoveController.instance.resetVelocity();
        parent.transform.rotation = player.transform.rotation;
        ChangeParent(parent.transform);
        parent.transform.parent = groundT.parent.parent;
        CamCtrl.instance.resetHeading();
        MoveController.instance.UpdateDeltaQ();
        //parent.transform.SetParent(MoveController.instance.currentGruond.transform);
        //Debug.Log("Grounded!");

        StateManager.isFlying = false;
        //Debug.Log("FlyFin");
    }

    public IEnumerator ChangeGroundOnWalk(Transform groundT, Vector3 destination)
    {

        //StartCoroutine(MoveController.instance.Jump(Vector3.Distance(MoveController.instance.transform.position, destination)*0.05f));
        Debug.Log("PlayerUp: " + player.transform.up);
        Debug.Log("GroundUp" + groundT.transform.up);
        float tx = player.transform.up.x - groundT.transform.up.x;
        float ty = player.transform.up.y - groundT.transform.up.y;
        float tz = player.transform.up.z - groundT.transform.up.z;
        Debug.Log("subractResult: " + tx + "," + ty + "," + tz);



        StateManager.isGroundChanging = true;
        yield return new WaitForSeconds(0.01f);
        MoveController.instance.currentGruond = groundT.gameObject;
        //yield return new WaitForSecondsRealtime(0.4f);
        ChangeParent(null);


        /*float gapAngle = player.transform.eulerAngles.y - groundT.eulerAngles.y;

        float y = player.transform.localEulerAngles.y;
        float x = player.transform.localEulerAngles.x;
        float z = player.transform.localEulerAngles.z;

        //#1
        //player.transform.up = groundT.up;//groundT.localToWorldMatrix * Vector3.up;
        //player.transform.forward = tempForward1;
        //Debug.Log(y - player.transform.rotation.y);
        Debug.Log(gapAngle);
        Debug.Log(y);
        Debug.Log(y + gapAngle);
        //player.transform.Rotate(0, gapAngle, 0, Space.Self);
        //if (z - player.transform.eulerAngles.z >= 90) player.transform.Rotate(0, 180, 0, Space.Self);
        var rot = transform.rotation;
        //player.transform.rotation = fromRotation;
        */

        Quaternion fromRotation = player.transform.localRotation;
        Quaternion toRotation = Quaternion.AngleAxis(Vector3.Angle(player.transform.up.normalized, groundT.up.normalized), Vector3.Cross(player.transform.up, groundT.up)) * player.transform.localRotation;

        cam.transform.SetParent(player.transform);
        CamCtrl.instance.resetHeading();
        float fromToRate = 0;
        float totalTime = 0.3f;
        while (fromToRate < 1 && MoveController.instance.currentGruond == groundT.gameObject)
        {
            player.transform.localRotation = Quaternion.Lerp(fromRotation, toRotation, fromToRate);
            fromToRate += Time.deltaTime / totalTime;
            yield return null;
        }
        if (MoveController.instance.currentGruond == groundT.gameObject)
        {
            parent.transform.rotation = player.transform.rotation;
            ChangeParent(parent.transform);
            MoveController.instance.UpdateDeltaQ();
            StateManager.isGroundChanging = false;
            Debug.Log("RotateFin");
        }
        Debug.Log("RotateEnd");
    }
    public void ChangeParent(Transform t)
    {
        player.transform.SetParent(t);
        cam.transform.SetParent(t);
    }

    Vector3 GetOuterProduct(Vector3 a, Vector3 b)
    {
        float x = a.y * b.z - a.z * b.y;
        float y = a.z * b.x - a.x * b.z;
        float z = a.x * b.y - a.y - b.x;

        return new Vector3(x, y, z).normalized;
    }
}
