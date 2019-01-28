using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OvenScript : MonoBehaviour {

    public bool IsOccupied { get; private set; }

    public OvenScript Cook { get; private set; }

    public void StartUsing(OvenScript cook)
    {
        IsOccupied = true;
        Cook = cook;
    }

    public void StopUsing()
    {
        IsOccupied = false;
        Cook = null;
    }

    // Use this for initialization
    void Start()
    {
        IsOccupied = false;
        Cook = null;
    }
}
