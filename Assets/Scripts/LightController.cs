using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {

	public bool is_on;
	public Color bulb_color;
	public GameObject light_bulb;
	public float bulb_dim_max_black = 190f;
	public float bulb_dim_time_mult = 1f;
	private float light_percentage = 0f;
	public Light actual_light;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.K)) {
			Instant_On();
		}
		//dim color every tick. perhaps exponential, half every tick
		light_percentage -= GetLightDim (light_percentage, Time.fixedDeltaTime);
		RefreshLight ();
		if(is_on){
			Instant_On();
		}
	}

	void RefreshLight(){
		light_bulb.GetComponent<MeshRenderer> ().material.color = bulb_color * GetColorMultiplier(light_percentage);
		actual_light.intensity = light_percentage;
		// div 255 = blac
		// div 1 = good
		// percemtage 1 = good
		// div 1+ (1-percent) * (254 or less depending on min color)
	}

	float GetColorMultiplier(float current_level){
		return 1.0f / (1 + ((1-light_percentage) * bulb_dim_max_black) );
	}

	public float GetLightDim(float current_level, float timescale){
		if (current_level < .01f) {
			return current_level;
		}
		return current_level / 2; //each (second/timemult)? light level is reduced to half
		// div 2 ^ (timescale)
	}

	public void TurnOn(){
		is_on = true;
	}

	public void TurnOff(){
		is_on = false;
	}

	public void Instant_On(){
		light_percentage = 1f;
	}
}
