using UnityEngine;
using FluentBehaviorTree;
using Data;
using UnityEngine.AI;

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
	QueueScript queue;

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
	
	NavMeshAgent agent;

	int AvailableTime;
	Food FoodToOrder;


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
			agent.destination = pos;
            agent.isStopped = false;
            return Status.RUNNING;
        }

        bool reachedPos = !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance) && (!agent.hasPath || agent.velocity.sqrMagnitude == 0f);
        bool onTheWay = agent.remainingDistance != Mathf.Infinity && (agent.remainingDistance > agent.stoppingDistance);

        if (reachedPos)
        {
            agent.isStopped = true;
            return Status.SUCCESS;
        }
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

	private Status StartQueue()
	{
		return Status.FAILURE;
	}

	// Use this for initialization
	void Start()
	{
		// Get references to each object
		gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
		queue = gm.queue.GetComponent<QueueScript>();
		exit = gm.exit;
		agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        // Declare BTs

        leaveSequence = new BehaviorTreeBuilder("")
		.Sequence("LeaveSequence")
			.If("WaitedTooLong", () => { return false; })
			.Do("Leave", () => { return Leave(); })
			.End()
		.End();

		bt = new BehaviorTreeBuilder("")
		.RepeatUntilFail("Loop")
			.Sequence("Sequence")
				.Do("GoToQueue", () => { return GoTo(queue.GetNextPosition()); })
				.Do("StartQueue", () => { return StartQueue(); })
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
						.Wait("Wait", 5)
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
		bt.Tick();
	}
}
