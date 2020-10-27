using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour
{
	
	public Text speedText;
	
	public void SetTimeScale(float s) {
		Time.timeScale=s;
		speedText.text = ("Speed: "+s.ToString("0.000"));
	}
	

	

}
