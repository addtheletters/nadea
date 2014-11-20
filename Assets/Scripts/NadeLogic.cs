using UnityEngine;
using System.Collections;

public class NadeLogic : MonoBehaviour {

	// const	
	public float radius = 6.0f;
	public float power = 750.0f;
	public float lift = 1.0f;
	public Vector3 pin_pull_force	= new Vector3(0, 0, 100);
	public Vector3 pin_pull_torque	= new Vector3(100, 0, 0);

	// apparently the const keyword also makes things static though
	// readonly is the proper keyword then, I think
	// will refrain from keywords just in case these need to be modified whenever
	public string nade_type = "standard"; // affects how it is thrown

	//setup / refs	
	public NadeThrow nadethrowcomponent; // should not require assignment
	private LightController lights; 	// does not require assignment
	public GameObject splosion_prefab;	// requires assignment
	public GameObject pin;				// probably requires assignment

	// vars
	public bool is_held = false;
	public bool pin_pulled = false;
	public bool fuse_lit = false;
	public float fuse_time = 5.0f;
	private float light_timer = 0f;

	// Use this for initialization
	void Start () {
		// get light controller if there are lights attached too
		// if there are none, lights will be null, which is OK
		lights = this.gameObject.GetComponent <LightController>();
	}

	public void Light_Fuse(){
		fuse_lit = true;
	}

	public void Pull_Pin(){
		pin_pulled = true;
		if (pin) {
			if(!pin.rigidbody){
				pin.AddComponent<Rigidbody>();
			}
			pin.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			pin.rigidbody.isKinematic = false; // let the pin fall and clatter into things
			pin.rigidbody.AddRelativeForce (pin_pull_force);
			pin.rigidbody.AddRelativeTorque(pin_pull_torque);
			pin.transform.parent = null;
			pin.GetComponent<NadePin>().stuck_in_nade = null;
			pin = null;
		}
		//fuse_lit = true;
	}

	public void Restore_Pin(GameObject new_pin){
		pin_pulled = false;
		pin = new_pin;
		pin.rigidbody.isKinematic = true;
		// stuff a new pin in?
	}

	protected bool shouldLightFuse(){
		return pin_pulled;
	}

	protected bool shouldBlowup(){
		return fuse_time <= 0;
	}

	protected bool shouldFlashLights(){
		return lights && light_timer < 0;
	}

	protected void applyExplosionEffect(Collider physics_hit, Vector3 explosionPos){
		// push all rigidbodies that are hit away with explosive force
		if (physics_hit && physics_hit.rigidbody) {
			applyRigidbodyExplEffect(physics_hit.rigidbody, explosionPos);
		}

		// if what's hit is a nade
		NadeLogic othernade = physics_hit.gameObject.GetComponent<NadeLogic>();
		if(othernade){
			applyNadeExplEffect(othernade, explosionPos);

		}
	}

	protected virtual void applyRigidbodyExplEffect(Rigidbody otherRB, Vector3 explosionPos){
		otherRB.AddExplosionForce(power, explosionPos, radius, lift);
	}

	protected virtual void applyNadeExplEffect(NadeLogic othernade, Vector3 explosionPos){
		if (!othernade.fuse_lit) {		// and the pin is not pulled
			// give it a random lowered fuse time and light the fuse
			othernade.fuse_time = (othernade.fuse_time) * (Random.value * 0.75f);
			othernade.Light_Fuse();
		}
	}

	protected virtual void Fuse_Time_Down(){
		fuse_time -= Time.fixedDeltaTime;
	}

	protected virtual void Light_Time_Down(){
		light_timer -= Time.fixedDeltaTime;
	}

	protected virtual float getFullLightTimer(){
		// lights blink faster as fuse goes down
		return fuse_time / 8f;
	}

	// Update is called once per frame
	void Update () {

		if (shouldLightFuse()) {
			Light_Fuse();
		}

		if (fuse_lit) {

			Fuse_Time_Down();
			Light_Time_Down();

			// if we have lights and our light timer expires
			if( shouldFlashLights() ){
				// flash the lights and reset the timer
				lights.Instant_On();
				light_timer = getFullLightTimer();
			}
		}
		if (shouldBlowup()) {
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
			applyExplosionEffect(hit, explosionPos);
		}
		Instantiate(splosion_prefab, this.gameObject.transform.position, Random.rotation); // make a boom
		Destroy (this.gameObject); // remove this nade
	}
}
