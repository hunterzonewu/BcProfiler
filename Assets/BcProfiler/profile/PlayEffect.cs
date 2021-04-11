using UnityEngine;
using System.Collections;
using System;

public class PlayEffect : MonoBehaviour {

    float time_ = 2.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        time_ -= Time.deltaTime;
        if (time_ <= 0)
        {
            ParticleSystem[] particle = this.GetComponentsInChildren<ParticleSystem>(true);
            Array.ForEach(particle, p => p.Play());
            time_ = 2.0f;

        }
	}
}
