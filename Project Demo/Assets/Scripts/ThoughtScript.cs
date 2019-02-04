using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class ThoughtScript : MonoBehaviour {

    GameManagerScript gm;
    GameObject cam;
    SpriteRenderer renderer;
    float showTimer;

	// Use this for initialization
	void Start ()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        cam = gm.cam;
        renderer = GetComponent<SpriteRenderer>();
        showTimer = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        if (showTimer > 0)
        {
            showTimer -= Time.deltaTime;
            if (showTimer < 0)
            {
                renderer.enabled = false;
            }
        }
    }

    public void Show(ThoughtType thought)
    {
        showTimer = 5;
        renderer.enabled = true;
    }
}
