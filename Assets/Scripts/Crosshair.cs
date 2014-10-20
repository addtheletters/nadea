using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {

	//bad kludge crosshair

	void OnGUI(){
		GUI.Box(new Rect(Screen.width/2,Screen.height/2, 10, 10), "");
	}
}
