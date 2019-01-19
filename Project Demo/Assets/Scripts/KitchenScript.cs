using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenScript : MonoBehaviour
{

    private List<FoodScript> foodPrepared;

	// Use this for initialization
	void Start ()
    {
        foodPrepared = new List<FoodScript>();
	}
	
	// Update is called once per frame
	void Update ()
    {	
	}

    public FoodScript GetFoodPrepared()
    {
        FoodScript plate = foodPrepared[0];
        foodPrepared.RemoveAt(0);
        return plate;
    }

    public void AddFoodPrepared(FoodScript plate)
    {
        foodPrepared.Add(plate);
    }

    public bool IsFoodPrepared()
    {
        return foodPrepared.Count > 0;
    }
}
