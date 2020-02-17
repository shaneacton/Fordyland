using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line3 : RealLine
{

    public Line3(MyVector3 start, MyVector3 end)
    {
        this.start = start;
        this.end = end;

        this.diff = (end - start);
    }
    public MyVector3 this[int index]
    {
        get
        {
            if (index == 0) {
                return (MyVector3)start;
            }
            if (index == 1)
            {
                return (MyVector3)end;
            }
            throw new Exception("");

        }

        set
        {
            if (index == 0)
            {
                start = value;
            }
            if (index == 1)
            {
                end = value;
            }
            throw new Exception("");
        }
    }

    public MyVector3 getStart() { return (MyVector3)start; }
    public MyVector3 getEnd() { return (MyVector3)end; }
    public MyVector3 getDiff() { return (MyVector3)diff; }

    public override Vector cross(RealLine otherLine)
    {
        if (otherLine is Line3 other3)
            return new MyVector3(Vector3.Cross(getDiff().vector, other3.getDiff().vector));


        throw new Exception();
    }

    internal int getClosestIndex(MyVector3 region)
    {
        int closestIndex = -1;
        float closestDist = float.PositiveInfinity;

        for (int i = 0; i < 2; i++)
        {
            float dist = region.distance(this[i]);
            if (closestDist < dist) {
                closestDist = dist;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    public static Line3 operator +(Line3 a, MyVector3 b)
    {
        return new Line3(a.getStart() + b , a.getEnd() + b);
    }

    public override bool Equals(object obj)
    {
        return obj is Line3 line && (
            (start == line.start &&
               end == line.end) ||
               (end == line.start &&
               start == line.end));
    }

    public override int GetHashCode()
    {
        var hashCode = 1474027755;
        int lowerCode = Mathf.Min(this[0].GetHashCode(), this[1].GetHashCode());
        int upperCode = Mathf.Max(this[0].GetHashCode(), this[1].GetHashCode());

        hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(lowerCode);
        hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(upperCode);

        return hashCode;
    }

    public override bool isParallelTo(RealLine otherLine, float tolerance = -1)
    {
        if(otherLine is Line3 other3)
        return getDiff().isParallelTo(other3.getDiff(), tollerance:tolerance);

        throw new Exception();
    }

    internal Line3 getPerpendicularThrough(MyVector3 passingThrough, MyVector3 otherPerp, float mag = 1f)
    {
        MyVector3 perp = new MyVector3(Vector3.Cross(getDiff().vector, otherPerp.vector).normalized * mag);
        return new Line3(passingThrough - perp * 0.5f, passingThrough + perp * 0.5f);
    }

    public MyVector3 getPoint3AtDimVal(int dim, float val)
    {
        return (MyVector3)getPointAtDimVal(dim, val);
    }
}
