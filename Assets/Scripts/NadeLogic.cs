using UnityEngine;
using System.Collections;

public class NadeLogic : MonoBehaviour {

	public float fuse_time = 5.0f;
	public float radius = 8.0f;
	public float power = 1000.0f;
	public float lift = 1.0f;
	public GameObject splosion_prefab;

	public bool is_held = false;

	public NadeThrow nadethrowcomponent; // should not require assignment
	private bool pin_pulled = false;
	private LightController lights;
	private float light_timer = 0f;

	// Use this for initialization
	void Start () {
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
			if( lights && light_timer < 0 ){
				lights.Instant_On();
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
			NadeLogic othernademaybe = hit.gameObject.GetComponent<NadeLogic>();
			if(othernademaybe && !othernademaybe.pin_pulled){
				othernademaybe.fuse_time = (othernademaybe.fuse_time) * (Random.value);
				othernademaybe.Pull_Pin();
			}
		}
		Instantiate(splosion_prefab, transform.position, Random.rotation);
		Destroy (this.gameObject);
	}
}
