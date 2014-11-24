using UnityEngine;
using System.Collections;

public class Scorer : MonoBehaviour {

	public int total_score		 = 0;
	public float incoming_multiplier = 1.0f;

	public float mult_expiration_lim = 5f;
	private float mult_expiration_timer = 0f;

	// distance from topright corner
	private float scorebox_xspace = 25;
	private float scorebox_yspace = 25;

	private float scorebox_width = 300;
	private float scorebox_height = 70;


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
		GUI.Box (new Rect(
			Screen.width - scorebox_width - scorebox_xspace,
			scorebox_yspace,
			scorebox_width,
			scorebox_height), "<size=10><b>Score:</b></size>\n<size=30>" + total_score + "</size>");
	}

	public void AddMultipliedScore( float base_score ){
		total_score += Mathf.FloorToInt(base_score * incoming_multiplier);
	}

	public void AddToMultiplier( float added_mult ){
		incoming_multiplier += added_mult;
	}

	public void AddRawScore( int base_score ){
		total_score += base_score;
	}
}
