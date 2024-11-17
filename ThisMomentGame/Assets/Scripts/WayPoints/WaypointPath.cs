using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public List<Transform> WayPoints { get { return GetWaypoints(); } }
    List<Transform> wayPoints = new List<Transform>();
    
    public int WayPointCount { get { return waypointCount; } }
    [SerializeField] int waypointCount;
    
    [SerializeField] bool useWorldSpace = false;


    private void OnValidate()
    {
        wayPoints.Clear();
        waypointCount = 0;

        foreach (Transform t in transform)
        {
            wayPoints.Add(t);
            waypointCount++;
        }
    }
    private void OnDrawGizmos()
    {
        if (IsWaypointSelected())
            DrawWaypointGizmos();
    }

    private void Start()
    {
        if (useWorldSpace)
            transform.parent = null;
    }

    public List<Transform> GetWaypoints()
    {
        return wayPoints;
        //return new List<Transform>(wayPoints);
    }
    public List<Vector3> GetWaypointPositions()
    {
        List<Vector3> points = new List<Vector3>();

        foreach (Transform t in wayPoints)
            points.Add(t.position);

        return points;
    }

    public int GetNextWaypointIndex(int currentWayPointIndex)
    {
        int nextWayPointIndex = currentWayPointIndex + 1;

        if(nextWayPointIndex >= transform.childCount)
        {
            nextWayPointIndex = 0;
        }

        return nextWayPointIndex;
    }

    public int GetIndexOfClosest(Vector3 position)
    {
        List<float> distances = new List<float>();

        foreach(Transform t in wayPoints)
            distances.Add(Vector3.Distance(position, t.position));

        return distances.IndexOf(distances.Min());
    }

    public void DrawWaypointGizmos()
    {
        //call draw on lines
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(GetNextWaypointIndex(i)).position);
        }

        //call draw on spheres
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<WayPoint>().DrawWaypointGizmos();
        }
    }

    bool IsWaypointSelected()
    {
        if (Selection.transforms.Contains(transform))
            return true;

        foreach(Transform child in transform)
        {
            if (Selection.transforms.Contains(child))
                return true;
        }

        return false;
    }
}
