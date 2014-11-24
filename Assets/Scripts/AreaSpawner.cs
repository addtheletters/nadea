using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaSpawner : MonoBehaviour {

	// refs
	public GameObject toSpawn; // needs assignment
	public GameObject scorekeeper; // also

	// settings
	public bool spawning_enabled = true;
	
	public float xSpread = 20f;
	public float ySpread = 10f;
	public float zSpread = 20f;

	public int max_spawned_objs = 8;
	public int spawn_group_size = 5;
	public float spawn_period = 7f;


	// internal
	private float spawn_timer = 0f;
	//List<GameObject> spawned_objects;

	// Use this for initialization
	void Start () {
		/*if ( spawned_objects == null ) {
			spawned_objects =	new List<GameObject> ();
		}*/
	}
	
	// Update is called once per frame
	void Update () {
		if (!spawning_enabled) {
			if(spawn_timer <= 0){
				spawn_timer = 0;
			}
			return;
		}
		if (spawn_timer < 0) {
			TriggerSpawn();
			spawn_timer = spawn_period;
		}
		spawn_timer -= Time.deltaTime;

	}

	void TriggerSpawn(){
		for (int i = 0; i < spawn_group_size; i++) {
			if( this.transform.childCount >= max_spawned_objs){
				break;
			}
			SpawnSingle();
		}
	}

	float RandCloseTo(float center, float rad){
		return center + Random.Range(-rad, rad);
	}

	void SpawnSingle(){
		Vector3 pos = new Vector3(
			RandCloseTo(transform.position.x, xSpread/2),
			RandCloseTo(transform.position.y, ySpread/2),
			RandCloseTo(transform.position.z, zSpread/2));
		if (!toSpawn) {
			Debug.Log("No object assigned to be spawned.");
			return;
		}
		GameObject newspawn = (GameObject)Instantiate (toSpawn, pos, new Quaternion());
		ScoreObject so = newspawn.GetComponent<ScoreObject> ();
		if (so) {
			so.scorekeeper = this.scorekeeper;
		}
		newspawn.transform.parent = this.transform;
		//spawned_objects.Add (newspawn);
	}




}
