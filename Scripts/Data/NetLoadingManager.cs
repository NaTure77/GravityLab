using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetLoadingManager : NetworkBehaviour {

    public GameObject canvas;
    public GameObject player;
	// Use this for initialization
	void Start () {
        GameObject p = Instantiate(player,this.transform);
        if (isLocalPlayer)
        {
            gameObject.AddComponent<StateManager>();
            GameObject c = Instantiate(canvas,this.transform);
            p.AddComponent<MoveController>();
            c.AddComponent<StageUI>();
            //p.GetComponentInChildren<CamCtrl>().isLocalPlayer = true;
        }

		
	}
}
