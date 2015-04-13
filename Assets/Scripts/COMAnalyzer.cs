using UnityEngine;
using System.Collections;

public class COMAnalyzer : MonoBehaviour {

	// requires gameobject to have rigidbody

	// Use this for initialization
	void Start () {
		Analyze (this.gameObject);
	}

	void Analyze(GameObject obj){
		Rigidbody rb = obj.GetComponent<Rigidbody>();
		Debug.Log ("rigidbody center of mass: "+rb.centerOfMass);
		Debug.Log ("transform center: "+obj.transform.position);
	}

}
