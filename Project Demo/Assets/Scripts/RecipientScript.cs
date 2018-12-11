using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipientScript : MonoBehaviour
{

	private Dictionary<string, GameObject> inventory;

	/// <summary>
	/// Give object to recipient
	/// </summary>
	/// <param name="obj"></param>
	public void Give(GameObject obj)
	{
		// TODO: Receive object

		// Process type of object
		if (!Has(obj.tag))
		{
			inventory[obj.tag] = obj;
		}
	}

	/// <summary>
	///  Get object from recipient, which loses possession of it
	/// </summary>
	/// <param name="tag"></param>
	/// <returns></returns>
	public GameObject Get(string tag)
	{
		GameObject obj = null;
		if (Has(tag))
		{
			obj = inventory[tag];
			inventory.Remove(tag);
			return inventory[tag];
		}
		return obj;
	}

	/// <summary>
	/// Check whether recipient has object
	/// </summary>
	/// <param name="tag"></param>
	/// <returns></returns>
	public bool Has(string tag)
	{
		if (tag == null) return false;
		return inventory.ContainsKey(tag);
	}




	// Use this for initialization
	void Start()
	{
		inventory = new Dictionary<string, GameObject>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
