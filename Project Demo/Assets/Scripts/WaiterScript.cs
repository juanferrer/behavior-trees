using UnityEngine;
using FluentBehaviorTree;

public class WaiterScript : MonoBehaviour
{
	GameManagerScript gm;


	BehaviorTree bt;
	QueueScript queue;
	KitchenScript kitchen;	   
	TableScript table;
	CustomerScript customer;
	FoodScript food;

	bool ShouldReceiveCustomer,
		ShouldAttendCustomer,
		ShouldServeFood,
		ShouldBringBill;

	/// <summary>
	/// Use Unity's meshnav to travel to given position
	/// </summary>
	/// <param name="pos"></param>
	/// <returns></returns>
	private Status GoTo(Vector3 pos)
	{
		bool reachedPos = false,
			 onTheWay = false;
		// TODO: Use Unity's meshnav to travel to position

		if (reachedPos) return Status.SUCCESS;
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

	private Status GiveTo(string tag, MonoBehaviour recipient)
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

	private Status GetFrom(string tag, MonoBehaviour giver)
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

    /// <summary>
    /// Decide what to do next and set the value of the appropriate boolean
    /// </summary>
    /// <returns></returns>
    private Status AssessPriority()
	{
        // TODO

        // We need to keep dishes warm, so that's first
        if (kitchen)
        {
            ShouldServeFood = true;
        }
		// TODO: Check if any dish is ready
		

		// There needs to be free tables before we receive a new customer


		//
		return Status.ERROR;
	}

    private Status FindAnEmptyTable()
    {
        return Status.ERROR;
    }

    private Status SendCustomerToTable()
    {
        // TODO: Find a free table

        table.SetCustomer(customer);
        return table.GetComponent<TableScript>().IsOccupied ? Status.SUCCESS : Status.FAILURE;
    }

        private Status CleanTable(TableScript table)
    {
        var tableScript = table.GetComponent<TableScript>();
        tableScript.Clean();
        return tableScript.IsClean ? Status.SUCCESS : Status.FAILURE;
    }

    // Use this for initialization
    void Start()
	{
		// TODO: Get references to each object
		gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		kitchen = gm.kitchen;
        queue = gm.queue;

		// Declare BT
		bt = new BehaviorTreeBuilder("")
		.RepeatUntilFail("Loop")
			.Sequence("Sequence")
				.Do("AssessPriority", AssessPriority)
				.Selector("Selector")
					.Sequence("ReceiveCustomer")
						.If("ShouldReceiveCustomer", () => { return ShouldReceiveCustomer; })
						.Do("GoToQueue", () => { return GoTo(queue.transform.position); })
						.Do("SendCustomerToTable", SendCustomerToTable)
					.End()
					.Sequence("AttendCustomer")
						.If("ShouldAttendCustomer", () => { return ShouldAttendCustomer; })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GetOrderFromCustomer", () => { return GetFrom("order", customer); })
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("GiveOrderToKitchen", () => { return GiveTo("order", kitchen); })
					.End()
					.Sequence("ServeFood")
						.If("ShouldServeFood", () => { return ShouldServeFood; })
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("PickupFood", () => { return GetFrom("food", kitchen); })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GiveFoodToCustomer", () => { return GiveTo("food", customer); })
					.End()
					.Sequence("BringBill")
						.If("ShouldBringBill", () => { return ShouldBringBill; })
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("PickupBill", () => { return GetFrom("Bill", kitchen); })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GiveBillToCustomer", () => { return GiveTo("Bill", customer); })
						.Do("GetMoneyFromCustomer", () => { return GetFrom("Money", customer); })
						.Do("CleanTable", () => { return CleanTable(table); })
						.End()
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
