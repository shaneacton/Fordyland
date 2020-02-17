using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle3
{
    MyVector3[] points;

    public Triangle3(MyVector3[] points)
    {
        this.points = new MyVector3[] { points[0], points[1], points[2] }; ;
    }

    public Triangle3(MyVector3 a, MyVector3 b, MyVector3 c)
    {
        this.points = new MyVector3[] { a, b, c };
    }

    internal MyVector3 getNormal()
    {
        Vector3 a = (points[1] - points[0]).vector;
        Vector3 b = (points[2] - points[1]).vector;

        return new MyVector3(Vector3.Cross(a, b));
    }

    internal MyVector3 getCenter()
    {
        return (points[0] + points[1] + points[2])*(1f/3f);
    }

    internal Line2[] cutTriAtDimVal(int dim, float val)
    {
        List<MyVector2> points2 = new List<MyVector2>();
        for (int i = 0; i <= 2; i++)
        {
            int nextId = (i + 1) % 3;
            Line3 triangleLine = new Line3(points[i], points[nextId]);
            if (triangleLine.doesLineExistAtDimVal(dim, val))
            {
                MyVector2 intersectionPoint = triangleLine.getPoint3AtDimVal(dim, val).dropDim(dim);
                points2.Add(intersectionPoint);
            }
        }
        if (points2.Count == 2)
        {
            return new Line2[] { new Line2(points2[0], points2[1]) };
        }
        if (points2.Count == 3) {
            return new Line2[] {
                new Line2(points[0].dropDim(dim), points[1].dropDim(dim)),
                new Line2(points[1].dropDim(dim), points[2].dropDim(dim)),
                new Line2(points[2].dropDim(dim), points[0].dropDim(dim))
            };
        }
        if (points2.Count == 0)
        {
            return null;
        }
        throw new Exception("strange num of points at intersection" + points2.Count);
    }
}
