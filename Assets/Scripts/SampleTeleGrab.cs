using UnityEngine;
using System.Collections;

public class SampleTeleGrab : MonoBehaviour {
	public float mCorrectionForce = 50.0f;
	public float mPointDistance = 3.0f;

	public GameObject heldObject;
	
	void FixedUpdate()
	{
		if(heldObject.rigidbody.constraints != RigidbodyConstraints.FreezeRotation)
			heldObject.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		if(heldObject.rigidbody.useGravity)
			heldObject.rigidbody.useGravity = false;
		
		Vector3 targetPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
		targetPoint += Camera.main.transform.forward * mPointDistance;
		Vector3 force = targetPoint - heldObject.transform.position;
		
		heldObject.rigidbody.velocity = force.normalized * heldObject.rigidbody.velocity.magnitude;
		heldObject.rigidbody.AddForce(force * mCorrectionForce);
		
		heldObject.rigidbody.velocity *= Mathf.Min(1.0f, force.magnitude / 2);
	}
}
