using UnityEngine;
using System.Collections;

public class ScoreObject : MonoBehaviour {

	// refs
	public GameObject scorekeeper; // required

	// settings
	public float score_to_add = 100f;
	public float multiplier_bonus = 0.1f;
	public bool kill_on_trigger = false;
	public bool use_reset_time = true;

	public float reset_max_time = 3f;

	public bool scoreTriggerable= false;
	private float reset_timer 	= 0f;




	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (reset_timer < 0) {
			scoreTriggerable = true;
			reset_timer = 0;
		}
		else{
			reset_timer -= Time.deltaTime;
		}
	}

	public void TriggerScoreObject(){
		if (!scorekeeper) {
			Debug.Log("No scorekeeper assigned. Scoring fail.");
			return;
		}

		if (!scoreTriggerable) {
			Debug.Log("Score object currently not triggerable.");
			return;
		}

		Scorer sc = scorekeeper.GetComponent<Scorer> ();
		sc.AddMultipliedScore (this.score_to_add);
		sc.AddToMultiplier (this.multiplier_bonus);

		if (kill_on_trigger) {
			Destroy (this.gameObject);
		} else {
			if(use_reset_time){
				reset_timer = reset_max_time;
				scoreTriggerable = false;
			}
		}
	}


	
}
