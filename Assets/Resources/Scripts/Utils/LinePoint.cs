using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LinePoint
{
    public IndexLine line;
    public int index;

    public LinePoint(IndexLine line, int index)
    {
        this.line = line;
        this.index = index;
    }

    public int this[int index]
    {
        get
        {
            if (index == 0)
            {
                return getPoint();
            }
            if (index == 1)
            {
                return getOtherPoint();
            }
            throw new Exception("");
        }
    }

    public int getPoint()
    {
        return line[index];
    }

    public int getOtherPoint()
    {
        return line[1 - index];
    }

    public override string ToString()
    {
        return "Linepoint (line: " + line + " id:" + index + ")";
    }

    public override bool Equals(object obj)
    {
        return obj is LinePoint otherLine &&
               (
                (line.start == otherLine.line.start && line.end == otherLine.line.end && index == otherLine.index) ||
                (line.start == otherLine.line.end && line.end == otherLine.line.start && index == 1 - otherLine.index)
               );
    }

    public override int GetHashCode()
    {
        var hashCode = 1474027755;
        hashCode = hashCode * -1521134295 + line.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(getPoint() < getOtherPoint() ? 0 : 1);
        return hashCode;
    }

    public int getNumIntersectionsAtPoint(Dictionary<int, HashSet<LinePoint>> lineIntersections) {
        return getNumIntersectionsAtIndex(lineIntersections, 0);
    }
    public int getNumIntersectionsAtOtherPoint(Dictionary<int, HashSet<LinePoint>> lineIntersections)
    {
        return getNumIntersectionsAtIndex(lineIntersections, 1);
    }

    private int getNumIntersectionsAtIndex(Dictionary<int, HashSet<LinePoint>> lineIntersections, int index) {
        int vertId = this[index];
        if (!lineIntersections[vertId].Contains(this)) { throw new Exception("line intersections does not contain this linepoint " + this + " cannot find num neighbours"); }
        return lineIntersections[vertId].Count;
    }

    internal LinePoint getOppositeLinePoint()
    {
        return new LinePoint(line, 1 - index);
    }
}
