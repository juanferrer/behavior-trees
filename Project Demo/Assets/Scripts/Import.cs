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
        MONEY,
        INGREDIENTS
    }

    public enum IngredientsState
    {
        RAW,
        PREPARED,
        COOKED
    }

    public enum ThoughtType
    {
        // Customer
        SATISFIED,
        UNSATISFIED,
        // Waiter
        ORDER,
        FOOD,
        BILL,
        // Cook
        INGREDIENTS,
        PREPARE,
        COOK,
    }

    [System.Serializable]
    public struct Food
    {
        public TableScript table;
        public FoodType food;
    }

    [System.Serializable]
    public struct Ingredients
    {
        public TableScript table;
        public FoodType food;
        public IngredientsState state;
    }

    static public class Globals
	{
		static public int MinWaitTime = 60;
		static public int MaxWaitTime = 200;
	}
}
