using System;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviorTree;
using UnityEngine.AI;

public class CookScript : MonoBehaviour
{
    GameManagerScript gm;
    NavMeshAgent agent;
    BehaviorTree bt;
    BehaviorTree grabIngredients;
    BehaviorTree prepareIngredients;
    BehaviorTree cookIngredients;
    QueueScript queue;
    KitchenScript kitchen;
    TableScript table;
    CustomerScript customer;
    public Inventory Inventory;

    /// <summary>
    /// Use Unity's meshnav to travel to given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Status GoTo(Vector3 pos)
    {
        // Set a new destination
        if (agent.isStopped)
        {
            agent.SetDestination(pos);
            agent.isStopped = false;
            return Status.RUNNING;
        }

        bool reachedPos = /*!agent.pathPending && */(agent.remainingDistance <= agent.stoppingDistance) && (!agent.hasPath || Mathf.Approximately(agent.velocity.sqrMagnitude, 0.0f));
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

    // Use this for initialization
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        kitchen = gm.kitchen;
        queue = gm.queue;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        Inventory = new Inventory();



        bt =  new BehaviorTreeBuilder("")
            .RepeatUntilFail("Loop")
                .Selector("Selector")
                    .Sequence("Cook")
                        .Do("GrabIngredients", grabIngredients)
                        .Do("PrepareIngredients", prepareIngredients)
                        .Do("CookIngredients", cookIngredients)
                        .Do("FinishDish", () => { return Status.ERROR; })
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
