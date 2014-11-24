using UnityEngine;
using System.Collections;

public class Scorer : MonoBehaviour {

	public int total_score		 = 0;
	public float incoming_multiplier = 1.0f;

	public float mult_expiration_lim = 8f;
	private float mult_expiration_timer = 0f;

	// distance from topright corner
	private float scorebox_xspace = 25;
	private float scorebox_yspace = 25;

	private float scorebox_width = 300;
	private float scorebox_height = 100;


	// Update is called once per frame
	void Update () {
		if (incoming_multiplier > 1) {
			if(mult_expiration_timer < mult_expiration_lim){
				mult_expiration_timer += Time.deltaTime;
			}
			else{
				mult_expiration_timer = 0;
				incoming_multiplier = 1.0f;
			}
		}
	}

	void OnGUI(){
		string message = "<size=12><b>Score:</b></size>\n";
		message += "<size=30>" + total_score + "</size>\n\n";
		message += "<size=14>Multiplier: <b><color=orange>" + incoming_multiplier + "x</color></b></size>\n";
		if (mult_expiration_timer > 0) {
			message += "<size=12><b>"+ (Mathf.Floor((mult_expiration_lim - mult_expiration_timer)*10) / 10) +"</b></size>";
		}
		GUI.Box (new Rect(
			Screen.width - scorebox_width - scorebox_xspace,
			scorebox_yspace,
			scorebox_width,
			scorebox_height), message);
	}

	public void AddMultipliedScore( float base_score ){
		total_score += Mathf.FloorToInt(base_score * incoming_multiplier);
		mult_expiration_timer = 0;
	}

	public void AddToMultiplier( float added_mult ){
		incoming_multiplier += added_mult;
	}

	public void AddRawScore( int base_score ){
		total_score += base_score;
	}
}
