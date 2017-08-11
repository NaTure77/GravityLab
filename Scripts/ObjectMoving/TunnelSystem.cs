using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TunnelSystem : MonoBehaviour {

    public Transform A;
    public Transform B;
    private Transform destination;

    private void Start()
    {
        destination = A;
    }
    IEnumerator MoveCar(Transform car)
    {
        Vector3 fromPosition = car.position;
        float fromToRate = 0;
        float totalTime = 10;
        while (fromToRate < 1)
        {
            car.position = Vector3.LerpUnclamped(fromPosition, destination.position, fromToRate);
            fromToRate += Time.deltaTime / totalTime;
            yield return null;
        }
        if (destination.Equals(A)) destination = B;
        else destination = A;
    }
    public void StartMove(Transform car)
    {
        MoveController.instance.transform.SetParent(car);
        StartCoroutine("MoveCar", car);
    }
    public void StartMoveLeft(Transform car)
    {
        StopCoroutine("MoveCar");
        destination = A;
        StartMove(car);
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void StartMoveRight(Transform car)
    {
        StopCoroutine("MoveCar");
        destination = B;
        StartMove(car);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void StopMove()
    {
        MoveController.instance.transform.SetParent(null);
        StopCoroutine("MoveCar");
        EventSystem.current.SetSelectedGameObject(null);
    }
}
