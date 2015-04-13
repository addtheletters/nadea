using UnityEngine;
using System.Collections;

public class SampleTeleGrab : MonoBehaviour {
	public float mCorrectionForce = 50.0f;
	public float mPointDistance = 3.0f;

	public GameObject heldObject;
	
	void FixedUpdate()
	{
		if(heldObject.GetComponent<Rigidbody>().constraints != RigidbodyConstraints.FreezeRotation)
			heldObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
		if(heldObject.GetComponent<Rigidbody>().useGravity)
			heldObject.GetComponent<Rigidbody>().useGravity = false;
		
		Vector3 targetPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		targetPoint += Camera.main.transform.forward * mPointDistance;
		Vector3 force = targetPoint - heldObject.transform.position;
		
		heldObject.GetComponent<Rigidbody>().velocity = force.normalized * heldObject.GetComponent<Rigidbody>().velocity.magnitude;
		heldObject.GetComponent<Rigidbody>().AddForce(force * mCorrectionForce);
		
		heldObject.GetComponent<Rigidbody>().velocity *= Mathf.Min(1.0f, force.magnitude / 2);
	}
}
