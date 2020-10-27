using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
		if (Input.GetKeyDown(KeyCode.F11))
			FullscreenToggle();
    }
	
	public void BtnQuit() {
		Application.Quit();
	}
	
	public void FullscreenToggle() {
		Screen.fullScreen = !Screen.fullScreen;
	}
}
