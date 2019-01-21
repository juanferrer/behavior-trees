using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using FluentBehaviorTree;

public class Inventory
{
    public Food food;
    public bool bill;
    public bool money;
    public Food order;

    public Inventory()
    {
        food = new Food();
        bill = false;
        money = false;
        order = new Food();
    }

    public bool Has(ItemType type)
    {
        switch (type)
        {
            case ItemType.FOOD: return this.food.food != FoodType.NONE;
            case ItemType.BILL: return this.bill;
            case ItemType.MONEY: return this.money;
            case ItemType.ORDER: return this.order.food != FoodType.NONE;
            default: return false;
        }
    }

    public Status GetFrom(ItemType type, Inventory entity)
    {
        return entity.GiveTo(type, this);
    }

    public Status GiveTo(ItemType type, Inventory entity)
    {
        if (!Has(type)) return Status.FAILURE;
        switch (type)
        {
            case ItemType.FOOD:
                entity.food = this.food;
                this.food = new Food();
                break;
            case ItemType.BILL:
                entity.bill = this.bill;
                this.bill = false;
                break;
            case ItemType.MONEY:
                entity.money = this.money;
                this.money = false;
                break;
            case ItemType.ORDER:
                entity.order = this.order;
                this.order = new Food();
                break;
        }
        return Status.SUCCESS;
    }
}
