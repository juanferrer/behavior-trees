using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    private GameManagerScript gm;
    private QueueScript queue;

    public int CustomersInQueueCount { get; private set; }
    public int WaiterInQueue { get; private set; }

    public Blackboard()
    {
 
    }

    private void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        queue = gm.queue.GetComponent<QueueScript>();
    }

    private void LateUpdate()
    {
        CustomersInQueueCount = queue.CustomerCount();
    }
}
