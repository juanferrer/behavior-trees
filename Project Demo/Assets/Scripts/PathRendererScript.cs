using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class PathRendererScript : MonoBehaviour {

    private LineRenderer line;
    private Transform targetPos;
    private NavMeshAgent agent;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        agent = GetComponent<NavMeshAgent>(); 
    }

    private void Update()
    {
        StartCoroutine(GetPath());
    }

    IEnumerator GetPath()
    {
        line.SetPosition(0, transform.position);

        // Draw after everything else has been calculated
        yield return new WaitForEndOfFrame();

        DrawPath(agent.path);

        yield return null;
    }

    void DrawPath(NavMeshPath path)
    {
        // If less than two corners, there is no path
        if (path.corners.Length < 2)
            return;

        line.positionCount = path.corners.Length;

        // Add each corner to the line renderer
        for (var i = 1; i < path.corners.Length; ++i)
        {
            line.SetPosition(i, path.corners[i]);
        }
    }
}
