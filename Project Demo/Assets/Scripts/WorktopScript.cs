using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class WorktopScript : MonoBehaviour {

    public bool IsOccupied { get; private set; }
    public bool IsAssigned { get; set; }

    public void PrepareIngredients(Ingredients ingredients, Inventory entity)
    {
        IsOccupied = true;
        var newIngredients = ingredients;
        newIngredients.state = IngredientsState.PREPARED;
        entity.ingredients = newIngredients;
        IsOccupied = false;
        IsAssigned = false;
    }

    public void FinishDish(Ingredients ingredients, Inventory entity)
    {
        IsOccupied = true;

        var food = new Food();
        food.food = ingredients.food;
        food.table = ingredients.table;

        entity.food = food;
        entity.ingredients = new Ingredients();

        IsOccupied = false;
        IsAssigned = false;
    }

    // Use this for initialization
    void Start ()
    {
        IsOccupied = false;
        IsAssigned = false;
	}
}
