using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class RotateAroundPlanet : MonoBehaviour {

    public GameObject planet;
    public float MaxSpeed = 5;
    private float speedX = 0;
    private float speedY = 0;
    public Scrollbar H_Slider;
    public Scrollbar V_Slider;
    public Text H_Text;
    public Text V_Text;
    public Text BSDistance; // distance between BalckHole and Satellite
    public Transform blackHole;
    public void StartLogic()
    {
        StartCoroutine("RotateXAxis");
        StartCoroutine("RotateYAxis");
        StartCoroutine("GetValue");
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void StopLogic()
    {
        StopCoroutine("RotateXAxis");
        StopCoroutine("RotateYAxis");
        StopCoroutine("GetValue");
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void resetValue()
    {
        V_Slider.value = 0.5f;
        H_Slider.value = 0.5f;
        speedX = 0;
        speedY = 0;
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void ShowValue()
    {
        H_Text.text = (Mathf.Round(speedX * 100) / 100).ToString();
        V_Text.text = (Mathf.Round(speedY * 100) / 100).ToString();
        BSDistance.text = ((int)Vector3.Distance(transform.position, blackHole.position)).ToString() + "m";
    }
    IEnumerator GetValue()
    {
        while(true)
        {
            speedY = (V_Slider.value - 0.5f) * MaxSpeed;
            speedX = (H_Slider.value - 0.5f) * MaxSpeed;
            ShowValue();
            yield return null;
        }
    }
    IEnumerator RotateYAxis()
    {
        while(true)
        {
            this.transform.RotateAround(planet.transform.position, transform.forward, speedY);
            yield return null;
        }
    }

    IEnumerator RotateXAxis()
    {
        while (true)
        {
            this.transform.RotateAround(planet.transform.position, transform.right, speedX);
            yield return null;
        }
    }
}
