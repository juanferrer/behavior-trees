using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardScript : MonoBehaviour
{
    private GameManagerScript gm;
    private QueueScript queue;

    public int CustomersInQueueCount { get; private set; }
    public bool WaiterInQueue { get; private set; }

    public BlackboardScript()
    {
        CustomersInQueueCount = 0;
        WaiterInQueue = false;
    }

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        queue = gm.queue.GetComponent<QueueScript>();
    }

    private void LateUpdate()
    {
        CustomersInQueueCount = queue.CustomerCount();
        WaiterInQueue = queue.IsWaiterInQueue();
    }
}
