using DG.Tweening;
using DG.Tweening.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using UnityEngine;
using UnityEngine.Events;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] WaypointPath path;
    [SerializeField] float defaultMoveDuration = 1f;
    [SerializeField] int waypointIndex = 0;
    Tween move; Tween rotate;

    [SerializeField] List<WaypointEmitter> waypointsEmitters;
    [Serializable] class WaypointEmitter
    {
        public WaypointEmitter(string name, int triggerWaypoint)
        {
            this.name = name;
            this.triggerWaypoint = triggerWaypoint;
        }

        #pragma warning disable 0414 
        [SerializeField] string name = "Emitter"; //this just names the thing
        #pragma warning restore 0414
        public int triggerWaypoint;
        public UnityEvent events;
    }

    [SerializeField] bool debugTrigger;

    private void OnValidate()
    {
        FillEmptyEmitterSpots();
    }

    private void FixedUpdate()
    {
        if(debugTrigger)
        {
            debugTrigger = false;
            MoveAlongPath();
        }
    }

    private void OnDrawGizmosSelected()
    {
         path?.DrawWaypointGizmos();
    }

    public void Stop()
    {
        transform.DOPause();
    }

    public void SetWayPointIndex(int index)
    {
        waypointIndex = index;
    }

    public void MoveAlongPath(float time = -1)
    {
        if (time <= 0)
            time = defaultMoveDuration;

        waypointIndex = path.GetNextWaypointIndex(waypointIndex);
        //Debug.Log("Moving to: " + waypointIndex);

        move?.Complete(); move = null;
        rotate?.Complete(); rotate = null;

        rotate = transform.DORotateQuaternion(path.WayPoints[waypointIndex].rotation, time);
        move = transform.DOMove(path.WayPoints[waypointIndex].position, time);

        move.onComplete += TriggerSignal;
    }

    public void MoveToWaypoint(int waypoint, float time = -1)
    {
        if (time <= 0)
            time = defaultMoveDuration;

        waypointIndex = Mathf.Clamp(waypoint, 0, path.WayPointCount - 1);

        move?.Complete(); move = null;
        rotate?.Complete(); rotate = null;

        rotate = transform.DORotateQuaternion(path.WayPoints[waypointIndex].rotation, time);
        move = transform.DOMove(path.WayPoints[waypointIndex].position, time);

        move.onComplete += TriggerSignal;
    }
    void TriggerSignal()
    {
        int checkIndex = waypointIndex;

        foreach (WaypointEmitter emitter in waypointsEmitters)
        {
            if (emitter.triggerWaypoint == checkIndex)
            {
                //Debug.Log("Arrived at : " + waypointIndex);
                emitter.events.Invoke();
            }
        }
    }

    void TriggerSignalDelayed()
    {
        int checkIndex = waypointIndex;

        foreach (WaypointEmitter emitter in waypointsEmitters)
        {
            if (emitter.triggerWaypoint == checkIndex)
            {
                //Debug.Log("Arrived at : " + waypointIndex);
                emitter.events.Invoke();
            }
        }
    }

    void FillEmptyEmitterSpots()
    {
        if (path == null)
            return;

        for (int i = 0; i < path.WayPointCount; i++)
        {
            bool exists = false;
            foreach (WaypointEmitter emitter in waypointsEmitters)
            {
                if (emitter.triggerWaypoint == i)
                {
                    exists = true;
                    break;
                }
            }

            if (exists)
                continue;

            WaypointEmitter missingWP = new WaypointEmitter(path.WayPoints[i].name, i);
            waypointsEmitters.Add(missingWP);
        }
    }
}
