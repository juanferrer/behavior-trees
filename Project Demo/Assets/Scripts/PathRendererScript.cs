using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class PathRendererScript : MonoBehaviour {

    private LineRenderer line; //to hold the line Renderer
    private Transform target; //to hold the transform of the target
    private NavMeshAgent agent; //to hold the agent of this gameObject

    void Start()
    {
        line = GetComponent<LineRenderer>(); // Get the line renderer
        agent = GetComponent<NavMeshAgent>(); // Get the agent  
    }

    private void Update()
    {
        StartCoroutine(GetPath());
    }

    IEnumerator GetPath()
    {
        line.SetPosition(0, transform.position); // Set the line's origin

        yield return new WaitForEndOfFrame();

        DrawPath(agent.path);

        //agent.isStopped = true; // Add this if you don't want to move the agent

        yield return null;
    }

    void DrawPath(NavMeshPath path)
    {
        if (path.corners.Length < 2) // If the path has 1 or no corners, there is no need
            return;

        line.positionCount = path.corners.Length; // Set the array of positions to the amount of corners

        for (var i = 1; i < path.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]); // Go through each corner and set that to the line renderer's position
        }
    }
}
