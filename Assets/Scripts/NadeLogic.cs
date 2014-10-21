using UnityEngine;
using System.Collections;

public class NadeLogic : MonoBehaviour {

	// const	
	public float radius = 8.0f;
	public float power = 1000.0f;
	public float lift = 1.0f;

	//setup / refs	
	public string nade_type = "standard"; // affects how it is thrown
	public NadeThrow nadethrowcomponent; // should not require assignment
	private LightController lights; 	// does not require assignment
	public GameObject splosion_prefab;	// requires assignment

	// vars
	public bool is_held = false;
	public bool pin_pulled = false;
	public float fuse_time = 5.0f;
	private float light_timer = 0f;

	// Use this for initialization
	void Start () {
		// get light controller if there are lights attached too
		// if there are none, lights will be null, which is OK
		lights = this.gameObject.GetComponent <LightController>();
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
			fuse_time -= Time.fixedDeltaTime;
			light_timer -= Time.fixedDeltaTime;

			// if we have lights and our light timer expires
			if( lights && light_timer < 0 ){
				// flash the lights and reset the timer
				lights.Instant_On();
				// lights blink faster as fuse goes down
				light_timer = fuse_time / 8f;
			}
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
			// push all rigidbodies that are hit away with explosive force

			// if what's hit is a nade
			NadeLogic othernademaybe = hit.gameObject.GetComponent<NadeLogic>();
			// and the pin is not pulled
			if(othernademaybe && !othernademaybe.pin_pulled){
				// give it a random lowered fuse time and pull the pin
				othernademaybe.fuse_time = (othernademaybe.fuse_time) * (Random.value);
				othernademaybe.Pull_Pin();
			}
		}
		Instantiate(splosion_prefab, this.gameObject.transform.position, Random.rotation); // make a boom
		Destroy (this.gameObject); // remove this nade
	}
}
