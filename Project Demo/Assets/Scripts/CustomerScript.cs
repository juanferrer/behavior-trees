
using UnityEngine;
using FluentBehaviorTree;
using Data;
using UnityEngine.AI;
using System;

public class CustomerScript : MonoBehaviour
{
    GameManagerScript gm;                   // Game manager
    NavMeshAgent agent;                     // Navigation agent
    BehaviorTree bt;                        // Behaviour tree
    BehaviorTree leaveCheck;                // Subtree to check whether available time has passed
    BehaviorTree queueSequence;             // Subtree for starting to queue
    BehaviorTree tableSequence;             // Subtree for going to table
    BehaviorTree decideOrderSequence;       // Subtree for deciding on order
    BehaviorTree beAttendedSequence;        // Subtree for waiting to be attended
    BehaviorTree beServedAndEatSequence;    // Subtree for waiting to be served and eat
    BehaviorTree waitBillSequence;          // Subtree for waiting bill and pay
    QueueScript queue;                      // Queue
    KitchenScript kitchen;                  // Kitchen
    TableScript table;                      // Usually empty. Set when going to a certain table
    GameObject exit;                        // Exit
    BlackboardScript blackboard;            // Global information
    public Inventory Inventory;             // Inventory of entity

    int availableTime;
    float timeWaited = 0;
    ulong timeEating = 3;

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
        Destroy(gameObject);
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
        HasFinishedEating = true;
        Debug.Log("Customer finished eating");
        return Status.SUCCESS;
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
        timeEating = (ulong)UnityEngine.Random.Range(3000, 5000);

        // Declare BTs

        leaveCheck = new BehaviorTreeBuilder("")
            .Selector("LeaveSelector")
                .Do("Wait", Wait)
                .Do("Leave", () => { return Leave(); })
                .End()
            .End();

        queueSequence = new BehaviorTreeBuilder("QueueSequence")
            .Sequence("QueueSequence")
                .Sequence("NeedsToStartQueue")
                    .Not("IsNotInQueue")
                        .If("IsInQueue", () => { return isInQueue; })
                    .Not("IsNotBeingReceived")
                        .If("IsBeingReceived", () => { return isBeingReceived; })
                    .Not("HasNotBeenReceived")
                        .If("HasBeenReceived", () => { return HasBeenReceived; })
                    .End()
                .Do("GoToQueue", () => { return GoTo(queue.GetNextPosition()); })
                .Do("StartQueue", StartQueue)
                .End()
            .End();

        tableSequence = new BehaviorTreeBuilder("TableSequence")
            .Sequence("TableSequence")
                .Sequence("NeedsToGetATable")
                    .Not("HasNotBeenReceived")
                        .If("HasBeenReceived", () => { return HasBeenReceived; })
                    .End()
                .Do("LeaveCheck", leaveCheck)
                .Sequence("ReadyToBeReceived")
                    .If("IsBeingReceived", () => { return isBeingReceived; })
                    .End()
                .Do("GoToTable", () => { return GoTo(table); })
                .Do("SitInTable", SitInTable)
                .End()
            .End();

        decideOrderSequence = new BehaviorTreeBuilder("DecideOrderSequence")
            .Sequence("DecideWhatToOrder")
                .Sequence("NoFoodDecided")
                    .If("IsInTable", () => { return isInTable; })
                    .Not("HasNotBeenAttended")
                        .If("HasBeenAttended", () => { return HasBeenAttended; })
                    .Not("DoesNotHaveOrder")
                        .If("HasOrder", () => { return Inventory.Has(ItemType.ORDER); })
                    .End()
                .Do("DecideFood", DecideFood)
                .End()
            .End();

        beAttendedSequence = new BehaviorTreeBuilder("BeAttendedSequence")
            .Sequence("BeAttendedSequence")
                .Sequence("NeedsToBeAttended")
                    .If("IsInTable", () => { return isInTable; })
                    .If("HasBeenReceived", () => { return HasBeenReceived; })
                    .Not("HasNotBeenAttended")
                        .If("HasBeenAttended", () => { return HasBeenAttended; })
                    .End()
                .Do("LeaveCheck", leaveCheck)
                .End()
            .End();

        beServedAndEatSequence = new BehaviorTreeBuilder("BeServedAndEatSequence")
            .Sequence("BeServedAndEatSequence")
                .Sequence("NeedsToBeServed")
                    .If("IsInTable", () => { return isInTable; })
                    .If("HasBeenAttended", () => { return HasBeenAttended; })
                    .Not("HasNotBeenServed")
                        .If("HasBeenServed", () => { return HasBeenServed; })
                    .Not("HasNotFinishedEating")
                        .If("HasFinishedEating", () => { return HasFinishedEating; })
                    .End()
                .Do("LeaveCheck", leaveCheck)
                .If("HasFood", () => { return HasReceivedFood; })
                .Wait("Eat", timeEating)
                    .Do("Eat", Eat)
                .End()
            .End();

        waitBillSequence = new BehaviorTreeBuilder("WaitBillSequence")
            .Sequence("WaitForBillSequence")
                .Sequence("NeedsToBeServed")
                    .If("IsInTable", () => { return isInTable; })
                    .If("HasFinishedEating", () => { return HasFinishedEating; })
                    .End()
                .Do("LeaveCheck", leaveCheck)
                .If("HasReceivedBill", () => { return HasReceivedBill; })
                .Do("PayBill", PayBill)
                .Do("Leave", Leave)
                .End()
            .End();

        // Debug DLL in Unity:
        // https://www.robinryf.com/blog/2016/04/10/convert-pdb-to-mdb.html
        bt = new BehaviorTreeBuilder("CustomerBT")
            .Selector("Selector")
                .Do("QueueSequence", queueSequence)
                .Do("TableSequence", tableSequence)
                .Do("DecideOrderSequence", decideOrderSequence)
                .Do("BeAttendedSequence", beAttendedSequence)
                .Do("BeServedAndEatSequence", beServedAndEatSequence)
                .Do("WaitBillSequence", waitBillSequence)
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
