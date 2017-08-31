using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MovingSystem : MonoBehaviour {

    public Transform A;
    public Transform B;
    private Transform destination;
    public Transform car;
    public GameObject blockingWall;
    public GameObject startButton;
    private void Start()
    {
        destination = A;
    }
    IEnumerator MoveCar()
    {
        //blockingWall.SetActive(true);
        startButton.SetActive(false);
        Vector3 fromPosition = car.position;
        Quaternion fromRotation = car.rotation;
        Quaternion toRotation = destination.rotation;
        float fromToRate = 0;
        //float fromToRateRotate = 0;
        float totalTime = 10;
        MoveController.instance.transform.SetParent(car);
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

        //blockingWall.SetActive(destination.Equals(A));
        if (destination.Equals(B)) MoveController.instance.transform.SetParent(null);

        destination = destination.Equals(A) ? B : A;
        startButton.SetActive(true);
    }
    public void StartMove()
    {
        StartCoroutine(MoveCar());
    }
}
