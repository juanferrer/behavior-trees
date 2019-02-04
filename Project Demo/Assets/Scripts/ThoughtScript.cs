using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtScript : MonoBehaviour {

    GameManagerScript gm;
    GameObject cam;

	// Use this for initialization
	void Start ()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        cam = gm.cam;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(cam.transform);
    }
}
