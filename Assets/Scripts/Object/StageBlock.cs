using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageBlock : MonoBehaviour {

    public int stageNum;
    public GameObject beforeStage_in;
    public GameObject beforeStage_out;

    public GameObject AfterStage_in;
    public GameObject AfterStage_out;
    public GameObject blockingBlock;
    bool isNotified = false;
	void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player") && !isNotified)
        {
            StateManager.currentPosition = collision.transform.position;
            StateManager.currentRotation = collision.transform.rotation;
            StartCoroutine(StageUI.instance.ShowStageClear(stageNum - 1));
            isNotified = true;
            SetNextStage();
            
        }
    }

    void SetNextStage()
    {
        blockingBlock.SetActive(true);
        beforeStage_in.SetActive(false);
        beforeStage_out.SetActive(false);
        AfterStage_in.SetActive(true);
        AfterStage_out.SetActive(true);
    }
}
