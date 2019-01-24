using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	/// <summary>
	/// Range of available dishes
	/// </summary>
	public enum FoodType
	{
        NONE,
		SOUP,
		PIZZA,
		FISHNCHIPS,
	}

    public enum ItemType
    {
        NONE,
        ORDER,
        FOOD,
        BILL,
        MONEY
    }

    [System.Serializable]
    public struct Food
    {
        public TableScript table;
        public FoodType food;
    }

    static public class Globals
	{
		static public int MinWaitTime = 20;
		static public int MaxWaitTime = 100;
	}
}
