using UnityEngine;
using System.Collections;

public class NadeThrow : MonoBehaviour {

	// make carrying dependant on forces, not direct position / nonkinematic

	// physical pin
	// physical trigger lever?

	// resetting targets
	// scoreboard referenced by their prefab

	public GameObject[] nade_pres; // requires assignment

	// const
	public float maxThrowStrength = 25f;
	public float minThrowStrength = 1f;
	public float maxThrowPrepTime = 1.5f;	
	public float standardCarryDistance = 1.2f;
	public float smooth 		= 5f;
	public float grabRange 	= 4f;

	// refs
	private GameObject cam;
	// need something to handle Oculus Rift position and rotation
	// private GameObject cam? cam_representation
	// all it needs is cam.transform.position
	// and cam.transform.rotation
	// and cam.transform.forward
	// for the purpose of Quaternion.LookRotation(...);

	// vars
	private int selected_nade = 1;
	private GameObject held_nade;
	private float throwPrepTime = 0f;
	private bool is_nade_held;
	private bool is_throw_started;

	// for gui
	private string[] toolbarStrings = {"Base", "Normal", "Cylinder", "Red Light", "Smoke"};

	// for audio
	public AudioClip woosh_sound;
	private float woosh_sound_threshold = 5f;
	private float woosh_pitch_scale = 0.1f;
	private float woosh_volume_low	= 0.15f;
	private float woosh_volume_high	= 0.2f;


	// Use this for initialization
	void Start () {
		if (Camera.main) {
			cam = (GameObject)Camera.main.gameObject;
		}
		// so we know where we're aiming we need the camera

		if (!cam) {
			Debug.Log("No main camera. Oculus Rift?");
			cam = GetOculusCamObject();
			Debug.Log(cam);
		}

		if (!cam) {
			Debug.Log("No camera or oculus camera equivalent found! Will break. :(");
		}

	}

	GameObject GetOculusCamObject(){
		return transform.Find ("ForwardDirection").gameObject;
	}

	void PickUpNade( GameObject nade ){
		// make it what we hold
		held_nade = nade;
		// allow us to drag it around without gravity snatching it
		//held_nade.rigidbody.isKinematic = true;
		// should be replaced soon by
		held_nade.GetComponent<Rigidbody>().useGravity = false;
		
		// keep track of the fact that we are now holding something
		is_nade_held = true;


		NadeLogic nl = nade.GetComponent<NadeLogic> ();
		if (!nl) {
			return;
		}
		// make sure it knows it's held
		nl.is_held = true;
		// make sure it knows what's holding it
		nl.nadethrowcomponent = this;


	}

	void DropNade( ){

		string heldID = GetHeldID ();

		// undo everything commented about in CheckNadePickup()
		is_nade_held = false;
		//held_nade.rigidbody.isKinematic = false;

		if(!heldID.Equals("fatcube")){
			held_nade.GetComponent<Rigidbody>().useGravity = true;
		}

		NadeLogic nl = held_nade.GetComponent<NadeLogic>();
		if(nl){
			nl.is_held = false;
		}
		held_nade = null;
	}

	void GetNade(){
		// pull a new nade from the ether
		if( is_nade_held ){
			Debug.Log("tried to get nade when already holding one.");
			return;
		}
		GameObject new_nade = (GameObject)Instantiate(nade_pres[selected_nade],
		                                              cam.transform.position + cam.transform.forward * GetCarryDistance(),
		                                              cam.transform.rotation);
		PickUpNade (new_nade);
	}

	void CarryHeldNade(){
		// does not require nade to become kinematic
		// nade should collide properly
		// should rely on forces / velocities / torques rather than setting values

		string carryid = GetHeldID();

		// position
		Vector3 intended_position = cam.transform.position + cam.transform.forward * GetCarryDistance(carryid);
		Vector3 delta_pos = intended_position - held_nade.transform.position;
		//Debug.Log("Delta pos for held nade: " + delta_pos);
		held_nade.GetComponent<Rigidbody>().velocity = GetComponent<CharacterController>().velocity + delta_pos.normalized * held_nade.GetComponent<Rigidbody>().velocity.magnitude;
		// makes it go in the direction of the grab point at the speed of before. change so that cannot spontaneously change dir
		held_nade.GetComponent<Rigidbody>().AddForce(delta_pos * 50, ForceMode.Acceleration);
		// Debug.Log ("vscale "+Mathf.Min(1.0f, delta_pos.magnitude ));
		held_nade.GetComponent<Rigidbody>().velocity *= Mathf.Min(1.0f, delta_pos.magnitude);

		// rotation
		held_nade.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		held_nade.transform.rotation = Quaternion.Lerp( 
               held_nade.transform.rotation,
               Quaternion.LookRotation (cam.transform.forward), Time.fixedDeltaTime * smooth);
	}

	void PullHeldPin(){
		if (!is_nade_held) {
			Debug.Log("tried to pull pin with no held nade.");
			return;
		}
		Debug.Log ("pulling pin on held nade... gulp...");
		NadeLogic nl = held_nade.GetComponent<NadeLogic>();
		if (!nl) {
			Debug.Log ("held object was not a nade");
			return;
		}
		nl.PullPin();
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

		// play woosh sound
		if( throwForce > woosh_sound_threshold ){
			InternalPlaySound(woosh_sound, 
			                    throwForce * woosh_pitch_scale,
			                    throwForce * woosh_pitch_scale,
			                    woosh_volume_low,
			                    woosh_volume_high);
		}

		// need temp var because drop clears internal reference
		GameObject tossed_nade = held_nade;

		DropNade ();
		// also give it the throw forces
		tossed_nade.GetComponent<Rigidbody>().AddForce( cam.transform.forward*throwForce, ForceMode.Impulse );
		tossed_nade.GetComponent<Rigidbody>().AddTorque( throwTorque ); // maybe should be AddRelativeTorque for easier calculation

	}

	public void InternalPlaySound(AudioClip sound, float plow, float phigh, float vlow, float vhigh){
		if(this.GetComponent<AudioSource>() && sound){
			this.GetComponent<AudioSource>().pitch = Random.Range (plow, phigh);
			float vol = Random.Range(vlow, vhigh);
			this.GetComponent<AudioSource>().PlayOneShot(sound, vol);
		}
		else{
			Debug.Log("Thrower failed to play sound.");
		}
	}

	void CheckNadePickup(){
		if (Input.GetButtonDown ("Grab")) {
			// attempt to grab a nade the camera is pointing at
			//int x = Screen.width / 2;
			//int y = Screen.height / 2;
			// raycast out from center of screen
			//Debug.Log("pos:"+cam.transform.position);
			//Debug.Log("tra:"+cam.transform.forward);
			Ray ray = new Ray(cam.transform.position, cam.transform.forward);//cam.ScreenPointToRay(new Vector3(x,y));
			RaycastHit hit;

			if(Physics.Raycast(ray, out hit, grabRange)) {
				//Debug.Log ("Raycast hit" + hit);
				NadeLogic p = hit.collider.GetComponent<NadeLogic>();
				if(p == null){
					// could be a grenade pin?
					p = hit.collider.GetComponentInParent<NadeLogic>();
				}
				// if we hit something and it has a NadeLogic component...
				if(p != null) {
					//Debug.Log ("Raycast hit is a nade");
					// make it what we hold
					PickUpNade(p.gameObject);
					return;
				}

				Pickupable pu = hit.collider.GetComponent<Pickupable>();
				if(pu != null){
					PickUpNade(pu.gameObject);
					return;
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

	Vector3 GetThrowTorque( float prepTime ){
		// depends on grenade type tho
		// maybe also depends on type of throw. toss?
		string nadetype = GetHeldID();
		if (nadetype.Equals("stick")) {
			// end over end forwards
		} else if (nadetype.Equals("ball")) {
			// like a righthand overhand, vector pointing up and left (or is it down and right)? 
		} else {
			// uhh something...
		}
		return Random.onUnitSphere * (GetThrowStrength (prepTime) * (Random.value * 0.3f + 0.7f)) * 5;
	}

	float GetCarryDistance( string identifier = "" ){
		if(identifier.Equals("fatcube")){
			return 3*standardCarryDistance;
		}
		return standardCarryDistance;
	}

	void CheckNadeSelect(){
		// check for input to see if selected nade changes (number keys)
		for (int i = 0; i < (nade_pres.Length + 1); i++) {
			if (Input.GetButtonDown ("Select Nade "+i)) {
				Debug.Log ("selecting nade "+i);
				selected_nade = i-1;
				if(selected_nade < 0){
					selected_nade = nade_pres.Length-1;
				}
			}
		}
	}

	string GetHeldID(){
		NadeLogic nl = held_nade.GetComponent<NadeLogic> ();
		string identifier;
		if (nl) {
			identifier = nl.nade_type;
			return identifier;
		}
		else{
			identifier = "not nade";
		}
		Pickupable pu = held_nade.GetComponent<Pickupable> ();
		if (pu) {
			identifier = "fatcube";
			return identifier;
		}
		return identifier;
	}


	void OnGUI () {
		// nade selector
		selected_nade = GUI.Toolbar (new Rect (25, 25, 70*(nade_pres.Length+1) , 30), selected_nade, toolbarStrings);

		// throw strength
		if (is_throw_started) {
			LowerLeftGUIBox(0, "Throw Strength: " + Mathf.Round (100 * (GetThrowStrength (throwPrepTime) / maxThrowStrength)) + "%");
		}

		// held nade pin pulled
		if (is_nade_held) {
			string message;
			NadeLogic nl = held_nade.GetComponent<NadeLogic>();
			if(!nl){
				message = "Held object is not a nade.";
			}else{
				if( nl.pin_pulled ){
					message = "PIN IS PULLED!";
					LowerLeftGUIBox(2, "Fuse time: " + Mathf.Round (nl.fuse_time));
				}
				else{
					message = "Pin is in place.";
					if( nl.fuse_lit ){
						LowerLeftGUIBox(2, "Fuse seems to be burning...");
					}
				}
			}
			LowerLeftGUIBox(1, message);
		}

	}

	void LowerLeftGUIBox(int index, string message){	
		GUI.Box (new Rect (25, Screen.height - (60+(index * (30 + 5))), 300, 30), message);
	}


	void Update(){
		// make sure nade is held in front of camera
		CheckNadeSelect ();

		if (is_nade_held) {
			CarryHeldNade ();

			if(is_throw_started){
				throwPrepTime += Time.deltaTime;
				if(Input.GetButtonDown ("Cancel Throw") ){
					CancelThrow();
				}
				if(Input.GetButtonUp ("Throw")){
					Debug.Log (":"+throwPrepTime+" of throw prep time");
					TossHeldNade(GetThrowStrength(throwPrepTime), GetThrowTorque(throwPrepTime));
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
