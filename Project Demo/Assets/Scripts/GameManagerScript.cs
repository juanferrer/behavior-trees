using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

	public GameObject kitchen;
	public GameObject queue;
    public QueueScript queueScript;
	public GameObject exit;
    public GameObject customerPrefab;

    public List<GameObject> tableList;

	// Use this for initialization
	void Start ()
	{
        queueScript = queue.GetComponent<QueueScript>();
        tableList.AddRange(GameObject.FindGameObjectsWithTag("Table"));
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var newPos = new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));
            GameObject.Instantiate(customerPrefab, newPos, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            queueScript.GetNextCustomer().StartReceiving(tableList[Random.Range(0, tableList.Count)]);
        }
	}
}
