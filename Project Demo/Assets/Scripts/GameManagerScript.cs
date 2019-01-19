using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

	public GameObject kitchen;
	public GameObject queue;
    [HideInInspector]
    public QueueScript queueScript;
	public GameObject exit;
    public GameObject customerPrefab;
    [HideInInspector]
    public List<GameObject> tableList;
    private new GameObject camera;

    private CustomerScript customer;

    public float LeftMostCamPos;
    public float RightMostCamPos;
    public float CamSpeed;
    public Blackboard blackboard;

	// Use this for initialization
	void Start ()
	{
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        queueScript = queue.GetComponent<QueueScript>();
        tableList.AddRange(GameObject.FindGameObjectsWithTag("Table"));
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        blackboard = new Blackboard();
    }
	
	// Update is called once per frame
	void Update ()
	{
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (camera.transform.position.x < LeftMostCamPos)
            {
                camera.transform.Translate(-CamSpeed * Time.deltaTime, 0.0f, 0.0f);
            }
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (camera.transform.position.x > RightMostCamPos)
            {
                camera.transform.Translate(CamSpeed * Time.deltaTime, 0.0f, 0.0f);
            }
        }

        // DEBUG
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var newPos = new Vector3(Random.Range(0.0f, -30.0f), 0.0f, Random.Range(-10.0f, 10.0f));
            GameObject.Instantiate(customerPrefab, newPos, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Receiving customer");
            customer = queueScript.GetNextCustomer();
            queueScript.GetNextCustomer().Receive(tableList[Random.Range(0, tableList.Count)]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Attending customer");
            customer.Attend();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Serving customer");
            customer.Serve();
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Bringing bill to customer");
            customer.BringBill();
        }
    }
}
