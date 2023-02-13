using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUI : UIManager{

	// Use this for initialization
	void Awake () {
        Application.targetFrameRate = 60;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        LoadUI();

    }
	
}
