using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour {

    public float rotateSpeed = 10;
    //Rigidbody rgb;
	// Use this for initialization
	void Start () {
        //rgb = GetComponent<Rigidbody>();
        StartCoroutine(DoLogic());
	}
	
    IEnumerator DoLogic()
    {
        while(true)
        {
            transform.Rotate(0,rotateSpeed * Time.deltaTime,0);
            //rgb.AddTorque(Vector3.up * rotateSpeed);
            Debug.DrawRay(transform.position, transform.up.normalized * 10, Color.red);
            yield return new WaitForFixedUpdate();
        }
    }
}
