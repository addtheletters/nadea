using UnityEngine;
using System.Collections;

public class NadeLogic : MonoBehaviour {

	public float fuse_time;
	public float radius = 5.0f;
	public float power = 10.0f;
	public float lift = 3.0f;
	public GameObject splosion_prefab;

	public bool is_held = false;
	public NadeThrow nadethrowcomponent;
	private bool pin_pulled = false;
	
	// Use this for initialization
	void Start () {

	}

	public void Pull_Pin(){
		pin_pulled = true;
	}

	public void Restore_Pin(){
		pin_pulled = false;
	}

	// Update is called once per frame
	void Update () {
		if (pin_pulled) {
			fuse_time -= Time.deltaTime;
		}

		if (fuse_time <= 0) {
			Blow_Up();
		}
	}

	public void Blow_Up(){
		if (is_held) {
			nadethrowcomponent.TossHeldNade(0);
		}
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			if (hit && hit.rigidbody)
				hit.rigidbody.AddExplosionForce(power, explosionPos, radius, lift);
			NadeLogic othernademaybe = hit.gameObject.GetComponent<NadeLogic>();
			if(othernademaybe && !othernademaybe.pin_pulled){
				othernademaybe.fuse_time = (othernademaybe.fuse_time) * (Random.value);
				othernademaybe.Pull_Pin();
			}
		}
		GameObject splosion = (GameObject)Instantiate(splosion_prefab, transform.position, new Quaternion());
		Destroy (this.gameObject);
	}
}
