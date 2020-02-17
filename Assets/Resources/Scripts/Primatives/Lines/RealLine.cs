using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RealLine 
{
    protected Vector start, end, diff;

    public Vector this[int index]
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
            }
            if (index == 1)
            {
                end = value;
            }
            throw new Exception("");
        }
    }

    public abstract bool isParallelTo(RealLine realLine, float tolerance = -1);
    public abstract Vector cross(RealLine otherRealLine3);

    public bool doesLineExistAtDimVal(int dim, float val)
    {
        if (diff[dim] == 0)
        {
            return false;
        }

        float t = getIntersectionFac(dim, val);
        return t >= 0 && t <= 1;
    }

    public Vector getPointAtDimVal(int dim, float val)
    {
        float t = getIntersectionFac(dim, val);
        return start + diff * t;
    }

    public float getIntersectionFac(int dim, float val)
    {
        return (val - this[0][dim]) / (this[1][dim] - this[0][dim]);
    }

    public Vector getDiff() {
        return diff;
    }

    public override string ToString()
    {
        return "Line (" + start + ", " + end + ") diff ~ " + getDiff();
    }

    internal bool doesLineContainPoint(Vector vector)
    {
        for (int dim = 0; dim < vector.getNumDims(); dim++)
        {
            if (!doesLineExistAtDimVal(dim, vector[dim])) {
                return false;
            }
        }
        return true;
    }
}
