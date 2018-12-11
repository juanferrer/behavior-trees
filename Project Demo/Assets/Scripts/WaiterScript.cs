using UnityEngine;
using FluentBehaviorTree;

public class WaiterScript : MonoBehaviour
{
	/// <summary>
	/// The behaviour tree of this agent
	/// </summary>
	BehaviorTree bt;

	/// <summary>
	/// Queue object. Constant reference, queue is always the same
	/// </summary>
	GameObject queue;

	/// <summary>
	/// Queue object. Constant reference, queue is always the same
	/// </summary>
	GameObject kitchen;

	/// <summary>
	/// Table object. Variable reference, set when assessing priority
	/// </summary>		   
	GameObject table;

	/// <summary>
	/// Customer object. Variable reference, set when receiving customer
	/// </summary>
	GameObject customer;

	/// <summary>
	/// Food object. Variable reference, set when bringing foot to table
	/// </summary>
	GameObject food;

	bool ShouldReceiveCustomer,
		ShouldAttendCustomer,
		ShouldServeFood,
		ShouldBringBill;

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
	/// 
	/// </summary>
	/// <param name="objectPos"></param>
	/// <returns></returns>
	private Status GoTo(GameObject objectPos)
	{
		Vector3 pos = objectPos.transform.position;

		return GoTo(pos);
	}

	private Status BringCustomerToTable()
	{
		// TODO: Find a free table. Was it not set when assessing priority?

		table.GetComponent<TableScript>().SetCustomer(customer);
		return table.GetComponent<TableScript>().IsOccupied ? Status.SUCCESS : Status.FAILURE;
	}

	private Status GiveTo(string tag, GameObject recipient)
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

	private Status GetFrom(string tag, GameObject giver)
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

	private Status CleanTable(GameObject table)
	{
		table.GetComponent<TableScript>().Clean();
		return table.GetComponent<TableScript>().IsClean ? Status.SUCCESS : Status.FAILURE;
	}

	private Status AssessPriority()
	{
		// TODO

		// We need to keep dishes warm, so that's first
		// TODO: Check if any dish is ready

		// There needs to be free tables before we receive a new customer

		//
		return Status.ERROR;
	}

	// Use this for initialization
	void Start()
	{
		// TODO: Get references to each object

		// Declare BT
		bt = new BehaviorTreeBuilder("")
		.RepeatUntilFail("Loop")
			.Sequence("Sequence")
				.Do("AssessPriority", AssessPriority)
				.Selector("Selector")
					.Sequence("ReceiveCustomer")
						.If("ShouldReceiveCustomer", () => { return ShouldReceiveCustomer; })
						.Do("GoToQueue", () => { return GoTo(queue); })
						.Do("BringCustomerToTable", () => { return BringCustomerToTable(); })
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
						.Do("PickupBill", () => { return Status.ERROR; })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GiveBillToCustomer", () => { return Status.ERROR; })
						.Do("GetMoneyFromCustomer", () => { return Status.ERROR; })
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
