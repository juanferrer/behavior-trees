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
    KitchenScript kitchen;                  // Kitchen
    GameObject storage;                     // Storage
    BlackboardScript blackboard;            // Global information
    public Inventory Inventory;             // Inventory of entity

    private bool alreadyGoingSomewhereThisFrame = false;

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
            Debug.Log("Waiter reached destination: " + pos.ToString("G2"));
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

    private Status GetOrderFromKitchen()
    {
        Inventory.order = kitchen.GetOrder();
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

        grabIngredients = new BehaviorTreeBuilder("GrabIngredientsSequence")
            .Sequence("GrabIngredientsSequence")
                .Sequence("NeedsToGetIngredients")
                    .Not("DoesNotHaveIngredientsForRecipe")
                        .If("HasIngredientsForRecipe", () => { return false; })
                    .End()
                .Do("GoToStorage", () => { return GoTo(storage.transform.position); })
                .Do("GrabRequiredIngredient", () => { return Status.ERROR; })
                .End()
            .End();

        prepareIngredients = new BehaviorTreeBuilder("PrepareIngredientsSequence")
            .Sequence("PrepareIngredientsSequence")
                .Sequence("NeedsToPrepareIngredients")
                    .If("HasIngredientsForRecipe", () => { return false; })
                    .Not("IngredientsAreNotPrepared")
                        .If("AreIngredientsPrepared", () => { return false; })
                    .End()
                .Do("GoToWorktop", () => { return GoTo(blackboard.GetEmptyWorktop()); })
                .Do("PrepareIngredients", () => { return Status.ERROR; })
                .End()
            .End();

        cookIngredients = new BehaviorTreeBuilder("")
            .Sequence("GrabIngredientsSequence")
                .Sequence("NeedsToCookIngredients")
                    .If("HasIngredientsForRecipe", () => { return false; })
                    .If("AreIngredientsPrepared", () => { return false; })
                    .Not("IngredientsAreNotCooked")
                        .If("AreIngredientsCooked", () => { return false; })
                    .End()
                .Do("GoToOven", () => { return GoTo(blackboard.GetEmptyOven()); })
                .Do("CookIngredients", () => { return Status.ERROR; })
                .End()
            .End();

        finishDish = new BehaviorTreeBuilder("")
            .Sequence("FinishDishSequence")
                .Sequence("HasFinishedDish")
                    .If("HasDish", () => { return Inventory.Has(ItemType.FOOD); })
                    .End()
                .End()
            .End();

        bt =  new BehaviorTreeBuilder("CookBT")
            .RepeatUntilFail("Loop")
                .Selector("Selector")
                    .Do("GetOrder", getOrderSequence)
                    .Sequence("Cook")
                        .Do("GrabIngredients", grabIngredients)
                        .Do("PrepareIngredients", prepareIngredients)
                        .Do("CookIngredients", cookIngredients)
                        .Do("FinishDish", finishDish)
                        .End()
                    .Do("JustWait", () => { return Status.ERROR; })
                .End()
            .End();

    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
