using UnityEngine;
using System.Collections;

public class NadeThrow : MonoBehaviour {

	public GameObject nade_pre;
	public float throwStrength;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire1")) {
			Camera cam = Camera.main;
			GameObject new_nade = (GameObject)Instantiate(nade_pre, cam.transform.position, cam.transform.rotation);
			new_nade.rigidbody.AddForce(cam.transform.forward*throwStrength, ForceMode.Impulse);
		}
	}
}
