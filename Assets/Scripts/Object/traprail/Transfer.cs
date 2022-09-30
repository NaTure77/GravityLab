using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transfer : MonoBehaviour {
    public Transform fromT;
    public Transform toT;
    public GameObject blockingWall;
    public float endTime = 10;
    // Use this for initialization
    bool isMoving = false;
    bool isFinished = false;
	void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player") && !isFinished && !isMoving)
        {
            StartCoroutine("MoveCar", collision.transform);
        }
    }

    /*void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player") && isMoving)
        {
            
            StopCoroutine("MoveCar");
            transform.position = fromT.position;
            blockingWall.SetActive(false);
            collision.transform.SetParent(null);
            
            //StartCoroutine("ResetCar", collision.transform);
        }
    }*/
    IEnumerator MoveCar(Transform playerObj)
    {
        blockingWall.SetActive(true);
        isMoving = true;
        yield return new WaitForSeconds(0.6f);
        Vector3 destination = toT.position;
        Vector3 fromPosition = transform.position;
        float fromToRate = 0;
        //float fromToRateRotate = 0;
        float totalTime = endTime;
        playerObj.SetParent(this.transform);
        while (fromToRate < 1 && playerObj.parent != null)
        {
            transform.position = Vector3.LerpUnclamped(fromPosition, destination, fromToRate);
            fromToRate += Time.deltaTime / totalTime;
            yield return new WaitForFixedUpdate();
        }
        if (fromToRate < 1)
        {
            transform.position = fromT.position;
            Debug.Log("?");
        }
        else
        {
            StateManager.currentPosition = playerObj.transform.position;
            StateManager.currentRotation = playerObj.transform.rotation;

            isFinished = true;
        }
        playerObj.transform.SetParent(null);
        isMoving = false;
        blockingWall.SetActive(false);
    }

    IEnumerator ResetCar(Transform playerObj)
    {
        Vector3 destination = fromT.position;
        Vector3 fromPosition = transform.position;
        float fromToRate = 0;
        //float fromToRateRotate = 0;
        float totalTime = 5;
        while (fromToRate < 1)
        {
            transform.position = Vector3.LerpUnclamped(fromPosition, destination, fromToRate);
            fromToRate += Time.deltaTime / totalTime;

            yield return null;
        }
        transform.position = destination;
    }
}
