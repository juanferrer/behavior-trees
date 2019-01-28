using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlackboardScript : MonoBehaviour
{
    private GameManagerScript gm;
    private QueueScript queue;
    public List<TableScript> tables;
    public List<WorktopScript> worktops;
    public List<OvenScript> ovens;

    public int CustomersInQueueCount { get; private set; }
    public int CustomersToAttendCount { get; private set; }
    public int EmptyTablesCount { get; private set; }
    public int EmptyWorktopsCount { get; private set; }
    public int EmptyOvensCount { get; private set; }
    public WaiterScript WaiterTakingCareOfQueue { get; private set; }

    public BlackboardScript()
    {
        CustomersInQueueCount = 0;
    }

    public TableScript GetEmptyTable()
    {
        if (EmptyTablesCount == 0) return null;
        var emptyTables = tables.Where(table => !table.IsOccupied && !table.IsAssigned);
        return emptyTables.ElementAt(Random.Range(0, emptyTables.Count()));
    }

    public WorktopScript GetEmptyWorktop()
    {
        if (EmptyWorktopsCount == 0) return null;
        var emptyWorktops = worktops.Where(worktop => !worktop.IsOccupied);
        return emptyWorktops.ElementAt(Random.Range(0, emptyWorktops.Count()));
    }

    public OvenScript GetEmptyOven()
    {
        if (EmptyOvensCount == 0) return null;
        var emptyOvens = ovens.Where(oven => !oven.IsOccupied);
        return emptyOvens.ElementAt(Random.Range(0, emptyOvens.Count()));
    }

public TableScript GetTableToAttend()
    {
        if (CustomersToAttendCount == 0) return null;
        var occupiedTablesWaitingToBeAttended = tables.Where(table => table.IsOccupied && !table.Customer.HasBeenAttended);
        return occupiedTablesWaitingToBeAttended.ElementAt(Random.Range(0, occupiedTablesWaitingToBeAttended.Count()));
    }

    public void SetTakingCareOfQueue(WaiterScript waiter)
    {
        WaiterTakingCareOfQueue = waiter;
    }

    private void Start()
    {
        gm = GetComponent<GameManagerScript>();
        queue = gm.queue;
        tables.AddRange(GameObject.FindGameObjectsWithTag("Table").Select(table => table.GetComponent<TableScript>()));
        worktops.AddRange(GameObject.FindGameObjectsWithTag("Worktop").Select(worktop => worktop.GetComponent<WorktopScript>()));
    }

    private void LateUpdate()
    {
        CustomersInQueueCount = queue.CustomerCount();
        EmptyTablesCount = 0;
        EmptyWorktopsCount = 0;
        CustomersToAttendCount = 0;
        foreach (var table in tables)
        {
            if (!table.IsOccupied && !table.IsAssigned) ++EmptyTablesCount;
            else if (!table.Customer?.HasBeenAttended ?? false) ++CustomersToAttendCount;
        }
        foreach (var worktop in worktops)
        {
            if (!worktop.IsOccupied) ++EmptyWorktopsCount;
        }
        foreach (var oven in ovens)
        {
            if (!oven.IsOccupied) ++EmptyOvensCount;
        }
    }
}
