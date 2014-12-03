using UnityEngine;
using System.Collections;

public class Scorer : MonoBehaviour {

	public int total_score		 = 0;

	public int sub_total		= 0;
	public float sub_multiplier = 1f;

	public	float combo_time_limit  = 10f;
	private float combo_timer 		= 0f;

	// distance from topright corner
	private float scorebox_xspace = 25;
	private float scorebox_yspace = 25;

	private float scorebox_width = 300;
	private float scorebox_height = 130;


	// Update is called once per frame
	void Update () {
		if (sub_total != 0) {
			if(combo_timer > 0){
				combo_timer -= Time.deltaTime;
			}
			else{
				FinishCombo();
			}
		}
	}

	void OnGUI(){
		string message = "<size=12><b>Score:</b></size>\n";
		message += "<size=30>" + total_score + "</size>\n\n";
		message += "<size=17>Subtotal: <b><color=cyan>" + sub_total + "</color></b></size>\n";
		message += "<size=14>Multiplier: <b><color=orange>" + GetDisplayMultiplier()  + "x</color></b></size>\n";
		if (combo_timer > 0) {
			message += "<size=13>Combo expires in: <b>"+ GetDisplayComboTime()  +" seconds!</b></size>";
		}
		GUI.Box (new Rect(
			Screen.width - scorebox_width - scorebox_xspace,
			scorebox_yspace,
			scorebox_width,
			scorebox_height), message);
	}

	public string GetDisplayComboTime(){
		return (Mathf.Floor((combo_timer)*10) / 10).ToString("0.0");
	}

	public string GetDisplayMultiplier(){
		return (Mathf.Floor ((sub_multiplier) * 10) / 10).ToString ("0.0");
	}

	public void AddScoreToCombo( int base_score ){
		sub_total += base_score;
	}

	public void AddToComboMultiplier( float added_mult ){
		sub_multiplier += added_mult;
	}

	public void RefreshComboTime(){
		combo_timer = combo_time_limit;
	}

	public void AddRawScore( int base_score ){
		total_score += base_score;
	}

	public void ResetCombo(){
		sub_total = 0;
		sub_multiplier = 1f;
		combo_timer = 0f;
	}

	public void FinishCombo(){
		total_score += Mathf.FloorToInt( sub_total * sub_multiplier );
		ResetCombo();
	}
}
