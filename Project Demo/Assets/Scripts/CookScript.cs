using System;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviorTree;
using UnityEngine.AI;
using Data;

public class CookScript : MonoBehaviour
{
    GameManagerScript gm;                   // Game manager
    NavMeshAgent agent;                     // Navigation agent
    BehaviorTree bt;                        // Behaviour tree
    BehaviorTree getOrderSequence;          // Subtree to get order to prepare
    BehaviorTree grabIngredients;           // Subtree for getting required ingredients
    BehaviorTree prepareIngredients;        // Subtree for preparing required ingredients
    BehaviorTree cookIngredients;           // Subtree for cooking prepared ingredients
    BehaviorTree finishDish;                // Subtree for finishing dish
    BehaviorTree leaveDishToServe;          // Subtree for leaving dish
    BehaviorTree getAwayFromTheTable;       // Subtree for getting away from table
    KitchenScript kitchen;                  // Kitchen
    StorageScript storage;                  // Storage
    WorktopScript worktop;                  // Worktop
    OvenScript oven;                        // Oven
    BlackboardScript blackboard;            // Global information
    public Inventory Inventory;             // Inventory of entity

    private bool alreadyGoingSomewhereThisFrame = false;
    private bool isRecalculatingTree = false;

    /// <summary>
    /// Use Unity's meshnav to travel to given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Status GoTo(Vector3 pos)
    {
        if (alreadyGoingSomewhereThisFrame)
        {
            return Status.FAILURE;
        }
        alreadyGoingSomewhereThisFrame = true;
        if (agent.destination != transform.position && (agent.destination - pos).sqrMagnitude > agent.stoppingDistance * 2)
        {
            agent.ResetPath();
            return Status.FAILURE;
        }

        // Set a new destination
        if (agent.isStopped)
        {
            agent.SetDestination(pos);
            agent.isStopped = false;
            return Status.RUNNING;
        }

        bool reachedPos = !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance) && (!agent.hasPath || Mathf.Approximately(agent.velocity.sqrMagnitude, 0.0f));
        bool onTheWay = agent.remainingDistance != Mathf.Infinity && (agent.remainingDistance > agent.stoppingDistance);

        if (reachedPos)
        {
            Debug.Log("Cook reached destination: " + pos.ToString("G2"));
            agent.isStopped = true;
            return Status.SUCCESS;
        }
        else if (onTheWay) return Status.RUNNING;
        else return Status.FAILURE;
    }

    /// <summary>
    /// Get position from object and call overloaded GoTo
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private Status GoTo(MonoBehaviour obj)
    {
        Vector3 pos = obj.transform.position;

        return GoTo(pos);
    }

    /// <summary>
    /// Check if waiter is close enough to object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool CloseEnough(MonoBehaviour obj)
    {
        return (transform.position - obj.transform.position).sqrMagnitude <= (agent.stoppingDistance * 2);
    }

    private Status GetOrderFromKitchen()
    {
        if (!CloseEnough(kitchen))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        Inventory.order = kitchen.GetOrder();
        Debug.Log("Cook got order from kitchen");
        return Status.SUCCESS;
    }

    private Status GetIngredientsFromStorage()
    {
        if (!CloseEnough(storage))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        storage.GetIngredients(Inventory.order, Inventory);
        Debug.Log("Cook got ingredients from storage");
        return Status.SUCCESS;
    }

    private Status GetEmptyWorktop()
    {
        if (worktop != null) return Status.SUCCESS;
        if (blackboard.EmptyWorktopsCount > 0)
        {
            worktop = blackboard.GetEmptyWorktop();
            return Status.SUCCESS;
        }
        return Status.FAILURE;
    }

    private Status PrepareIngredientsInWorktop()
    {
        if (!CloseEnough(worktop))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        worktop.PrepareIngredients(Inventory.ingredients, Inventory);
        // And reset for next step
        worktop = null;
        Debug.Log("Cook prepared ingredients in worktop");
        return Status.SUCCESS;
    }

    private Status GetEmptyOven()
    {
        if (oven != null) return Status.SUCCESS;
        if (blackboard.EmptyOvensCount > 0)
        {
            oven = blackboard.GetEmptyOven();
            return Status.SUCCESS;
        }
        return Status.FAILURE;
    }

    private Status CookIngredients()
    {
        if (!CloseEnough(oven))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        oven.CookIngredients(Inventory.ingredients, Inventory);
        // And reset for next step
        oven = null;
        Debug.Log("Cook cooked ingredients in oven");
        return Status.SUCCESS;
    }

    private Status PrepareDish()
    {
        if (!CloseEnough(worktop))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }

        worktop.FinishDish(Inventory.ingredients, Inventory);
        // And reset for next step
        worktop = null;
        Debug.Log("Cook finished dish");
        return Status.SUCCESS;
    }

    private Status LeaveDishInKitchen()
    {
        if (!CloseEnough(kitchen))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }

        kitchen.AddFoodPrepared(Inventory.food);
        Inventory.food = new Food();
        Debug.Log("Cook left dish in kitchen");
        return Status.SUCCESS;
    }

    // Use this for initialization
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        kitchen = gm.kitchen;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        Inventory = new Inventory();
        blackboard = gm.blackboard;
        storage = GameObject.FindGameObjectWithTag("Storage").GetComponent<StorageScript>();

        getOrderSequence = new BehaviorTreeBuilder("GetOrderSequence")
            .Sequence("GetOrderSequence")
                    .Sequence("NeedsToGetOrder")
                        .Not("DoesNotHaveOrder")
                            .If("HasOrder", () => { return Inventory.Has(ItemType.ORDER); })
                        .Not("IsNotAlreadyPreparingSomething")
                            .If("HasIngredients", () => { return Inventory.Has(ItemType.INGREDIENTS); })
                        .If("ThereAreOrdersPending", () => { return kitchen.IsOrderPending(); })
                        .End()
                    .Do("GoToKitchen", () => { return GoTo(kitchen); })
                    .Do("GetOrderFromKitchen", GetOrderFromKitchen)
                .End()
            .End();

        grabIngredients = new BehaviorTreeBuilder("GrabIngredientsSequence")
            .Sequence("GrabIngredientsSequence")
                .Sequence("NeedsToGetIngredients")
                    .If("HasRecipe", () => { return Inventory.Has(ItemType.ORDER); })
                    .Not("DoesNotHaveIngredientsForRecipe")
                        .If("HasIngredientsForRecipe", () => { return Inventory.Has(ItemType.INGREDIENTS); })
                        .Not("IsNotRecalculating")
                            .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GoToStorage", () => { return GoTo(storage); })
                .Do("GrabRequiredIngredient", GetIngredientsFromStorage)
                .End()
            .End();

        prepareIngredients = new BehaviorTreeBuilder("PrepareIngredientsSequence")
            .Sequence("PrepareIngredientsSequence")
                .Sequence("NeedsToPrepareIngredients")
                    .If("HasIngredientsForRecipe", () => { return Inventory.Has(ItemType.INGREDIENTS); })
                    .If("AreIngredientsPrepared", () => { return Inventory.ingredients.state == IngredientsState.RAW; })
                    .Not("IsNotRecalculating")
                        .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GetEmptyWorktop", GetEmptyWorktop)
                .Do("GoToWorktop", () => { return GoTo(worktop); })
                .Do("PrepareIngredients", PrepareIngredientsInWorktop)
                .End()
            .End();

        cookIngredients = new BehaviorTreeBuilder("CookIngredientsSequence")
            .Sequence("GrabIngredientsSequence")
                .Sequence("NeedsToCookIngredients")
                    .If("HasIngredientsForRecipe", () => { return Inventory.Has(ItemType.INGREDIENTS); })
                    .If("AreIngredientsPrepared", () => { return Inventory.ingredients.state == IngredientsState.PREPARED; })
                    .Not("IsNotRecalculating")
                            .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GetEmptyOven", GetEmptyOven)
                .Do("GoToOven", () => { return GoTo(oven); })
                .Do("CookIngredients", CookIngredients)
                .End()
            .End();

        finishDish = new BehaviorTreeBuilder("FinishDishSequence")
            .Sequence("FinishDishSequence")
                .Sequence("HasCookedIngredients")
                    .If("HasIngredientsForRecipe", () => { return Inventory.Has(ItemType.INGREDIENTS); })
                    .If("AreIngredientsCooked", () => { return Inventory.ingredients.state == IngredientsState.COOKED; })
                    .Not("IsNotRecalculating")
                            .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GetEmptyWorktop", GetEmptyWorktop)
                .Do("GoToWorktop", () => { return GoTo(worktop); })
                .Do("PrepareDish", PrepareDish)
                .End()
            .End();

        leaveDishToServe = new BehaviorTreeBuilder("LeaveDishToServeSequence")
            .Sequence("LeaveDishToServeSequence")
                .Sequence("HasFinishedDish")
                    .If("HasFood", () => { return Inventory.Has(ItemType.FOOD); })
                    .End()
                .Do("GoToKitchen", () => { return GoTo(kitchen); })
                .Do("LeaveDishInKitchen", LeaveDishInKitchen)
                .End()
            .End();

        getAwayFromTheTable = new BehaviorTreeBuilder("GetAwayFromTheTableSequence")
            .Sequence("GetAwayFromTheTableSequence")
                .Do("GoSomewhereElse", () => { return GoTo(new Vector3(24.5f, 0, 4.5f)); })
                .End()
            .End();

        bt = new BehaviorTreeBuilder("CookBT")
            .RepeatUntilFail("Loop")
                .Selector("Selector")
                    .Do("GetOrder", getOrderSequence)
                    .Do("GrabIngredients", grabIngredients)
                    .Do("PrepareIngredients", prepareIngredients)
                    .Do("CookIngredients", cookIngredients)
                    .Do("FinishDish", finishDish)
                    .Do("LeaveDishToServe", leaveDishToServe)
                    .Do("GetAwayFromTheTable", getAwayFromTheTable)
                    .Do("JustWait", () => {
                        isRecalculatingTree = false;
                        return Status.SUCCESS;
                    })
                    .End()
            .End();

    }

    // Update is called once per frame
    void Update()
    {
        bt.Tick();
        alreadyGoingSomewhereThisFrame = false;
    }
}
