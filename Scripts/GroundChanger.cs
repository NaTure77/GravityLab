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

    public IEnumerator ChangeGroundOnJump(Transform groundT,Vector3 normal, Vector3 destination)
    {
        StateManager.isFlying = true;
        float distance = Vector3.Distance(parent.transform.position, destination);
        //StartCoroutine(MoveController.instance.Jump(Vector3.Distance(MoveController.instance.transform.position, destination)*0.05f));
        MoveController.instance.resetVelocity();

        if (!MoveController.instance.isJumping) StartCoroutine(MoveController.instance.Jump(4));
        yield return new WaitWhile(() => MoveController.instance.isGrounded);
        MoveController.instance.currentGruond = groundT.gameObject;
        //parent.transform.parent = groundT.parent;
        Debug.Log("PlayerUp: "+ player.transform.up);
        Debug.Log("GroundUp" + groundT.transform.up);
        float tx = player.transform.up.x - groundT.transform.up.x;
        float ty = player.transform.up.y - groundT.transform.up.y;
        float tz = player.transform.up.z - groundT.transform.up.z;
        Debug.Log("subractResult: " + tx + "," + ty + "," + tz);

        //ChangeParent(parent.transform.parent);

        Quaternion fromRotation = parent.transform.rotation;
        Vector3 fromPosition = parent.transform.position;
        
        Quaternion toRotation = Quaternion.AngleAxis(Vector3.Angle(parent.transform.up.normalized, normal.normalized), Vector3.Cross(parent.transform.up.normalized, normal.normalized)) * parent.transform.rotation;


        //cam.transform.SetParent(player.transform);
        CamCtrl.instance.resetHeading();
        Debug.DrawRay(Vector3.zero, player.transform.up * 3, Color.green, 3);
        Debug.DrawRay(Vector3.zero, groundT.transform.up * 3, Color.red,3);
        Debug.DrawRay(Vector3.zero, Vector3.Cross(player.transform.up, groundT.up), Color.blue,3);
        float currentDistance = 0;
        float totalTime = 2;
        float fromToRate = 0;
        //Debug.Log("FirstRotateStart");
        while(fromToRate < 1)//while (!MoveController.instance.isGrounded)
        {
            Debug.DrawRay(Vector3.zero, player.transform.up, Color.black, 5);
            Debug.DrawRay(Vector3.zero, player.transform.forward, Color.cyan, 5);

            currentDistance = Vector3.Distance(parent.transform.position, destination);
            fromToRate += Time.deltaTime / totalTime;
            //fromToRate += (flySpeed / distance);
            parent.transform.rotation = Quaternion.LerpUnclamped(fromRotation, toRotation, fromToRate);
            //player.transform.position = Vector3.MoveTowards(player.transform.position, destination, flySpeed * Time.deltaTime);
            parent.transform.position =Vector3.LerpUnclamped(fromPosition, destination, fromToRate);
            //player.transform.position += player.transform.rotation * Vector3.up * distance * 0.02f *(Mathf.Sin((t / distance) * Mathf.PI* 2));
            //t += Time.deltaTime * flySpeed;
            if (fromToRate > 0.6f)
            {
                MoveController.instance.animator.SetFloat("JumpSpeed", 1);
                MoveController.instance.resetVelocity();
            }
            yield return null;
        }
       // Debug.Log("firstRotateFin");
        /*while (Quaternion.Angle(player.transform.rotation, toRotation) != 0 && MoveController.instance.currentGruond == groundT.gameObject)
        {
            fromToRate += (flySpeed / distance);
            player.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, fromToRate);
            yield return null;
        }*/
        MoveController.instance.resetVelocity();
        //parent.transform.rotation = player.transform.rotation;
       // ChangeParent(parent.transform);

        //CamCtrl.instance.resetHeading();
        //MoveController.instance.UpdateDeltaQ();
        //parent.transform.SetParent(MoveController.instance.currentGruond.transform);
        //Debug.Log("Grounded!");
        StateManager.isFlying = false;
        //Debug.Log("FlyFin");
    }

    public IEnumerator ChangeGroundOnWalk(Transform groundT,Vector3 normal)
    {

        //StartCoroutine(MoveController.instance.Jump(Vector3.Distance(MoveController.instance.transform.position, destination)*0.05f));
       /* Debug.Log("PlayerUp: " + player.transform.up);
        Debug.Log("GroundUp" + groundT.transform.up);
        float tx = player.transform.up.x - groundT.transform.up.x;
        float ty = player.transform.up.y - groundT.transform.up.y;
        float tz = player.transform.up.z - groundT.transform.up.z;
        Debug.Log("subractResult: " + tx + "," + ty + "," + tz);
        */


        StateManager.isGroundChanging = true;
        yield return new WaitForSeconds(0.01f);

        MoveController.instance.currentGruond = groundT.gameObject;
        //yield return new WaitForSecondsRealtime(0.4f);
        //ChangeParent(parent.transform.parent);
        //parent.transform.parent = groundT.parent;
        Quaternion fromRotation = parent.transform.rotation;
        Quaternion toRotation = Quaternion.AngleAxis(Vector3.Angle(parent.transform.up.normalized, normal.normalized), Vector3.Cross(parent.transform.up.normalized, normal.normalized)) * fromRotation;

        //cam.transform.SetParent(player.transform);
        //CamCtrl.instance.resetHeading();
        float fromToRate = 0;
        float totalTime = 0.3f;
        while (fromToRate < 1 && MoveController.instance.currentGruond == (groundT.gameObject))
        {
            //fromRotation = player.transform.rotation;
            parent.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, fromToRate);
            fromToRate += Time.deltaTime / totalTime;
            yield return null;
        }
        if (MoveController.instance.currentGruond == (groundT.gameObject))
        {
           // parent.transform.rotation = player.transform.rotation;
            //ChangeParent(parent.transform);
           // MoveController.instance.UpdateDeltaQ();
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
