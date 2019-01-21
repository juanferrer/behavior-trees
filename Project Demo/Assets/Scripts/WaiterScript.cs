using UnityEngine;
using FluentBehaviorTree;
using UnityEngine.AI;
using Data;

public class WaiterScript : MonoBehaviour
{
	GameManagerScript gm;
    NavMeshAgent agent;

    BehaviorTree bt;
	QueueScript queue;
	KitchenScript kitchen;	   
	TableScript table;
	CustomerScript customer;
	FoodScript food;
    BlackboardScript blackboard;

    bool shouldReceiveCustomer,
        shouldAttendCustomer,
        shouldServeFood,
        shouldBringBill,
        isPerformingAction;

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

    private Status GiveTo(ItemType tag, MonoBehaviour recipient)
	{
		// Get a reference to the my script
		var giverScript = this.GetComponent<RecipientScript>();
		// Make sure I have object
		if (giverScript.Has(tag))
		{
			var obj = giverScript.Get(tag);
			if (obj != null)
			{
				// If so, give object to recipient
				recipient.GetComponent<RecipientScript>().Give(obj);
				return Status.SUCCESS;
			}
			else { return Status.FAILURE; }
		}
		return Status.ERROR;
	}

	private Status GetFrom(ItemType tag, MonoBehaviour giver)
	{
		// Get a reference to the giver's script
		var giverScript = giver.GetComponent<RecipientScript>();
		// Make sure giver has object
		if (giverScript.Has(tag))
		{
			var obj = giverScript.Get(tag);
			if (obj != null)
			{
				// If so, give object to self
				GetComponent<RecipientScript>().Give(obj);
				return Status.SUCCESS;
			}
			else { return Status.FAILURE; }
		}
		return Status.ERROR;
	}

    private Status GetCustomerToAttend()
    {
        customer = blackboard.GetTableToAttend().Customer;
        return Status.SUCCESS;
    }

    private Status AttendCustomer()
    {
        GetFrom(ItemType.ORDER, customer);
        customer.Attend();

        return Status.SUCCESS;
    }

    /// <summary>
    /// Decide what to do next and set the value of the appropriate boolean
    /// </summary>
    /// <returns></returns>
    private Status AssessPriority()
	{
        // TODO: Add?
        //if (isPerformingAction) return Status.SUCCESS;

        shouldReceiveCustomer = false;
        shouldAttendCustomer = false;
        shouldServeFood = false;
        shouldBringBill = false;
        isPerformingAction = false;

        // We need to keep dishes warm, so that's first
        if (kitchen.IsFoodPrepared())
        {
            isPerformingAction = shouldServeFood = true;
            return Status.SUCCESS;
        }

        // We have no food ready to be served
        // There needs to be free tables before we receive a new customer
        // Also, make sure someone is in the queue
        if (blackboard.EmptyTablesCount > 0 && blackboard.CustomersInQueueCount > 0)
        {
            isPerformingAction = shouldReceiveCustomer = true;
            return Status.SUCCESS;
        }

        // Someone is already receiving customers or there is noone to receive
        // Bring bill to customers, lest they leave without paying
        // STOP THE SINPA! (https://en.wiktionary.org/wiki/sinpa)
        if (false)
        {
            // TODO
            isPerformingAction = shouldBringBill = true;
        }

        // Since the bills have been taken care of, check if any sitting customer
        // is still to be attended
        if (blackboard.CustomersToAttendCount > 0)
        {
            isPerformingAction = shouldAttendCustomer = true;
        }

        // Wait, I guess? You're still getting paid, tho
        isPerformingAction = false;

        return Status.SUCCESS;
	}

    private Status SendCustomerToTable()
    {
        table = blackboard.GetEmptyTable();
        customer = queue.GetNextCustomer();
        customer.Receive(table);
        table.SetCustomer(customer);

        // Now that the customer has been received, so prepare for next customer
        customer = null;
        table = null;
        isPerformingAction = false;

        return Status.SUCCESS;
    }

        private Status CleanTable(TableScript table)
    {
        var tableScript = table.GetComponent<TableScript>();
        tableScript.Clean();
        isPerformingAction = false;
        return tableScript.IsClean ? Status.SUCCESS : Status.FAILURE;
    }

    // Use this for initialization
    void Start()
	{
		// TODO: Get references to each object
		gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		kitchen = gm.kitchen;
        queue = gm.queue;
        blackboard = gm.blackboard;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        shouldReceiveCustomer = false;
        shouldAttendCustomer = false;
        shouldServeFood = false;
        shouldBringBill = false;
        isPerformingAction = false;

        // Declare BT
        bt = new BehaviorTreeBuilder("")
		.RepeatUntilFail("Loop")
			.Sequence("Sequence")
				.Do("AssessPriority", AssessPriority)
				.Selector("Selector")
					.Sequence("ReceiveCustomer")
						.If("ShouldReceiveCustomer", () => { return shouldReceiveCustomer; })
						.Do("GoToQueue", () => { return GoTo(queue); })
						.Do("SendCustomerToTable", SendCustomerToTable)
					.End()
					.Sequence("AttendCustomer")
						.If("ShouldAttendCustomer", () => { return shouldAttendCustomer; })
                        .Do("GetCustomerToAttend", GetCustomerToAttend)
						.Do("GoToCustomer", () => { return GoTo(customer); })
						.Do("Attend", AttendCustomer)
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("GiveOrderToKitchen", () => { return GiveTo(ItemType.ORDER, kitchen); })
					.End()
					.Sequence("ServeFood")
						.If("ShouldServeFood", () => { return shouldServeFood; })
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("PickupFood", () => { return GetFrom(ItemType.FOOD, kitchen); })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GiveFoodToCustomer", () => { return GiveTo(ItemType.ORDER, customer); })
					.End()
					.Sequence("BringBill")
						.If("ShouldBringBill", () => { return shouldBringBill; })
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("PickupBill", () => { return GetFrom(ItemType.BILL, kitchen); })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GiveBillToCustomer", () => { return GiveTo(ItemType.BILL, customer); })
						.Do("GetMoneyFromCustomer", () => { return GetFrom(ItemType.MONEY, customer); })
						.Do("CleanTable", () => { return CleanTable(table); })
						.End()
                    .Do("JustWait", () => { return Status.SUCCESS; })
					.End()
				.End()
			.End();

	}

	// Update is called once per frame
	void Update()
	{
		bt.Tick();
	}
}
