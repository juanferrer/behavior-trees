using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableScript : MonoBehaviour
{
	public void Clean()
	{
		// TODO: Change texture to clean?

		IsClean = true;
	}

	public void SetCustomer(GameObject customer)
	{
		// TODO: Add a new customer to this table

		Customer = customer;
	}

	public bool IsClean { get; private set; }

	public bool IsOccupied { get; private set; }

	public GameObject Customer { get; private set; }


	// Use this for initialization
	void Start()
	{
		IsClean = true;
		IsOccupied = false;
		Customer = null;
	}

	// Update is called once per frame
	void Update()
	{

	}
}
