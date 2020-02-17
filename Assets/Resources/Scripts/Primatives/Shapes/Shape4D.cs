using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape4D : Shape
{
    public Shape4D(): base()
    {
    }

    public Shape3D getShape3DAtDimVal(int dim, float val) {
        Shape3D shape3 = new Shape3D();

        foreach (IndexTriangle tri in triangles)
        {
            //Debug.Log("triangle: " + tri);
            Triangle4 tri4 = tri.getTriangle4(this);
            Line3[] lines = tri4.cutTriAtDimVal(dim,val);
            if (lines == null) { continue; }
            //Debug.Log("non null line found: " + line);
            foreach (Line3 line in lines)
            {
                MyVector3 a = line[0];
                MyVector3 b = line[1];
                shape3.addVertex(a);
                shape3.addVertex(b);
                if (a.Equals(b)) {
                    continue;
                }
                shape3.addLine(new IndexLine(shape3.getIndexOf(a), shape3.getIndexOf(b)));
            }
        }
        return shape3;
    }

    public Shape3D getShapeAtQ(float q)
    {
        /*
         * returns result of cross section at a fixed value in the forth dimension
         */

        return getShape3DAtDimVal(3, q);
    }

    public Shape2D getShapeAtXZ(float x, float z)
    {
        Shape3D shape3 = getShape3DAtDimVal(2, z);//shape is made of triangles with points in the space (x,y,q)
        //Visualiser.visualiseLines(shape3);
        Triangulator.triangulateShape(shape3, debug: false);
        //Visualiser.visualiseTriangles(shape3);
        Shape2D shape2 = shape3.getShapeAtDimVal(0, x);//shape made of tris in the sapce (y,q)
        shape2.swizzle();//shape now in space (q,y)
        //Visualiser.visualiseLines(shape2);
        return shape2;
    }

    internal Shape3D getShape3AtXZ(float x, float z)
    {
        Shape2D shape2 = getShapeAtXZ(x, z);
        //Visualiser.visualiseLines(shape2);
        Shape3D extruded = shape2.getExtrudedShape();
        //Visualiser.visualiseLines(extruded);
        return extruded;
    }

    internal Vector4 getVertex4At(int index)
    {
        return (Vector4)getVertexAt(index);
    }

    public Vector4 calculateBoundingBoxSize() {
        Vector4 mins = new Vector4();
        Vector4 maxs = new Vector4();

        foreach (Vector4 vert in verticesIndexMap.Keys)
        {
            for (int dim = 0; dim < 4; dim++)
            {
                mins[dim] = Mathf.Min(vert[dim], mins[dim]);
                maxs[dim] = Mathf.Max(vert[dim], maxs[dim]);
            }
        }

        return maxs - mins;
    }

}
