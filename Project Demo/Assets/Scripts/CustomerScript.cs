
using UnityEngine;
using FluentBehaviorTree;
using Data;
using UnityEngine.AI;
using System;

public class CustomerScript : MonoBehaviour
{
	GameManagerScript gm;
	BehaviorTree bt;
	BehaviorTree leaveCheck;
	QueueScript queue;
	TableScript table;
	GameObject exit;
	NavMeshAgent agent;
    public Inventory Inventory;

	int availableTime;
	Food foodToOrder;
    float timeWaited = 0;
    float timeEating = 3;

    bool isInQueue = false;
    bool isInTable = false;
    //bool isBeingAttended = false;
    bool isBeingReceived = false;
    public bool HasBeenReceived { get; private set; } = false;
    public bool HasBeenAttended { get; private set; } = false;
    public bool HasBeenServed { get; private set; } = false;
    public bool HasReceivedFood { get; private set; } = false;
    public bool HasFinishedEating { get; private set; } = false;
    public bool HasReceivedBill { get; private set; } = false;


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

	private Status StartQueue()
	{
        queue.StartQueue(this);
        isInQueue = true;
        Debug.Log("Start queue");
        return Status.SUCCESS;
	}

    private Status Wait()
    {
        timeWaited += Time.deltaTime;
        if (timeWaited > availableTime) return Status.FAILURE;
        return Status.SUCCESS;
    }

    private Status Leave()
    {
        var status = GoTo(exit.transform.position);
        // If it was in Queue, leave queue
        isInQueue = false;
        isInTable = false;
        queue.LeaveQueue(this);
        if (status == Status.SUCCESS) gameObject.SetActive(false);
        // Send this to pool or destroy
        Debug.Log("Customer left");
        return status;
	}

    private Status SitInTable()
    {
        // TODO
        // Register to table events?
        //timeWaited = 0;
        Debug.Log("Customer sitting in table");
        HasBeenReceived = true;
        isBeingReceived = false;
        isInTable = true;
        return Status.SUCCESS;
    }

    private Status DecideFood()
    {
        Debug.Log("Customer decided what to order");
        var values = Enum.GetValues(typeof(Food));
        foodToOrder = (Food)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        return Status.SUCCESS;
    }

    private Status MakeOrder()
    {
        // TODO
        //timeWaited = 0;
        Debug.Log("Make order");
        HasBeenAttended = true;
        return Status.SUCCESS;
    }

    private Status Eat()
    {
        // Reset, so that the customer doesn't leave while eating
        //timeWaited = 0;
        // Destroy food from table
        if (!HasBeenServed) HasBeenServed = true;
        timeEating -= Time.deltaTime;

        if (timeEating <= 0)
        {
            HasFinishedEating = true;
            Debug.Log("Customer finished eating");
            return Status.SUCCESS;
        }
        return Status.RUNNING;
    }

    private Status PayBill()
    {
        // TODO
        Debug.Log("Customer payed bill");
        return Status.SUCCESS;
    }

    public void Receive(TableScript newTable)
    {
        //timeWaited = 0;
        queue.LeaveQueue(this);
        isInQueue = false;
        isBeingReceived = true;
        table = newTable;
    }

    public void Attend()
    {
        HasBeenAttended = true;
    }

    public void Serve()
    {
        HasReceivedFood = true;
    }

    public void BringBill()
    {
        HasReceivedBill = true;
    }

    // Use this for initialization
    void Start()
    {
        // Get references to each object
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        queue = gm.queue;
        exit = gm.exit;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        Inventory = new Inventory();
        Inventory.money = true;

        // Declare BTs

        leaveCheck = new BehaviorTreeBuilder("")
            .Selector("LeaveSelector")
                .Do("Wait", Wait)
                .Do("Leave", () => { return Leave(); })
                .End()
            .End();

        // Debug DLL in Unity:
        // https://www.robinryf.com/blog/2016/04/10/convert-pdb-to-mdb.html
        bt = new BehaviorTreeBuilder("")
            .Selector("Selector")
                .Sequence("QueueSequence")
                    .If("NeedsToStartQueue", () => { return !isInQueue && !isBeingReceived && !HasBeenReceived; })
                    .Do("GoToQueue", () => { return GoTo(queue.GetNextPosition()); })
                    .Do("StartQueue", StartQueue)
                    .End()
                .Sequence("TableSequence")
                    .If("NeedsToGetATable", () => { return !HasBeenReceived; })
                    .Do("LeaveCheck", leaveCheck)
                    .If("ReadyToBeReceived", () => { return isBeingReceived; })
                    .Do("GoToTable", () => { return GoTo(table); })
                    .Do("SitInTable", SitInTable)
                    .End()
                .Sequence("DecideWhatToOrder")
                    .If("NoFoodDecided", () => { return Inventory.Has(ItemType.FOOD); })
                    .Do("DecideFood", DecideFood)
                    .End()
                .Sequence("BeAttendedSequence")
                    .If("NeedsToBeAttended", () => { return ( isInTable && HasBeenReceived && !HasBeenAttended); })
                    .Do("LeaveCheck", leaveCheck)
                    /*.If("ReadyToBeAttended", () => { return isBeingAttended; })
                    .Do("MakeOrder", MakeOrder)*/
                    .End()
                .Sequence("BeServedAndEatSequence")
                    .If("NeedsToBeServed", () => { return isInTable && HasBeenAttended && !HasBeenServed && !HasFinishedEating; })
                    .Do("LeaveCheck", leaveCheck)
                    .If("HasFood", () => { return HasReceivedFood;  })
                    .Do("Eat", Eat)
                    .End()
                .Sequence("WaitForBillSequence")
                    .If("NeedsToReceiveBill", () => { return isInTable && HasFinishedEating; })
                    .Do("LeaveCheck", leaveCheck)
                    .If("HasReceivedBill", () => { return HasReceivedBill; })
                    .Do("PayBill", PayBill)
                    .Do("Leave", Leave)
                    .End()
                .End()
            .End();

        // TODO: Set properties of customer

		availableTime = UnityEngine.Random.Range(Globals.MinWaitTime, Globals.MaxWaitTime);
	}

	// Update is called once per frame
	void Update()
	{
        bt.Tick();
	}
}
