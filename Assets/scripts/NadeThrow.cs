using UnityEngine;
using System.Collections;

public class NadeThrow : MonoBehaviour {

	public GameObject nade_pre; // requires assignment

	// const
	public float maxThrowStrength = 30f;
	public float minThrowStrength = 5f;
	public float maxThrowPrepTime = 2.5f;	
	public float carryDistance = 1.2f;
	public float smooth 	= 5f;
	public float grabRange 	= 4f;

	// refs
	private Camera cam;

	// vars
	private GameObject held_nade;
	private float throwPrepTime = 0f;
	private bool is_nade_held;
	private bool is_throw_started;
	
	// Use this for initialization
	void Start () {
		cam = Camera.main;
		// so we know where we're aiming we need the camera
	}

	void GetNade(){
		// pull a new nade from the ether
		if( is_nade_held ){
			Debug.Log("tried to get nade when already holding one.");
			return;
		}
		GameObject new_nade = (GameObject)Instantiate(nade_pre, cam.transform.position + cam.transform.forward * carryDistance, cam.transform.rotation);
		// do all the same assignments described below in CheckNadePickup()
		held_nade = new_nade;
		held_nade.rigidbody.isKinematic = true;
		is_nade_held = true;
		NadeLogic nl = held_nade.GetComponent<NadeLogic>();
		nl.is_held = true;
		nl.nadethrowcomponent = this;
	}

	void CarryHeldNade(){
		// causes the nade to be dragged in front of the camera. kinda jerky when the player is moving tho
		held_nade.transform.position = Vector3.Lerp (
			held_nade.transform.position,
			cam.transform.position + cam.transform.forward * carryDistance, Time.deltaTime * smooth);
	}

	void PullHeldPin(){
		if (!is_nade_held) {
			Debug.Log("tried to pull pin with no held nade.");
			return;
		}
		Debug.Log ("pulling pin on held nade... gulp...");
		((NadeLogic)held_nade.GetComponent("NadeLogic")).Pull_Pin();
	}

	void StartThrow(){
		if (is_throw_started) {
			Debug.Log ("tried to throw when already throwing.");
			return;
		}
		throwPrepTime = 0;
		is_throw_started = true;
	}

	void CancelThrow(){
		is_throw_started = false;
		throwPrepTime = 0;
	}

	public void TossHeldNade(float throwForce, Vector3 throwTorque = new Vector3()){
		if (!is_nade_held) {
			Debug.Log ("tried to throw nade when none held.");
			return;
		}
		Debug.Log ("Throwing with strength "+throwForce);

		// undo everything commented about in CheckNadePickup()
		is_nade_held = false;
		held_nade.rigidbody.isKinematic = false;

		// also give it the throw forces
		held_nade.rigidbody.AddForce( cam.transform.forward*throwForce, ForceMode.Impulse );
		held_nade.rigidbody.AddTorque( throwTorque );

		NadeLogic nl = held_nade.GetComponent<NadeLogic>();
		nl.is_held = false;
		held_nade = null;
	}

	void CheckNadePickup(){
		if (Input.GetButtonDown ("Grab")) {
			// attempt to grab a nade the camera is pointing at
			int x = Screen.width / 2;
			int y = Screen.height / 2;
			// raycast out from center of screen
			Ray ray = cam.ScreenPointToRay(new Vector3(x,y));
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, grabRange)) {
				Debug.Log ("Raycast hit");
				NadeLogic p = hit.collider.GetComponent<NadeLogic>();
				// if we hit something and it has a NadeLogic component...
				if(p != null) {
					Debug.Log ("Raycast hit is a nade");
					// make it what we hold
					held_nade = p.gameObject;
					// make sure it knows it's held
					p.is_held = true;
					// make sure it knows what's holding it
					p.nadethrowcomponent = this;
					// allow us to drag it around without gravity snatching it
					held_nade.rigidbody.isKinematic = true;
					// keep track of the fact that we are now holding something
					is_nade_held = true;
				}
			}
		}
	}

	float GetThrowStrength( float prepTime ){
		float maybe = minThrowStrength + ((maxThrowStrength - minThrowStrength) * prepTime / maxThrowPrepTime);
		// strength of throw is based on time mouse is held down, with a maximum time and force
		if (maybe > maxThrowStrength) {
			return maxThrowStrength;
		}
		return maybe;
	}
	

	void Update(){
		// make sure nade is held in front of camera
		if (is_nade_held) {
			CarryHeldNade ();

			if(is_throw_started){
				throwPrepTime += Time.fixedDeltaTime;
				if(Input.GetButtonDown ("Cancel Throw") ){
					CancelThrow();
				}
				if(Input.GetButtonUp ("Throw")){
					Debug.Log (":"+throwPrepTime+" of throw prep time");
					TossHeldNade(GetThrowStrength(throwPrepTime));
					CancelThrow();
				}
			}
			else if( Input.GetButtonDown ("Throw") ){
				Debug.Log ("Started to throw...");
				StartThrow();
			}
		
			if( Input.GetButtonDown ("Pull Pin") ){
				PullHeldPin();
			}
			if( Input.GetButtonDown("Drop") ){
				CancelThrow();
				TossHeldNade(0);
			}
		} else {
			CheckNadePickup();
			if(Input.GetButtonDown("Get Nade")){
				GetNade();
			}
		}
	}













}
