
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
	BehaviorTree leaveCheck;

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

	int availableTime;
	Food foodToOrder;
    const float customerWaitingTime = 5.0f;
    float timeWaited = 0.0f;
    bool isInQueue = false;
    bool isInTable = false;

    bool hasBeenReceived = true;
    bool hasBeenAttended = false;
    bool hasBeenServed = false;
    bool hasReceivedBill = false;


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
        // If it was in Queue, leave queue
        queue.GetComponent<QueueScript>().LeaveQueue(this);
        if (status == Status.SUCCESS) gameObject.SetActive(false);
        // Send this to pool or destroy
        return status;
	}

	private Status StartQueue()
	{
        queue.GetComponent<QueueScript>().StartQueue(this);
        isInQueue = true;
		return Status.SUCCESS;
	}

    private Status SitInTable()
    {
        // Register to table events?
        isInTable = true;
        return Status.SUCCESS;
    }

    private Status Eat()
    {
        // Destroy food from table
        return Status.SUCCESS;
    }

    private Status Wait()
    {
        timeWaited += Time.deltaTime;
        if (timeWaited > customerWaitingTime) return Status.FAILURE;
        return Status.SUCCESS;
    }


    public void StartReceiving(GameObject newTable)
    {
        hasBeenReceived = true;
        table = newTable;
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

        leaveCheck = new BehaviorTreeBuilder("")
            .Selector("LeaveSelector")
                .Do("Wait", Wait)
                .Do("Leave", () => { return Leave(); })
                .End()     
		    .End();

		bt = new BehaviorTreeBuilder("")
			.Sequence("Sequence")
                .Selector("QueueSelector")
                    .If("InQueue", () => { return isInQueue; })
                    .Sequence("MoveToQueueSequence")
				        .Do("GoToQueue", () => { return GoTo(queue.GetNextPosition()); })
				        .Do("StartQueue", StartQueue)
                        .End()
                    .End()
                .Sequence("Sequence")
					.Sequence("BeReceived")
                        .If("HasBeenReceived", () => { return hasBeenReceived; })
                        .Selector("ReceiveSelector")
                            .If("InTable", () => { return isInTable; })
                            .Sequence("MoveToTableSequence")
                                .Do("GoToTable", () => { return GoTo(table); })
                                .Do("SitInTable", SitInTable)
                                .End()                                
                            .Do("LeaveCheck", leaveCheck)
                            .End()
						.End()
					.Selector("BeAttended")
						.Do("LeaveCheck", leaveCheck)
						.If("HasBeenAttended", () => { return hasBeenAttended; })
						.Do("MakeOrder", () => { return Status.ERROR; })
						.End()
					.Selector("BeServed")
						.Do("LeaveCheck", leaveCheck)
						.If("HasBeenServed", () => { return hasBeenServed; })
						.Wait("Wait", 5)
						    .Do("Eat", Eat)
						.Do("RequestBill", () => { return Status.ERROR; })
						.End()
					.Selector("ReceiveBill")
						.Do("LeaveCheck", leaveCheck)
						.If("HasReceivedBill", () => { return hasReceivedBill; })
						.Do("PayBill", () => { return Status.ERROR; })
						.Do("Leave", () => { return Leave(); })
						.End()
					.End()
				.End()
			.End();
		// TODO: Set properties of customer
		var values = System.Enum.GetValues(typeof(Data.Food));
		foodToOrder = (Food)values.GetValue(Random.Range(0, values.Length));

		availableTime = Random.Range(Globals.MinWaitTime, Globals.MaxWaitTime);
	}

    bool stopBT = false;

	// Update is called once per frame
	void Update()
	{
        bt.Tick();
	}
}
