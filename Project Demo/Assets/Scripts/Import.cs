using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	/// <summary>
	/// Range of available dishes
	/// </summary>
	enum Food
	{
		Dish1,
		Dish2,
		Dish3,
	}

    public enum ItemType
    {
        NONE,
        ORDER,
        FOOD,
        BILL,
        MONEY
    }

    static public class Globals
	{
		static public int MinWaitTime = 20;
		static public int MaxWaitTime = 100;
	}
}
