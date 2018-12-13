using UnityEngine;
using FluentBehaviorTree;
using Data;

public class CustomerScript : MonoBehaviour
{

	GameManagerScript gm;
	/// <summary>
	/// The behaviour tree of this agent
	/// </summary>
	BehaviorTree bt;

	/// <summary>
	///	Subtree called inside the main bt
	/// </summary>
	BehaviorTree leaveSequence;

	/// <summary>
	/// Queue object. Constant reference, queue is always the same
	/// </summary>
	GameObject queue;

	/// <summary>
	/// Table object. Variable reference, set when received by waiter
	/// </summary>
	GameObject table;

	/// <summary>
	/// Exit object. Constant reference, exit is always the same
	/// </summary>
	GameObject exit;

	/// <summary>
	/// Time available for customer to perform all activities
	/// </summary>
	int AvailableTime;
	Food FoodToOrder;


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
	/// <param name="objectPos"></param>
	/// <returns></returns>
	private Status GoTo(GameObject objectPos)
	{
		Vector3 pos = objectPos.transform.position;

		return GoTo(pos);
	}

	private Status Leave()
	{
		var status = GoTo(exit);
		// Send this to pool or destroy
		return status;
	}

	// Use this for initialization
	void Start()
	{
		// TODO: Get references to each object
		gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		queue = gm.queue;
		exit = gm.exit;

		leaveSequence = new BehaviorTreeBuilder("")
		.Sequence("LeaveSequence")
			.If("WaitedTooLong", () => { return false; })
			.Do("Leave", () => { return Leave(); })
			.End()
		.End();

		// TODO: Declare BT
		bt = new BehaviorTreeBuilder("")
		.RepeatUntilFail("Loop")
			.Sequence("Sequence")
				.Do("GoToQueue", () => { return GoTo(queue); })
				.Selector("Selector")
					.Selector("HasBeenReceived")
						.Do("LeaveSequence", leaveSequence)
						.If("IsBeingReceived", () => { return false; })
						.Do("GoToTable", () => { return GoTo(table); })
						.End()
					.Selector("HasBeenAttended")
						.Do("LeaveSequence", leaveSequence)
						.If("IsBeingAttended", () => { return false; })
						.Do("MakeOrder", () => { return Status.ERROR; })
						.End()
					.Selector("HasBeenServed")
						.Do("LeaveSequence", leaveSequence)
						.If("IsBeingServed", () => { return false; })
						.Do("Eat", () => { return Status.ERROR; })
						.Wait("Wait", 0)
							.Do("RequestBill", () => { return Status.ERROR; })
						.End()
					.Selector("HasReceivedBill")
						.Do("LeaveSequence", leaveSequence)
						.If("HasReceivedBill", () => { return false; })
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
