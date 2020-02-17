using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexLine 
{
    public int start, end;

    public IndexLine(int start, int end) {
        this.start = start;
        this.end = end;
        if (start == end) { throw new Exception("line cannot connect point to itself"); }
    }

    public Line2 getLine2(Shape2D shape)
    {
        return new Line2(shape.getVertex2At(start), shape.getVertex2At(end));
    }

    public Line3 getLine3(Shape3D shape) {
        return new Line3(shape.getVertex3At(start), shape.getVertex3At(end));
    }

    public Line4 getLine4(Shape4D shape) {
        return new Line4(shape.getVertex4At(start), shape.getVertex4At(end));
    }

    public RealLine getRealLine(Shape shape) {
        if(shape is Shape3D shape3)
        {
            return getLine3(shape3);
        }
        if (shape is Shape4D shape4)
        {
            return getLine4(shape4);
        }
        if (shape is Shape2D shape2)
        {
            return getLine2(shape2);
        }
        throw new Exception();
    }

    public int this[int index]
    {
        get
        {
            if (index == 0)
            {
                return start;
            }
            if (index == 1)
            {
                return end;
            }
            throw new Exception("");

        }

        set
        {
            if (index == 0)
            {
                start = value;
                return;
            }
            if (index == 1)
            {
                end = value;
                return;
            }
            throw new Exception("");
        }
    }

    public override string ToString()
    {
        return "Line ("+start + "," + end+")";
    }

    public override bool Equals(object obj)
    {
        return obj is IndexLine line &&
               (
                (start == line.start && end == line.end) ||
                (start == line.end && end == line.start)
               );
    }

    public override int GetHashCode()
    {
        var hashCode = 1474027755;
        int lowerId = Mathf.Min(this[0], this[1]);
        int upperId = Mathf.Max(this[0], this[1]);
        hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(lowerId);
        hashCode = hashCode * -1521134295 + EqualityComparer<int>.Default.GetHashCode(upperId);
        return hashCode;
    }

}
