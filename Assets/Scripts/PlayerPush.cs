using UnityEngine;
using System.Collections;

public class PlayerPush : MonoBehaviour {

	public float pushPower = 2.0f;

	void OnControllerColliderHit(ControllerColliderHit hit){
		Rigidbody body = hit.collider.attachedRigidbody;
		Vector3 force;
		
		// no rigidbody
		if (body == null || body.isKinematic) { return; }

		force = hit.controller.velocity * pushPower;

		body.AddForceAtPosition(force, hit.point);
	}
}
