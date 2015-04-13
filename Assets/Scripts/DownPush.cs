using UnityEngine;
using System.Collections;

public class DownPush : MonoBehaviour {

	public float strength = 15;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		this.GetComponent<Rigidbody>().AddForce(Vector3.down * strength * Time.deltaTime);
	}
}
