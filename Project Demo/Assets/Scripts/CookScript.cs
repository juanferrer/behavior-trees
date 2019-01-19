using System;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviorTree;
using UnityEngine.AI;

public class CookScript : MonoBehaviour
{

    GameManagerScript gm;
    NavMeshAgent agent;
    BlackboardScript blackboard;
    BehaviorTree bt;
    GameObject kitchen;
    GameObject table;
    MonoBehaviour queue;

    /// <summary>
    /// Use Unity's meshnav to travel to given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Status GoTo(Vector3 pos)
    {
        // Set a new destination
        if (agent.isStopped)
        //if (!Mathf.Approximately(agent.destination.x, pos.x) || !Mathf.Approximately(agent.destination.z, pos.z))
        {
            //agent.destination = pos;
            agent.SetDestination(pos);
            agent.isStopped = false;
            return Status.RUNNING;
        }

        bool reachedPos = /*!agent.pathPending && */(agent.remainingDistance <= agent.stoppingDistance) && (!agent.hasPath || Mathf.Approximately(agent.velocity.sqrMagnitude, 0.0f));
        bool onTheWay = agent.remainingDistance != Mathf.Infinity && (agent.remainingDistance > agent.stoppingDistance);

        if (reachedPos)
        {
            Debug.Log("Customer reached destination: " + pos.ToString("G2"));
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
    /// Decide what to do next and set the value of the appropriate boolean
    /// </summary>
    /// <returns></returns>
    private Status AssessPriority()
    {
        return Status.ERROR;
    }

    private Status CleanTable(GameObject table)
    {
        return Status.ERROR;
    }

    private Status FindAnEmptyTable()
    {
        return Status.ERROR;
    }

    private Status SendCustomerToTable(GameObject table)
    {
        return Status.ERROR;
    }

    // Use this for initialization
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        blackboard = gm.GetComponent<BlackboardScript>();
        queue = gm.queue.GetComponent<QueueScript>();
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
