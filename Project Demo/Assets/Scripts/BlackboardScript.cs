using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlackboardScript : MonoBehaviour
{
    private GameManagerScript gm;
    private QueueScript queue;
    public List<TableScript> tables;

    public int CustomersInQueueCount { get; private set; }
    public int CustomersToAttendCount { get; private set; }
    public int EmptyTablesCount { get; private set; }

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

    public TableScript GetTableToAttend()
    {
        if (CustomersToAttendCount == 0) return null;
        var occupiedTablesWaitingToBeAttended = tables.Where(table => table.IsOccupied && !table.Customer.HasBeenAttended);
        return occupiedTablesWaitingToBeAttended.ElementAt(Random.Range(0, occupiedTablesWaitingToBeAttended.Count()));
    }

    private void Start()
    {
        gm = GetComponent<GameManagerScript>();
        queue = gm.queue;
        tables.AddRange(GameObject.FindGameObjectsWithTag("Table").Select(table => table.GetComponent<TableScript>()));
    }

    private void LateUpdate()
    {
        CustomersInQueueCount = queue.CustomerCount();
        EmptyTablesCount = 0;
        CustomersToAttendCount = 0;
        foreach (var table in tables)
        {
            if (!table.IsOccupied && !table.IsAssigned) ++EmptyTablesCount;
            else if (!table.Customer?.HasBeenAttended ?? false) ++CustomersToAttendCount;
        }
    }
}
