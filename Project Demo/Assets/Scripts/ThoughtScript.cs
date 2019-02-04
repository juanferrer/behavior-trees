using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class ThoughtScript : MonoBehaviour {

    GameManagerScript gm;
    GameObject cam;
    SpriteRenderer renderer;
    float showTimer;
    public bool IsShowing { get; private set;}

	// Use this for initialization
	void Start ()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        cam = gm.cam;
        renderer = GetComponent<SpriteRenderer>();
        showTimer = 0;
        IsShowing = false;
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
                IsShowing = false;
            }
        }
    }

    public void Show(ThoughtType thought)
    {
        showTimer = 2;
        renderer.enabled = true;
        IsShowing = true;
    }
}
