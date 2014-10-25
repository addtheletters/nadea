using UnityEngine;
using System.Collections;

public class NadePin : MonoBehaviour {

	public GameObject stuck_in_nade;
	public 	float despawn_wait = 7f;
	private float despawn_timer;

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
