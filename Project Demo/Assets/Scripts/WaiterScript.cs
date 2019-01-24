using UnityEngine;
using FluentBehaviorTree;
using UnityEngine.AI;
using Data;

public class WaiterScript : MonoBehaviour
{
    GameManagerScript gm;           // Game manager
    NavMeshAgent agent;             // Navigation agent
    BehaviorTree bt;                // Behaviour tree
    QueueScript queue;              // Queue
    KitchenScript kitchen;          // Kitchen
    TableScript table;              // Usually empty. Set when going to a certain table
    CustomerScript customer;        // Usually empty. Set when interacting with a customer
    BlackboardScript blackboard;    // Global information
    public Inventory Inventory;     // Inventory of entity

    bool shouldReceiveCustomer;
    bool shouldAttendCustomer;
    bool shouldBringOrder;
    bool shouldServeFood;
    bool shouldBringBill;
    bool isPerformingAction;

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
            Debug.Log("Waiter reached destination: " + pos.ToString("G2"));
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
    /// Transfer order from entity's inventory to kitchen
    /// </summary>
    /// <returns></returns>
    private Status GiveOrderToKitchen()
    {
        isPerformingAction = false;
        if (!Inventory.order.table.Customer.IsWaiting)
        {
            Inventory.order = new Food();
            return Status.FAILURE;
        }
        Debug.Log("Waiter left order in kitchen");
        kitchen.AddOrder(Inventory.order);
        Inventory.order = new Food();
        return Status.SUCCESS;
    }

    /// <summary>
    /// Get prepared food from kitchen's inventory
    /// </summary>
    /// <returns></returns>
    private Status GetFoodFromKitchen()
    {
        isPerformingAction = false;
        Debug.Log("Waiter got food from kitchen");
        Inventory.food = kitchen.GetFoodPrepared();
        table = Inventory.food.table;
        if (!table.Customer.IsWaiting)
        { 
            // Customer left, throw this away
            // TODO: Maybe can reuse?
            Inventory.food = new Food();
            table = null;
            return Status.FAILURE;
        }
        return Status.SUCCESS;
    }

    /// <summary> 
    /// 
    /// </summary>
    /// <returns></returns>
    private Status ServeFood()
    {
        isPerformingAction = false;
        if (!Inventory.food.table.Customer.IsWaiting)
        {
            Inventory.food = new Food();
            customer = null;
            return Status.FAILURE;
        }
        Inventory.GiveTo(ItemType.FOOD, customer.Inventory);
        customer.Serve();
        return Status.SUCCESS;
    }

    /// <summary>
    /// Get bill from kitchen's inventory
    /// </summary>
    /// <returns></returns>
    private Status GetBillFromKitchen()
    {
        isPerformingAction = false;
        Debug.Log("Waiter got bill from kitchen");
        Inventory.bill = true;
        table = kitchen.GetBill();
        if (!table.Customer.IsWaiting)
        {
            // Customer left... Well, destroy the bill
            Inventory.bill = false;
            table = null;
            return Status.FAILURE;
        }
        return Status.SUCCESS;
    }

    private Status GiveBillToCustomer()
    {
        isPerformingAction = false;
        if (!customer.IsWaiting)
        {
            // Customer left... Well, destroy the bill
            Inventory.bill = false;
            customer = null;
            return Status.FAILURE;
        }
        Inventory.GiveTo(ItemType.BILL, customer.Inventory);
        customer.BringBill();

        return Status.SUCCESS;
    }

    /// <summary>
    /// Check what customer has not been attended yet
    /// </summary>
    /// <returns></returns>
    private Status GetCustomerToAttend()
    {
        customer = blackboard.GetTableToAttend().Customer;
        return Status.SUCCESS;
    }

    /// <summary>
    /// Attend customer and get their order
    /// </summary>
    /// <returns></returns>
    private Status AttendCustomer()
    {
        isPerformingAction = false;
        if (!customer.IsWaiting)
        {
            customer = null;
            return Status.FAILURE;
        }
        Inventory.GetFrom(ItemType.ORDER, customer.Inventory);
        customer.Attend();
        return Status.SUCCESS;
    }

    /// <summary>
    /// Decide what to do next and set the value of the appropriate boolean
    /// </summary>
    /// <returns></returns>
    private Status AssessPriority()
    {
        // TODO: Add?
        if (isPerformingAction) return Status.SUCCESS;

        shouldBringOrder = false;
        shouldReceiveCustomer = false;
        shouldAttendCustomer = false;
        shouldServeFood = false;
        shouldBringBill = false;
        isPerformingAction = false;

        // Do we have a customer's order? We should be taking that to the kitchen
        if (Inventory.Has(ItemType.ORDER))
        {
            isPerformingAction = shouldBringOrder = true;
            return Status.SUCCESS;
        }

        // We need to keep dishes warm, so that's first
        if (kitchen.IsFoodPrepared() || Inventory.Has(ItemType.FOOD))
        {
            isPerformingAction = shouldServeFood = true;
            return Status.SUCCESS;
        }

        // We have no food ready to be served
        // There needs to be free tables before we receive a new customer
        // Also, make sure someone is in the queue
        if (blackboard.EmptyTablesCount > 0 && blackboard.CustomersInQueueCount > 0)
        {
            isPerformingAction = shouldReceiveCustomer = true;
            return Status.SUCCESS;
        }

        // Someone is already receiving customers or there is noone to receive
        // Bring bill to customers, lest they leave without paying
        // STOP THE SINPA! (https://en.wiktionary.org/wiki/sinpa)
        if (kitchen.IsBillReady() || Inventory.Has(ItemType.BILL))
        {
            // TODO
            isPerformingAction = shouldBringBill = true;
            return Status.SUCCESS;
        }

        // Since the bills have been taken care of, check if any sitting customer
        // is still to be attended
        if (blackboard.CustomersToAttendCount > 0)
        {
            isPerformingAction = shouldAttendCustomer = true;
            return Status.SUCCESS;
        }

        // Wait, I guess? You're still getting paid, tho
        isPerformingAction = false;

        return Status.SUCCESS;
    }

    /// <summary>
    /// Get an empty table and send the first customer from the queue to that table
    /// </summary>
    /// <returns></returns>
    private Status SendCustomerToTable()
    {
        isPerformingAction = false;
        table = blackboard.GetEmptyTable();
        customer = queue.GetNextCustomer();
        customer.Receive(table);

        // Now that the customer has been received, so prepare for next customer
        customer = null;
        table = null;

        return Status.SUCCESS;
    }

    /// <summary>
    /// Clean the table after client left
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private Status CleanTable(TableScript table)
    {
        isPerformingAction = false;
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
        blackboard = gm.blackboard;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
        Inventory = new Inventory();


        shouldReceiveCustomer = false;
        shouldAttendCustomer = false;
        shouldServeFood = false;
        shouldBringBill = false;
        isPerformingAction = false;

        // Declare BT
        bt = new BehaviorTreeBuilder("")
        .RepeatUntilFail("Loop")
            .Selector("Selector")
                .Sequence("BringOrder")
                    .Sequence("ShouldBringOrder")
                        .If("HasOrder", () => { return Inventory.Has(ItemType.ORDER); })
                        .End()
                    .Do("GoToKitchen", () => { return GoTo(kitchen); })
                    .Do("GiveOrderToKitchen", GiveOrderToKitchen)
                    .End()
                .Sequence("BringFoodToTable")
                    .Sequence("ShouldServeFood")
                        .If("HasFood", () => { return Inventory.Has(ItemType.FOOD); })
                        .End()
                    .Do("GoToTable", () => { return GoTo(table); })
                    .Do("GiveFoodToCustomer", ServeFood)
                    .End()
                .Sequence("GetFood")
                    .Sequence("ShouldServeFood")
                        .If("IsFoodPrepared", kitchen.IsFoodPrepared)
                        .Not("DoesNotHaveFood")
                            .If("HasFood", () => { return Inventory.Has(ItemType.FOOD); })
                        .End()
                    .Do("GoToKitchen", () => { return GoTo(kitchen); })
                    .Do("PickupFood", GetFoodFromKitchen)
                    .End()
                .Sequence("ReceiveCustomer")
                    .Sequence("ShouldReceiveCustomer")
                        .If("AreThereEmptyTables", () => { return blackboard.EmptyTablesCount > 0; })
                        .If("AreThereCustomersInQueue", () => { return blackboard.CustomersInQueueCount > 0; })
                        .End()
                    .Do("GoToQueue", () => { return GoTo(queue); })
                    .Do("SendCustomerToTable", SendCustomerToTable)
                    .End()
                .Sequence("GetBill")
                    .Sequence("ShouldGetBill")
                        .If("IsBillReady", kitchen.IsBillReady)
                        .Not("DoesNotHaveBill")
                            .If("HasBill", () => { return Inventory.Has(ItemType.BILL); })
                        .End()
                    .Do("GoToKitchen", () => { return GoTo(kitchen); })
                    .Do("PickupBill", GetBillFromKitchen)
                    .End()
                .Sequence("BringBill")
                    .Sequence("ShouldBringBill")
                        .If("HasBill", () => { return Inventory.Has(ItemType.BILL); })
                        .End()
                    .Do("GoToTable", () => { return GoTo(table); })
                    .Do("GiveBillToCustomer", GiveBillToCustomer)
                    .Do("GetMoneyFromCustomer", () => { return Inventory.GetFrom(ItemType.MONEY, customer.Inventory); })
                    .Do("CleanTable", () => { return CleanTable(table); })
                    .End()
                .Sequence("AttendCustomer")
                    .Sequence("ShouldAttendCustomer")
                        .If("AreThereCustomerToAttend", () => { return blackboard.CustomersToAttendCount > 0; })
                        .End()
                    .Do("GetCustomerToAttend", GetCustomerToAttend)
                    .Do("GoToCustomer", () => { return GoTo(customer); })
                    .Do("Attend", AttendCustomer)
                    .End()
                .Do("JustWait", () => { return Status.SUCCESS; })

           
                .End()
            .End();

    }

    // Update is called once per frame
    void Update()
    {
        bt.Tick();
    }
}
