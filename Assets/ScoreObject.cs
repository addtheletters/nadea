using UnityEngine;
using System.Collections;

public class ScoreObject : MonoBehaviour {

	public GameObject scorekeeper;
	public float score_to_add = 100f;
	public float multiplier_bonus = 0.1f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void TriggerScoreObject(){
		Scorer sc = scorekeeper.GetComponent<Scorer> ();
		sc.AddMultipliedScore (this.score_to_add);
		sc.AddToMultiplier (this.multiplier_bonus);
	}
	
}
