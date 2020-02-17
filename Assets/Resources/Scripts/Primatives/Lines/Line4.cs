using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line4 : RealLine
{

    public Line4(Vector4 origin, Vector4 end)
    {
        this.start = origin;
        this.end = end;
        this.diff = (end- origin);
    }

    public Vector4 getStart() { return (Vector4)start; }
    public Vector4 getEnd() { return (Vector4)end; }
    public Vector4 getDiff() { return (Vector4)diff; }

    public bool doesLineExistAtQ(float q) {
        return doesLineExistAtDimVal(3, q);     
    }

    public MyVector3 getPointAtQ(float q)
    {
        Vector4 intersectionPoint = getPoint4AtDimVal(3, q);
        return intersectionPoint.dropQ();
    }

    public Vector4 getPoint4AtDimVal(int dim, float val) {
        return (Vector4)getPointAtDimVal(dim, val);
    }

    public override bool isParallelTo(RealLine realLine, float tolerance = -1)
    {
        throw new NotImplementedException();
    }

    public override Vector cross(RealLine otherRealLine3)
    {
        throw new NotImplementedException();
    }
}
