using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {

	public bool is_on;
	public Color bulb_color;
	public GameObject light_bulb;
	public float bulb_dim_max_black = .2f;
	public float bulb_dim_time_mult = 10f;
	private float light_percentage = 0f;
	public Light actual_light;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//dim color every tick. perhaps exponential
		light_percentage -= GetLightDim (light_percentage, Time.fixedDeltaTime);
		RefreshLight ();
		if(is_on){
			Instant_On();
		}
	}

	void RefreshLight(){
		//Debug.Log (bulb_color * GetColorMultiplier(light_percentage));
		light_bulb.GetComponent<MeshRenderer> ().material.color = bulb_color * GetColorMultiplier(light_percentage);
		actual_light.intensity = light_percentage;
		// div 255 = blac
		// div 1 = good
		// percemtage 1 = good
		// div 1+ (1-percent) * (254 or less depending on min color)
	}

	float GetColorMultiplier(float current_level){
		return bulb_dim_max_black + ((1-bulb_dim_max_black)*light_percentage);
	}

	public float GetLightDim(float current_level, float timescale){
		if (current_level < .01f) {
			return current_level;
		}
		return current_level * .45f * timescale * bulb_dim_time_mult; // 2; //each (second/timemult)? light level is reduced to half
		// div 2 ^ (timescale)? no
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
