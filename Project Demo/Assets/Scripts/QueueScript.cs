using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueScript : MonoBehaviour
{
    private List<CustomerScript> customerQueue;
	// Use this for initialization
	void Start ()
	{
		customerQueue = new List<CustomerScript>();
    }
	
	// Update is called once per frame
	void Update ()
	{
		
	}

    /// <summary>
    /// Get the next position in the queue
    /// </summary>
    /// <returns></returns>
	public Vector3 GetNextPosition()
	{
        var nextPosition = gameObject.transform.position;
        nextPosition.x -= (customerQueue.Count * 3.0f);
        return nextPosition;
	}

    public CustomerScript GetNextCustomer()
    {
        if (customerQueue.Count == 0) return null;
        return customerQueue[0];
    }

    public int CustomerCount()
    {
        return customerQueue.Count;
    }

    /// <summary>
    /// Allow customer to start queuing
    /// </summary>
    public void StartQueue(CustomerScript customer)
    {
        customerQueue.Add(customer);
    }

    public void LeaveQueue(CustomerScript customer)
    {
        customerQueue.Remove(customer);
    }
}
