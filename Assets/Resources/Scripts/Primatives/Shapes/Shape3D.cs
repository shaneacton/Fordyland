using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape3D : Shape
{
    public Shape3D(): base()
    {
    }

    public Shape2D getShapeAtDimVal(int dim, float val) {
        Shape2D shape2 = new Shape2D();
        foreach (IndexTriangle tri in triangles)
        {
            Triangle3 tri3 = tri.getTriangle3(this);
            Line2[] lines = tri3.cutTriAtDimVal(dim, val);
            if (lines == null) { continue; }
            foreach (Line2 line in lines)
            {
                MyVector2 a = line[0];
                MyVector2 b = line[1];
                shape2.addVertex(a);
                shape2.addVertex(b);
                if (a.Equals(b)) {
                    continue;
                }
                shape2.addLine(new IndexLine(shape2.getIndexOf(a), shape2.getIndexOf(b)));
            }
        }

        ShapeSimplifier.removeRedundantLinesFromShape(shape2, debug: false);

        return shape2;
    }

    public Mesh getMesh() {
        Mesh mesh = new Mesh();
        //Debug.Log("num verts: " + numVertices + " for mesh: " + numVertices);
        mesh.vertices = getVertexList();
        mesh.triangles = getMeshTriangles();
        //Debug.Log("num triangles: " + mesh.triangles.Length);

        return mesh;
    }

    public Vector3[] getVertexList()
    {
        Vector3[] verts = new Vector3[numVertices];
        for (int i = 0; i < numVertices; i++)
        {
            verts[i] = getVertex3At(i).vector;
        }
        return verts;
    }

    public int[] getMeshTriangles()
    {
        MyVector3 center = getCenterPoint();
        int[] meshTriangles = new int[triangles.Count * 3];
        int t = 0;
        foreach (IndexTriangle tri in triangles)
        {
            Triangle3 tri3 = tri.getTriangle3(this);
            MyVector3 normal = tri3.getNormal();
            MyVector3 offset = tri3.getCenter() - center;
            
            for (int i = 0; i < 3; i++)
            {
                if (normal.dot(offset) > 0)
                {
                    meshTriangles[t] = tri[i];
                }
                else {
                    meshTriangles[t] = tri[2 - i];
                }
                t++;
            }
        }
        return meshTriangles;
    }

    private MyVector3 getCenterPoint()
    {
        MyVector3 sum = MyVector3.zero;
        for (int i = 0; i < numVertices; i++)
        {
            sum += getVertex3At(i);
        }
        return sum * (1f / numVertices);
    }

    public MyVector3 getVertex3At(int index)
    {
        return (MyVector3)getVertexAt(index);
    }
}

