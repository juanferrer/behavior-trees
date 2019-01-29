using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class OvenScript : MonoBehaviour {

    public bool IsOccupied { get; private set; }
    public bool IsAssigned { get; set; }

    public void CookIngredients(Ingredients ingredients, Inventory entity)
    {
        var newIngredients = ingredients;
        newIngredients.state = IngredientsState.COOKED;
        entity.ingredients = newIngredients;
    }

    // Use this for initialization
    void Start()
    {
        IsOccupied = false;
        IsAssigned = false;
    }
}
