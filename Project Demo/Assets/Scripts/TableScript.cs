using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableScript : MonoBehaviour
{
	public bool IsClean { get; private set; }

	public bool IsOccupied { get; private set; }
    public bool IsAssigned { get; set; }

	public CustomerScript Customer { get; private set; }

    public void Clean()
    {
        // TODO: Change texture to clean?

        IsClean = true;
    }

    public void SetCustomer(CustomerScript customer)
    {
        // TODO: Add a new customer to this table
        IsOccupied = true;
        Customer = customer;
        IsAssigned = false;
    }

    // Use this for initialization
    void Start()
	{
		IsClean = true;
		IsOccupied = false;
        IsAssigned = false;
		Customer = null;
	}
}
