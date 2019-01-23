
using UnityEngine;
using FluentBehaviorTree;
using Data;
using UnityEngine.AI;
using System;

public class CustomerScript : MonoBehaviour
{
    GameManagerScript gm;           // Game manager
    NavMeshAgent agent;             // Navigation agent
    BehaviorTree bt;                // Behaviour tree
    BehaviorTree leaveCheck;        // Subtree to check whether available time has passed
    QueueScript queue;              // Queue
    KitchenScript kitchen;          // Kitchen
    TableScript table;              // Usually empty. Set when going to a certain table
    GameObject exit;                // Exit
    BlackboardScript blackboard;    // Global information
    public Inventory Inventory;     // Inventory of entity

    int availableTime;
    float timeWaited = 0;
    float timeEating = 3;

    bool isInQueue = false;
    bool isInTable = false;
    bool isBeingReceived = false;
    public bool HasBeenReceived { get; private set; } = false;
    public bool HasBeenAttended { get; private set; } = false;
    public bool HasBeenServed { get; private set; } = false;
    public bool HasReceivedFood { get; private set; } = false;
    public bool HasFinishedEating { get; private set; } = false;
    public bool HasReceivedBill { get; private set; } = false;
    public bool IsWaiting { get; private set; } = true;


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

    /// <summary>
    /// Go to queue and start queueing
    /// </summary>
    /// <returns></returns>
	private Status StartQueue()
	{
        queue.StartQueue(this);
        isInQueue = true;
        Debug.Log("Start queue");
        return Status.SUCCESS;
	}

    /// <summary>
    /// Wait and check if available time has passed
    /// </summary>
    /// <returns></returns>
    private Status Wait()
    {
        timeWaited += Time.deltaTime;
        if (timeWaited > availableTime)
        {
            IsWaiting = false;
            return Status.FAILURE;
        }
        return Status.SUCCESS;
    }

    /// <summary>
    /// Go to exit and leave
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Sit in assigned table
    /// </summary>
    /// <returns></returns>
    private Status SitInTable()
    {
        // TODO
        // Register to table events?
        //timeWaited = 0;
        Debug.Log("Customer sitting in table");
        HasBeenReceived = true;
        isBeingReceived = false;
        isInTable = true;
        table.SetCustomer(this);
        return Status.SUCCESS;
    }

    /// <summary>
    /// Read menu and decide what to order
    /// </summary>
    /// <returns></returns>
    private Status DecideFood()
    {
        Debug.Log("Customer decided what to order");
        var values = Enum.GetValues(typeof(FoodType));
        Inventory.order.food = (FoodType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        Inventory.order.table = table;
        return Status.SUCCESS;
    }

    /// <summary>
    /// Eat ordered food
    /// </summary>
    /// <returns></returns>
    private Status Eat()
    {
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

    /// <summary>
    /// Pay bill before leaving
    /// </summary>
    /// <returns></returns>
    private Status PayBill()
    {
        // TODO
        Debug.Log("Customer payed bill");
        return Status.SUCCESS;
    }

    /// <summary>
    /// Be received
    /// </summary>
    /// <param name="newTable"></param>
    public void Receive(TableScript newTable)
    {
        queue.LeaveQueue(this);
        isInQueue = false;
        isBeingReceived = true;
        table = newTable;
    }

    /// <summary>
    /// Be attended
    /// </summary>
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
                    .If("NoFoodDecided", () => { return isInTable && !HasBeenAttended && !Inventory.Has(ItemType.ORDER); })
                    .Do("DecideFood", DecideFood)
                    .End()
                .Sequence("BeAttendedSequence")
                    .If("NeedsToBeAttended", () => { return ( isInTable && HasBeenReceived && !HasBeenAttended); })
                    .Do("LeaveCheck", leaveCheck)
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
