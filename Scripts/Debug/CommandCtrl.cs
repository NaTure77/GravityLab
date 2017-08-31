using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CommandCtrl : MonoBehaviour {

    public List<Transform> Seg;
    public GameObject cmdLine;
    InputField inputField;
    public static CommandCtrl instance;
    private void Start()
    {
        
    }
    private void Awake()
    {
        instance = this;
        inputField = cmdLine.GetComponentInChildren<InputField>();
    }

    public void ToggleCmd()
    {
        cmdLine.SetActive(!cmdLine.activeSelf);
    }
    
    public void InputCmd(Text command)
    {
        string doLine = command.text.Substring(0, command.text.IndexOf(" "));
        int numLine = int.Parse(command.text.Substring(command.text.IndexOf(" ")));
        Debug.Log(doLine);
        Debug.Log(numLine);

        DoCmd(doLine, numLine);
    }

    private void DoCmd(string command,int segNum)
    {
        StartCoroutine(command,segNum);
    }

    IEnumerator moveTo(int segNum)
    {
        if (segNum < Seg.Count)
        {
            MoveController.instance.transform.position = Seg[segNum].position;
            MoveController.instance.transform.rotation = Seg[segNum].rotation;
        }
        yield return null;
    }

    IEnumerator moveSpeed(int speed)
    {
        MoveController.instance.runSpeed = speed;
        yield return null;
    }

    IEnumerator rayDistance(int dist)
    {
        RayCastManager.instance.maxDistance = dist;
        yield return null;
    }
}
