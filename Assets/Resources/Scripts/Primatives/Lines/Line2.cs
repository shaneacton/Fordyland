using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line2 : RealLine
{
    public Line2(MyVector2 start, MyVector2 end)
    {
        this.start = start;
        this.end = end;

        this.diff = (end - start);
    }

    public MyVector2 getStart() { return (MyVector2)start; }
    public MyVector2 getEnd() { return (MyVector2)end; }
    public MyVector2 getDiff() { return (MyVector2)diff; }

    public MyVector2 this[int index]
    {
        get
        {
            if (index == 0)
            {
                return (MyVector2)start;
            }
            if (index == 1)
            {
                return (MyVector2)end;
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

    public override bool isParallelTo(RealLine otherLine, float tolerance = -1)
    {
        if (otherLine is Line2 other2)
            return getDiff().isParallelTo(other2.getDiff(), tollerance:tolerance);

        throw new Exception();
    }

    public override Vector cross(RealLine otherRealLine3)
    {
        return new MyVector2(1, 1);//cross product not defined in 2d, pretend all normals are equal
    }

    public Line3 addDimWithVal(int val)
    {
        MyVector3 a = new MyVector3(getStart().x, getStart().y, val);
        MyVector3 b = new MyVector3(getEnd().x, getEnd().y, val);
        return new Line3(a, b);
    }
}
