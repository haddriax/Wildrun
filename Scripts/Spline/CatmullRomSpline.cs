﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomSpline : MonoBehaviour
{
    //Has to be at least 4 points
    public Transform[] controlPointsList;

    //Are we making a line or a loop?
    [SerializeField]
    private bool isLooping = true;
   
    //Max loops
    private int loops;

    //The spline's resolution
    [SerializeField]
    [Range(0.01f, 0.2f)]
    [Tooltip("Lower resolution is, better the spline is")]
    private float resolution = 0.01f;

    //Path points for IA
    public List<Vector3> pathPoints;

    [SerializeField]
    [Tooltip("Use it to init debug lines")]
    private bool initDebugLine = false;

    [SerializeField]
    [Tooltip("Use it to draw debug lines")]
    private bool drawDebugLine = false;

    //Line Renderer to debug
    private LineRenderer lineRenderer;

    private void Awake()
    {
        loops = 0;
        pathPoints = new List<Vector3>();

        if (initDebugLine)
            lineRenderer = this.gameObject.GetComponent<LineRenderer>();

        for (int i = 0; i < controlPointsList.Length; i++)
        {
            if ((i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1) && !isLooping)
            {
                continue;
            }
            DisplayCatmullRomSpline(i);
        }

        DrawDebugLineInGame();
    }

    private void Update()
    {
        if (initDebugLine && drawDebugLine)
            lineRenderer.enabled = true;
        else if (initDebugLine && !drawDebugLine)
            lineRenderer.enabled = false;
    }

    //Display without having to press play
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;

    //    //Draw the Catmull-Rom spline between the points
    //    for (int i = 0; i < controlPointsList.Length; i++)
    //    {
    //        //Cant draw between the endpoints
    //        //Neither do we need to draw from the second to the last endpoint
    //        //...if we are not making a looping line
    //        if ((i == 0 || i == controlPointsList.Length - 2 || i == controlPointsList.Length - 1) && !isLooping)
    //        {
    //            continue;
    //        }

    //        DisplayCatmullRomSpline(i);
    //    }
    //}

    private void DrawDebugLineInGame()
    {
        if (initDebugLine)
        {
            lineRenderer.positionCount = pathPoints.Count;
            
            Vector3[] temp = new Vector3[pathPoints.Count];
            pathPoints.CopyTo(temp);
            lineRenderer.SetPositions(temp);
        }
    }

    //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
    private void DisplayCatmullRomSpline(int pos)
    {
        //The 4 points we need to form a spline between p1 and p2
        Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].position;
        Vector3 p1 = controlPointsList[pos].position;
        Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].position;
        Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].position;

        //The start position of the line
        Vector3 lastPos = p1;

        //How many times should we loop?
        loops = Mathf.FloorToInt(1f / resolution);

        for (int i = 1; i <= loops; i++)
        {
            //Which t position are we at?
            float t = i * resolution;

            //Find the coordinate between the end points with a Catmull-Rom spline
            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

            //Get path points via new position
            pathPoints.Add(newPos);

            //Draw this line segment
            //Gizmos.DrawLine(lastPos, newPos);

            //Save this pos so we can draw the next line segment
            lastPos = newPos;
        }
    }

    //Clamp the list positions to allow looping
    private int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = controlPointsList.Length - 1;
        }

        if (pos > controlPointsList.Length)
        {
            pos = 1;
        }
        else if (pos > controlPointsList.Length - 1)
        {
            pos = 0;
        }

        return pos;
    }

    //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
    private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }
}
