using UnityEngine;
using System.Collections;

public class NadePin : MonoBehaviour {

	public	GameObject stuck_in_nade;
	public	float despawn_wait = 1f;
	public	float despawn_timer; // timer default val is zero, despawns when timer < negative wait.
	
	public AudioClip pin_pull_sound;

	private float pitch_low 	= 0.75f;
	private float pitch_high 	= 1.25f;
	
	private float pin_volume_low	= 0.1f;
	private float pin_volume_high	= 0.2f;

	public void Internal_Play_Sound(AudioClip sound){
		if(this.audio && sound){
			this.audio.pitch = Random.Range (pitch_low, pitch_high);
			float vol = Random.Range(pin_volume_low, pin_volume_high);
			this.audio.PlayOneShot(sound, vol);
		}
		else{
			Debug.Log("Pin failed to play sound.");
		}
	}

	// Use this for initialization
	void Start () {
		if (!stuck_in_nade) {
			if(transform.parent.GetComponent<NadeLogic>()){
				stuck_in_nade = transform.parent.gameObject;
				Debug.Log ("Pin initialized, nade reference set to parent");
			}
			else{
				Debug.Log ("Pin initialized without nade reference, will despawn");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!stuck_in_nade) {
			despawn_timer -= Time.fixedDeltaTime;
			if(despawn_timer < -despawn_wait){
				Destroy(this.gameObject);
			}
		}
		else{
			if(despawn_timer != 0){
				despawn_timer = 0;
			}
		}
	}
}
