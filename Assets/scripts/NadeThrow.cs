using UnityEngine;
using System.Collections;

public class NadeThrow : MonoBehaviour {

	public GameObject nade_pre;
	public float maxThrowStrength = 30f;
	public float minThrowStrength = 5f;
	public float maxThrowPrepTime = 2.5f;	
	public float carryDistance = 1.2f;
	public float smooth 	= 5f;
	public float grabRange 	= 4f;

	private float throwPrepTime = 0f;
	private GameObject held_nade;
	private bool is_nade_held;
	private bool is_throw_started;

	private Camera cam;
	
	// Use this for initialization
	void Start () {
		cam = Camera.main;
	}

	void GetNade(){
		if( is_nade_held ){
			Debug.Log("tried to get nade when already holding one.");
			return;
		}
		GameObject new_nade = (GameObject)Instantiate(nade_pre, cam.transform.position + cam.transform.forward * carryDistance, cam.transform.rotation);
		held_nade = new_nade;
		held_nade.rigidbody.isKinematic = true;
		is_nade_held = true;
		NadeLogic nl = held_nade.GetComponent<NadeLogic>();
		nl.is_held = true;
		nl.nadethrowcomponent = this;
	}

	void CarryHeldNade(){
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
		is_nade_held = false;
		held_nade.rigidbody.isKinematic = false;
		held_nade.rigidbody.AddForce( cam.transform.forward*throwForce, ForceMode.Impulse );
		held_nade.rigidbody.AddTorque( throwTorque );
		NadeLogic nl = held_nade.GetComponent<NadeLogic>();
		nl.is_held = false;
		held_nade = null;
	}

	void CheckNadePickup(){
		if (Input.GetButtonDown ("Grab")) {
			Debug.Log ("Attempting to grab with raycast");
			int x = Screen.width / 2;
			int y = Screen.height / 2;
			Ray ray = cam.ScreenPointToRay(new Vector3(x,y));
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, grabRange)) {
				Debug.Log ("Raycast hit");
				NadeLogic p = hit.collider.GetComponent<NadeLogic>();
				if(p != null) {
					Debug.Log ("Raycast hit is a nade");
					held_nade = p.gameObject;
					p.is_held = true;
					p.nadethrowcomponent = this;
					held_nade.rigidbody.isKinematic = true;
					is_nade_held = true;
				}
			}
		}
	}

	float GetThrowStrength( float prepTime ){
		float maybe = minThrowStrength + ((maxThrowStrength - minThrowStrength) * prepTime / maxThrowPrepTime);
		if (maybe > maxThrowStrength) {
			return maxThrowStrength;
		}
		return maybe;
	}


	/*void OnGUI(){
		Event e = Event.current;
		if (e.isKey)
			Debug.Log("Detected key code: " + e.keyCode);
	}*/

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
