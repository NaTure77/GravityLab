using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TunnelSystem : MonoBehaviour {

    public Transform A;
    public Transform B;
    private Transform destination;
    public Transform floor;

    private void Start()
    {
        destination = A;
    }
    IEnumerator MoveCar(Transform car)
    {
        Vector3 fromPosition = car.position;
        Quaternion fromRotation = car.rotation;
        Quaternion toRotation = destination.rotation;
        float fromToRate = 0;
        float fromToRateRotate = 0;
        float totalTime = 10;
        MoveController.instance.transform.SetParent(floor);
        while (fromToRate < 1)
        {
            car.position = Vector3.LerpUnclamped(fromPosition, destination.position, fromToRate);
            car.rotation = Quaternion.LerpUnclamped(fromRotation, toRotation, fromToRate);
            fromToRate += Time.deltaTime / totalTime;

            //if (fromToRateRotate <= 1)
           // {
            //    car.rotation = Quaternion.LerpUnclamped(fromRotation, toRotation, fromToRateRotate);
            //    fromToRateRotate += (Time.deltaTime / totalTime) * 2;
           // }
            yield return null;
        }
        car.position = destination.position;
        car.rotation = toRotation;
        if (destination.Equals(A)) destination = B;
        else destination = A;
    }
    public void StartMove(Transform car)
    {
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
