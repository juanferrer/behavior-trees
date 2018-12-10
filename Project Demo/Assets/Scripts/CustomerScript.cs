using UnityEngine;
using FluentBehaviorTree;
using Data;

public class CustomerScript : MonoBehaviour
{
	/// <summary>
    /// The behaviour tree of this agent
    /// </summary>
	BehaviorTree bt;

	/// <summary>
	///	Subtree called inside the main bt
	/// </summary>
	BehaviorTree leaveSequence;

	/// <summary>
	/// Queue object. Constant reference, the queue is always the same
	/// </summary>
	GameObject queue;

	/// <summary>
	/// Table object. Variable reference, the specific table is set when received by waiter
	/// </summary>
	GameObject table;

	/// <summary>
	/// Time available for customer to perform all activities
	/// </summary>
    int AvailableTime;
    Food FoodToOrder;
	
	static private Status GoTo(Vector3 pos)
	{
		return Status.ERROR;
	}

	static private Status GoTo(GameObject objectPos)
	{
		Vector3 pos = objectPos.transform.position;

		return Status.ERROR;
	}

	static private Status Leave()
	{
		// TODO: 
		return Status.ERROR;
	}

	// Use this for initialization
	void Start()
	{
		// TODO: Get references to each object

		leaveSequence = new BehaviorTreeBuilder("")
		.Sequence("LeaveSequence")
			.If("WaitedTooLong",() => { return false; })
			.Do("Leave", () => { return Leave(); })
			.End()
		.End();

		// TODO: Declare BT
		bt = new BehaviorTreeBuilder("")
		.RepeatUntilFail("Loop")
			.Sequence("Sequence")
				.Do("StartQueue", () => { return Status.ERROR; })
				.Selector("Selector")
					.Selector("HasBeenReceived")
						.Do("LeaveSequence", leaveSequence)
						.If("IsBeingReceived",() => { return false; })
						.Do("GoToTable", () => { return GoTo(table); })
						.End()
					.Selector("HasBeenAttended")
						.Do("LeaveSequence", leaveSequence)
						.If("IsBeingAttended",() => { return false; })
						.Do("MakeOrder", () => { return Status.ERROR; })
						.End()
					.Selector("HasBeenServed")
						.Do("LeaveSequence", leaveSequence)
						.If("IsBeingServed",() => { return false; })
						.Do("Eat", () => { return Status.ERROR; })
						.Wait("Wait", 0)
							.Do("RequestBill", () => { return Status.ERROR; })
						.End()
					.Selector("HasReceivedBill")
						.Do("LeaveSequence", leaveSequence)
						.If("IsReceivingBill",() => { return false; })
						.Do("PayBill", () => { return Status.ERROR; })
						.Do("Leave", () => { return Leave(); })
						.End()
					.End()
				.End()
			.End();
		// TODO: Set properties of customer
		var values = System.Enum.GetValues(typeof(Data.Food));
		FoodToOrder = (Food)values.GetValue(Random.Range(0, values.Length));

		AvailableTime = Random.Range(Globals.MinWaitTime, Globals.MaxWaitTime);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
