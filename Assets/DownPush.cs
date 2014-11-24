using UnityEngine;
using System.Collections;

public class DownPush : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.rigidbody.AddForce(Vector3.down * 10 * Time.deltaTime);
	}
}
