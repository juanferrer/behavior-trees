using UnityEngine;
using FluentBehaviorTree;
using UnityEngine.AI;
using Data;

public class WaiterScript : MonoBehaviour
{
    GameManagerScript gm;                   // Game manager
    NavMeshAgent agent;                     // Navigation agent
    BehaviorTree bt;                        // Behaviour tree
    BehaviorTree bringOrderSequence;        // Subtree for getting the order to the kitchen
    BehaviorTree bringFoodToTableSequence;  // Subtree for giving food to customer
    BehaviorTree getFoodSequence;           // Subtree for fetching food from kitchen
    BehaviorTree receiveCustomerSequence;   // Subtree for receiving customer
    BehaviorTree getBillSequence;           // Subtree for fetching bill from kitchen
    BehaviorTree bringBillSequence;         // Subtree for giving bill to customer
    BehaviorTree attendCustomerSequence;    // Subtree for attending waiting customer
    QueueScript queue;                      // Queue
    KitchenScript kitchen;                  // Kitchen
    TableScript table;                      // Usually empty. Set when going to a certain table
    CustomerScript customer;                // Usually empty. Set when interacting with a customer
    BlackboardScript blackboard;            // Global information
    public Inventory Inventory;             // Inventory of entity

    private bool isRecalculatingTree = false;
    private bool alreadyGoingSomewhereThisFrame = false;

    /// <summary>
    /// Use Unity's meshnav to travel to given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Status GoTo(Vector3 pos)
    {
        if (alreadyGoingSomewhereThisFrame)
        {
            return Status.FAILURE;
        }
        alreadyGoingSomewhereThisFrame = true;
        if (agent.destination != transform.position && (agent.destination - pos).sqrMagnitude > agent.stoppingDistance * 2)
        {
            agent.ResetPath();
            return Status.FAILURE;
        }

        // Set a new destination
        if (agent.isStopped)
        {
            agent.SetDestination(pos);
            agent.isStopped = false;
            return Status.RUNNING;
        }

        bool reachedPos = !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance) && (!agent.hasPath || Mathf.Approximately(agent.velocity.sqrMagnitude, 0.0f));
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
    /// Check if waiter is close enough to object
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool CloseEnough(MonoBehaviour obj)
    {
        return (transform.position - obj.transform.position).sqrMagnitude <= (agent.stoppingDistance * 2);
    }

    /// <summary>
    /// Transfer order from entity's inventory to kitchen
    /// </summary>
    /// <returns></returns>
    private Status GiveOrderToKitchen()
    {
        if (!CloseEnough(kitchen))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        if (!Inventory.order.table.Customer.IsWaiting)
        {
            Inventory.order = new Food();
            return Status.FAILURE;
        }
        kitchen.AddOrder(Inventory.order);
        Inventory.order = new Food();
        Debug.Log("Waiter left order in kitchen");
        return Status.SUCCESS;
    }

    /// <summary>
    /// Get prepared food from kitchen's inventory
    /// </summary>
    /// <returns></returns>
    private Status GetFoodFromKitchen()
    {
        if (!CloseEnough(kitchen))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
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
        Debug.Log("Waiter got food from kitchen");
        return Status.SUCCESS;
    }

    /// <summary> 
    /// 
    /// </summary>
    /// <returns></returns>
    private Status ServeFood()
    {
        if (!CloseEnough(Inventory.food.table.Customer))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        if (!Inventory.food.table.Customer.IsWaiting)
        {
            Inventory.food = new Food();
            return Status.FAILURE;
        }
        Inventory.food.table.Customer.Serve();
        Inventory.GiveTo(ItemType.FOOD, Inventory.food.table.Customer.Inventory);
        Debug.Log("Waiter served food");
        return Status.SUCCESS;
    }

    /// <summary>
    /// Get bill from kitchen's inventory
    /// </summary>
    /// <returns></returns>
    private Status GetBillFromKitchen()
    {
        if (!CloseEnough(kitchen))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        Inventory.bill = true;
        table = kitchen.GetBill();
        if (!table.Customer.IsWaiting)
        {
            // Customer left... Well, destroy the bill
            Inventory.bill = false;
            table = null;
            return Status.FAILURE;
        }
        Debug.Log("Waiter got bill from kitchen");
        return Status.SUCCESS;
    }

    private Status GiveBillToCustomer()
    {
        if (!CloseEnough(table.Customer))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        if (!table.Customer.IsWaiting)
        {
            // Customer left... Well, destroy the bill
            Inventory.bill = false;
            table = null;
            return Status.FAILURE;
        }
        table.Customer.BringBill();
        Inventory.GiveTo(ItemType.BILL, table.Customer.Inventory);
        Debug.Log("Waiter gave bill to customer");
        return Status.SUCCESS;
    }

    /// <summary>
    /// Check what customer has not been attended yet
    /// </summary>
    /// <returns></returns>
    private Status GetCustomerToAttend()
    {
        table = blackboard.GetTableToAttend();
        customer = table.Customer;
        return Status.SUCCESS;
    }

    /// <summary>
    /// Attend customer and get their order
    /// </summary>
    /// <returns></returns>
    private Status AttendCustomer()
    {
        if (!CloseEnough(customer))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        if (!customer.IsWaiting)
        {
            customer = null;
            return Status.FAILURE;
        }
        Inventory.GetFrom(ItemType.ORDER, customer.Inventory);
        customer.Attend();
        table.HasWaiterEnRoute = false;

        // Now that the customer has been attended, so prepare for next customer
        customer = null;
        table = null;

        Debug.Log("Waiter attended customer");
        return Status.SUCCESS;
    }

    /// <summary>
    /// Get an empty table and send the first customer from the queue to that table
    /// </summary>
    /// <returns></returns>
    private Status SendCustomerToTable()
    {
        if (!CloseEnough(queue))
        {
            isRecalculatingTree = true;
            return Status.FAILURE;
        }
        Debug.Log("Waiter sent customer to table");
        table = blackboard.GetEmptyTable();
        customer = queue.GetNextCustomer();
        customer.Receive(table);
        table.IsAssigned = true;

        // Now that the customer has been received, so prepare for next customer
        customer = null;
        table = null;

        // And, if there's no customers left, tell everyone you're not in the queue anymore
        if (blackboard.WaiterTakingCareOfQueue == this && blackboard.CustomersInQueueCount == 0) blackboard.SetTakingCareOfQueue(null);

        return Status.SUCCESS;
    }

    private Status SetGoingToQueue()
    {
        blackboard.SetTakingCareOfQueue(this);
        return Status.SUCCESS;
    }

    /// <summary>
    /// Clean the table after client left
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private Status CleanTable(TableScript table)
    {
        Debug.Log("Waiter cleaned table");
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

        // Do we have a customer's order? We should be taking that to the kitchen
        bringOrderSequence = new BehaviorTreeBuilder("BringOrderSequence")
            .Sequence("BringOrder")
                .Sequence("ShouldBringOrder")
                    .If("HasOrder", () => { return Inventory.Has(ItemType.ORDER); })
                    .Not("IsNotRecalculating")
                        .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GoToKitchen", () => { return GoTo(kitchen); })
                .Do("GiveOrderToKitchen", GiveOrderToKitchen)
                .End()
            .End();

        // We need to keep dishes warm (and we have one in our hands!), so do that now
        bringFoodToTableSequence = new BehaviorTreeBuilder("BringFoodToTableSequence")
            .Sequence("BringFoodToTable")
                .Sequence("ShouldServeFood")
                    .If("HasFood", () => { return Inventory.Has(ItemType.FOOD); })
                    .Not("IsNotRecalculating")
                        .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GoToTable", () => { return GoTo(table.Customer); })
                .Do("GiveFoodToCustomer", ServeFood)
                .End()
            .End();

        // We need to keep dishes warm, so do that now
        getFoodSequence = new BehaviorTreeBuilder("GetFoodSequence")
           .Sequence("GetFood")
                .Sequence("ShouldServeFood")
                    .If("IsFoodPrepared", kitchen.IsFoodPrepared)
                    .Not("DoesNotHaveFood")
                        .If("HasFood", () => { return Inventory.Has(ItemType.FOOD); })
                    .Not("IsNotRecalculating")
                        .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GoToKitchen", () => { return GoTo(kitchen); })
                .Do("PickupFood", GetFoodFromKitchen)
                .End()
            .End();

        // We have no food ready to be served
        // There needs to be free tables before we receive a new customer
        // Also, make sure someone is in the queue
        receiveCustomerSequence = new BehaviorTreeBuilder("ReceiveCustomerSequence")
            .Sequence("ReceiveCustomer")
                .Sequence("ShouldReceiveCustomer")
                    .If("NoOneOrMeIsTakingCareOfQueue", () => { return blackboard.WaiterTakingCareOfQueue == null || blackboard.WaiterTakingCareOfQueue == this; })
                    .If("AreThereEmptyTables", () => { return blackboard.EmptyTablesCount > 0; })
                    .If("AreThereCustomersInQueue", () => { return blackboard.CustomersInQueueCount > 0; })
                    .Not("IsNotRecalculating")
                        .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("SetGoingToQueue", SetGoingToQueue)
                .Do("GoToQueue", () => { return GoTo(queue); })
                .Do("SendCustomerToTable", SendCustomerToTable)
                .End()
            .End();

        // Someone is already receiving customers or there is noone to receive
        // Bring bill to customers, lest they leave without paying
        // STOP THE SINPA! (https://en.wiktionary.org/wiki/sinpa)
        getBillSequence = new BehaviorTreeBuilder("GetBillSequence")
            .Sequence("GetBill")
                .Sequence("ShouldGetBill")
                    .If("IsBillReady", kitchen.IsBillReady)
                    .Not("DoesNotHaveBill")
                        .If("HasBill", () => { return Inventory.Has(ItemType.BILL); })
                    .Not("DoesNotHaveFood")
                        .If("HasFood", () => { return Inventory.Has(ItemType.FOOD); })
                    .Not("IsNotRecalculating")
                        .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GoToKitchen", () => { return GoTo(kitchen); })
                .Do("PickupBill", GetBillFromKitchen)
                .End()
            .End();

        // We have the bill and need to take it to the customer
        bringBillSequence = new BehaviorTreeBuilder("BringBillSequence")
            .Sequence("BringBill")
                .Sequence("ShouldBringBill")
                    .If("HasBill", () => { return Inventory.Has(ItemType.BILL); })
                    .Not("IsNotRecalculating")
                        .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GoToTable", () => { return GoTo(table.Customer); })
                .Do("GiveBillToCustomer", GiveBillToCustomer)
                .Do("GetMoneyFromCustomer", () => { return Inventory.GetFrom(ItemType.MONEY, customer.Inventory); })
                .Do("CleanTable", () => { return CleanTable(table); })
                .End()
            .End();

        // Since the bills have been taken care of, check if any sitting customer
        // is still to be attended
        attendCustomerSequence = new BehaviorTreeBuilder("AttendCustomerSequence")
            .Sequence("AttendCustomer")
                .Sequence("ShouldAttendCustomer")
                    .If("AreThereCustomerToAttend", () => { return blackboard.CustomersToAttendCount > 0; })
                    .Not("IsNotRecalculating")
                        .If("Recalculating", () => { return isRecalculatingTree; })
                    .End()
                .Do("GetCustomerToAttend", GetCustomerToAttend)
                .Do("GoToCustomer", () => { return GoTo(customer); })
                .Do("Attend", AttendCustomer)
                .End()
            .End();

        // Declare BT
        bt = new BehaviorTreeBuilder("WaiterBT")
        .RepeatUntilFail("Loop")
            .Selector("Selector")
                .Do("BringOrderSequence", bringOrderSequence)
                .Do("BringFoodToTableSequence", bringFoodToTableSequence)
                .Do("GetFoodSequence", getFoodSequence)
                .Do("ReceiveCustomerSequence", receiveCustomerSequence)
                .Do("GetBillSequence", getBillSequence)
                .Do("BringBillSequence", bringBillSequence)
                .Do("AttendCustomerSequence", attendCustomerSequence)
                .Do("JustWait", () => {
                    isRecalculatingTree = false;
                    return Status.SUCCESS;
                })
                .End()
        .End();
    }

    // Update is called once per frame
    void Update()
    {
        bt.Tick();
        alreadyGoingSomewhereThisFrame = false;
    }
}
