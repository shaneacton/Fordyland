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

    public bool doesVectorIntersectLine(Vector origin, Vector A) {
        if (A.isParallelTo(getDiff())) {
            //lines will never meet. 
            bool originOfVecOnLine = doesLineContainPoint(origin);
            if (originOfVecOnLine) {
                Debug.LogWarning("intersecting vec ("+A+") and real line ("+getDiff()+"), but lines parallel and origin of vec ("+origin+") on line");
            }
            return originOfVecOnLine;
        }

        float beta = getVectorIntersectionBeta(origin, A);
        //if beta < 0 then the intersection is behind start
        //if beta > 0 then the intersection is ahead of the end
        return beta >= 0 && beta <= 1;
    }

    internal Vector getCenterPoint()
    {
        return (start.plus(end)).times(0.5f);
    }

    float getVectorIntersectionBeta(Vector origin, Vector A) {
        /*
         * used to find the intersection between this realLine and the vec A originating at origin
         * beta is the coefficient for this.diff at the intersection
         */
        Vector B = getDiff();
        Vector C = this.start - origin;
        Vector D = A.times(B[0]).minus(B.times(A[0]));
        Vector E = C.times(A[0]) - A.times(B[0] - A[0]);
        float beta = E[1] / D[1];
        return beta;
    }
}
