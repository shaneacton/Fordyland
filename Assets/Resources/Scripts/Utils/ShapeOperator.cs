using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShapeOperator
{
    internal Dictionary<int, HashSet<LinePoint>> lineIntersections; // maps each vertex to the lines which attach to it
    protected Shape shape;

    protected void findLineIntersections(bool debug = false)
    {
        lineIntersections = new Dictionary<int, HashSet<LinePoint>>();
        HashSet<IndexLine> shapeLines = shape.getLines();
        Dictionary<int, HashSet<LinePoint>> intersections = findIntersectionsFromLines(shape.getLines());
        //Debug.Log("total number of intersection points for " + shape+": " + intersections.Count);
        lineIntersections = widdleLineIntersections(intersections);
        //Debug.Log("number of valid intersection pointsfor " + shape + ": " + lineIntersections.Count);
    }



    public static Dictionary<int, HashSet<LinePoint>> findIntersectionsFromLines(HashSet<IndexLine> lines)
    {
        Dictionary<int, HashSet<LinePoint>> intersections = new Dictionary<int, HashSet<LinePoint>>();
        foreach (IndexLine line in lines)
        {
            for (int i = 0; i < 2; i++)
            {//start and finish of each line
                int vertId = line[i];
                if (!intersections.ContainsKey(vertId))
                {
                    intersections.Add(vertId, new HashSet<LinePoint>());
                    //Debug.Log("found new intersection vert: " + vertId);
                }
                intersections[vertId].Add(new LinePoint(line, i));
                //Debug.Log("adding new line to vert intersect: " + (new LinePoint(line, i)));
            }
        }
        return intersections;
    }

    private Dictionary<int, HashSet<LinePoint>> widdleLineIntersections(Dictionary<int, HashSet<LinePoint>> intersections, int remainingRecurses = 50)
    {
        if (remainingRecurses == 0)
        {
            throw new Exception("ran out of recruses reducing line intersections");
        }
        Dictionary<int, HashSet<LinePoint>> widdledIntersections = new Dictionary<int, HashSet<LinePoint>>();

        bool removedline = false;
        foreach (int intersection in intersections.Keys)
        {
            if (intersections[intersection].Count < 2)
            {
                foreach (LinePoint line in intersections[intersection])
                {
                    //Debug.LogError("dead end line: " + line  + " no intersection for vert: " + intersection + " recurses: " + remainingRecurses);
                    shape.removeLine(line.line);
                    removedline = true;
                    intersections[line.getOtherPoint()].Remove(line.getOppositeLinePoint());
                    if (widdledIntersections.ContainsKey(line.getOtherPoint())){
                        widdledIntersections[line.getOtherPoint()].Remove(line.getOppositeLinePoint());
                    }
                }
            }
            else
            {
                if (!widdledIntersections.ContainsKey(intersection))
                {
                    //throw new Exception();
                    widdledIntersections.Add(intersection, new HashSet<LinePoint>());

                }
                //widdledIntersections.Add(intersection, new HashSet<LinePoint>());
                String mess = "found new valid intersection vert: " + intersection + " with lines: ";
                foreach (LinePoint line in intersections[intersection])
                {
                    if (line.getPoint() != intersection) { throw new Exception(); }

                    widdledIntersections[intersection].Add(line);
                    int outGoingPoint = line.getOtherPoint();
                    if (!widdledIntersections.ContainsKey(outGoingPoint))
                    {
                        widdledIntersections.Add(outGoingPoint, new HashSet<LinePoint>());
                    }
                    widdledIntersections[outGoingPoint].Add(line.getOppositeLinePoint());
                    mess += (line + ", ");
                }
            }
        }
        if (removedline)
        {
            return widdleLineIntersections(widdledIntersections, remainingRecurses - 1);
        }
        else
        {
            return widdledIntersections;
        }
    }
}
