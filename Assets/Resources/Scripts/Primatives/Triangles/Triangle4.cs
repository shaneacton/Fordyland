using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle4 
{
    public Vector4[] points;

    public Triangle4(Vector4[] points)
    {
        this.points = new Vector4[] { points[0], points[1], points[2]};
    }

    public Triangle4(Vector4 a, Vector4 b, Vector4 c)
    {
        this.points = new Vector4[] {a, b, c};
    }

    public override bool Equals(object obj)
    {
        return obj is Triangle4 triangle &&
               obj.GetHashCode() == this.GetHashCode();
    }

    internal Line3[] cutTriAtDimVal(int dim, float val)
    {
        List<MyVector3> points3 = new List<MyVector3>();
        for (int i = 0; i <= 2; i++)
        {
            int nextId = (i + 1) % 3;
            Line4 triangleline = new Line4(points[i], points[nextId]);
            if (triangleline.doesLineExistAtDimVal(dim,val))
            {
                MyVector3 intersectionPoint = triangleline.getPoint4AtDimVal(dim, val).dropDim(dim);
                points3.Add(intersectionPoint);
            }
        }
        if (points3.Count == 2)
        {
            //Debug.Log("found line at triangle intersection");
            return new Line3[] { new Line3(points3[0], points3[1]) };
        }
        if (points3.Count == 3) {
            //the triangle and cut plane are coplanar - return the whole triangle
            return new Line3[] {
                new Line3(points[0].dropDim(dim), points[1].dropDim(dim)),
                new Line3(points[1].dropDim(dim), points[2].dropDim(dim)),
                new Line3(points[2].dropDim(dim), points[0].dropDim(dim))
            };
        }
        if (points3.Count == 0)
        {
            return null;
        }


        throw new Exception("strange num of points at intersection: " + points3.Count);
    }

    internal Line3[] cutTriAtQ(float q)
    {
        return cutTriAtDimVal(3, q);
    }

    public override int GetHashCode()
    {
        var hashCode = 1474027755;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(points[0]);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(points[1]);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(points[2]);
        return hashCode;
    }

    internal IndexTriangle getTriangle(Dictionary<Vector4, int> indexMap)
    {
        //Debug.Log(indexMap.ContainsKey(points[0]) + "," + indexMap.ContainsKey(points[1]) + "," + indexMap.ContainsKey(points[2]));
        return new IndexTriangle(indexMap[points[0]], indexMap[points[1]], indexMap[points[2]]);
    }

    public override string ToString()
    {
        return "Triangle(" + points[0] + "," + points[1] + "," + points[2] + ")";
    }
}
