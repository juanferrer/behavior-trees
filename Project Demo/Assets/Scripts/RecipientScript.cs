using System;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class RecipientScript : MonoBehaviour
{

	private Dictionary<ItemType, GameObject> inventory;

	/// <summary>
	/// Give object to recipient
	/// </summary>
	/// <param name="obj"></param>
	public void Give(GameObject obj)
	{
        // TODO: Receive object
        ItemType type;
		// Process type of object
		if (Enum.TryParse(obj.tag.ToUpper(), out type) && !Has(type))
		{
			inventory[type] = obj;
		}
		else
		{
			// TODO: What to do it recipient already has object with tag?
			// Multiple foods? Bills?
		}
	}

	/// <summary>
	///  Get object with tag from recipient, which loses possession of it
	/// </summary>
	/// <param name="tag"></param>
	/// <returns></returns>
	public GameObject Get(ItemType tag)
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
	/// Check whether recipient has object with tag
	/// </summary>
	/// <param name="tag"></param>
	/// <returns></returns>
	public bool Has(ItemType tag)
	{
		if (tag == ItemType.NONE) return false;
		return inventory.ContainsKey(tag);
	}




	// Use this for initialization
	void Start()
	{
		inventory = new Dictionary<ItemType, GameObject>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
