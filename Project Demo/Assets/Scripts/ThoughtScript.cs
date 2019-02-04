using System;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class ThoughtScript : MonoBehaviour {

    GameManagerScript gm;
    GameObject cam;
    SpriteRenderer topRenderer;
    SpriteRenderer baseRenderer;
    // Credits:
    // satisfied: Icon by Freepic from www.flaticon.com
    // unsatisfied: Icon by Freepic from www.flaticon.com
    // queue: Icon by Freepic from www.flaticon.com
    // menu: Icon by Freepic from www.flaticon.com
    // customer: Icon by Freepic from www.flaticon.com
    // order: Icon by Freepic from www.flaticon.com
    // food: Icon by Freepic from www.flaticon.com
    // bill: Icon by Freepic from www.flaticon.com
    // ingredients: Icon by Freepic from www.flaticon.com
    // prepare: Icon by Freepic from www.flaticon.com
    // cook: Icon by Freepic from www.flaticon.com
    List<Sprite> sprites;
    float showTimer;
    public bool IsShowing { get; private set;}

	// Use this for initialization
	void Start ()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        cam = gm.cam;
        sprites = gm.ThoughtSprites;
        topRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        baseRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
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
                topRenderer.enabled = false;
                baseRenderer.enabled = false;
                IsShowing = false;
            }
        }
    }

    public void Show(ThoughtType thought)
    {
        showTimer = 2;
        topRenderer.sprite = sprites[(int)thought];
        topRenderer.enabled = true;
        baseRenderer.enabled = true;
        IsShowing = true;
    }
}
