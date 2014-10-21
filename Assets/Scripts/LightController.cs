using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour {

	//Light controller should be attached to the same thing as NadeLogic

	//setup / refs
	public Color bulb_color;
	public GameObject light_bulb;
	public Light actual_light;

	// const
	public float bulb_dim_max_black = .1f;
	public float bulb_dim_time_mult = 10f;

	// vars	
	public bool is_on;
	private float light_percentage = 0f;


	// Use this for initialization
	void Start () {
		if (!light_bulb) {
			light_bulb = this.gameObject;
		}
		if (!actual_light) {
			actual_light = this.gameObject.GetComponent<Light>();
		}
		if (light_bulb && !actual_light) {
			actual_light = light_bulb.GetComponent<Light>();
		}
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
		light_bulb.GetComponent<MeshRenderer> ().material.color = bulb_color * GetColorMultiplier(light_percentage);
		// change the material color of the bulb to seem more/less glowy
		actual_light.intensity = light_percentage; // and dim the point light source
	}

	float GetColorMultiplier(float current_level){
		return bulb_dim_max_black + ((1-bulb_dim_max_black)*light_percentage); // color for material of bulb
	}

	public float GetLightDim(float current_level, float timescale){
		if (current_level < .01f) {
			return current_level; // don't want teeny tiny light floats, just force to zero if <1%
		}
		return current_level * .75f * timescale * bulb_dim_time_mult; // 2; //each (second/timemult)? light level is reduced to half
		// time mult var can be tweaked for faster / slower light fading
	}

	// turn off and turn on for continuous light shining
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
