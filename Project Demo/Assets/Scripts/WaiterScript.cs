using UnityEngine;
using FluentBehaviorTree;

public class WaiterScript : MonoBehaviour
{	
	/// <summary>
    /// The behaviour tree of this agent
    /// </summary>
	BehaviorTree bt;

	/// <summary>
	/// Queue object. Constant reference, the queue is always the same
	/// </summary>
	GameObject queue;

	/// <summary>
	/// Queue object. Constant reference, the queue is always the same
	/// </summary>
	GameObject kitchen;

	/// <summary>
	/// Table object. Variable reference, the specific table is set when assessing priorit
	/// </summary>		   
	GameObject table;

	bool ShouldReceiveCustomer,
		ShouldAttendCustoemr,
		ShouldServeFood,
		ShouldBringBill;

	static private Status GoTo(Vector3 pos)
	{
		return Status.ERROR;
	}

	static private Status GoTo(GameObject objectPos)
	{
		Vector3 pos = objectPos.transform.position;

		return Status.ERROR;
	}

	static private Status AssessPriority()
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
						.If("ShouldReceiveCustomer", () => { return false; })
						.Do("GoToQueue", () => { return GoTo(queue); })
						.Do("BringCustomerToTable", () => { return Status.ERROR; })
						.Do("GiveMenuToCustomer", () => { return Status.ERROR; })
					.End()
					.Sequence("AttendCustomer")
						.If("ShouldAttendCustomer", () => { return false; })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GetOrderFromCustomer", () => { return Status.ERROR; })
						.Do("GetMenuFromCustomer", () => { return Status.ERROR; })
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("GiveOrderToKitchen", () => { return Status.ERROR; })
					.End()
					.Sequence("ServeFood")
						.If("ShouldServeFood", () => { return false; })
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("PickupFood", () => { return Status.ERROR; })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GiveFoodToCustomer", () => { return Status.ERROR; })
					.End()
					.Sequence("BringBill")
						.If("ShouldBringBill", () => { return false; })
						.Do("GoToKitchen", () => { return GoTo(kitchen); })
						.Do("PickupBill", () => { return Status.ERROR; })
						.Do("GoToTable", () => { return GoTo(table); })
						.Do("GiveBillToCustomer", () => { return Status.ERROR; })
						.Do("GetMoneyFromCustomer", () => { return Status.ERROR; })
						.Do("CleanTable", () => { return Status.ERROR; })
						.End()
					.End()
				.End()
			.End();

	}

	// Update is called once per frame
	void Update()
	{

	}
}
