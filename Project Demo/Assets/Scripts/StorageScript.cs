using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class StorageScript : MonoBehaviour {

    public Inventory Inventory;

    public void GetIngredients(Food order, Inventory entity)
    {
        // Remove order
        entity.order = new Food();
        // Prepare ingredients
        var ingredients = new Ingredients();
        ingredients.food = order.food;
        ingredients.table = order.table;
        ingredients.state = IngredientsState.RAW;
        // And give them to entity
        entity.ingredients = ingredients;
    }

    // Use this for initialization
    void Start()
    {
        Inventory = new Inventory();
    }
}
