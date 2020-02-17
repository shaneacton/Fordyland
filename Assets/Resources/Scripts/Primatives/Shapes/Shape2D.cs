using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shape2D : Shape
{
    static MyVector3 extrusion = new MyVector3(0, 0, 0.5f);

    public Shape2D() : base()
    {
    }

    public Mesh getMesh()
    {
        //Shape3D extrudedShape = getExtrudedShape();
        //Debug.Log("before triangulation: extruded shape has " + extrudedShape.numVertices + " verts and " + extrudedShape.getLines().Count + " lines");

        //Triangulator.triangulateShape(extrudedShape);
        //Debug.Log("after triangulation: extruded shape has " + extrudedShape.numVertices + " verts and " + extrudedShape.getLines().Count + " lines");
        //return extrudedShape.getMesh();
        throw new Exception();
    }

    public Shape3D getExtrudedShape() {
        Shape3D shape3 = new Shape3D();
        //Debug.Log("extruding shape2 with " + numVertices + " verts and " + getLines().Count + " lines");
        foreach (IndexLine line in getLines())
        {
            Line2 line2 = line.getLine2(this);
            MyVector3[] verts = new MyVector3[2];
            MyVector3[] extrudeVerts = new MyVector3[2];

            for (int i = 0; i < 2; i++)
            {//start and end of line
                verts[i] = line2[i].addDimWithVal(0);
                extrudeVerts[i] = verts[i] + extrusion;
                shape3.addVertex(verts[i]);
                shape3.addVertex(extrudeVerts[i]);
                IndexLine extrudeLine = new IndexLine(shape3.getIndexOf(verts[i]), shape3.getIndexOf(extrudeVerts[i]));
                shape3.addLine(extrudeLine);
            }
            shape3.addLine(new IndexLine(shape3.getIndexOf(verts[0]), shape3.getIndexOf(verts[1])));
            shape3.addLine(new IndexLine(shape3.getIndexOf(extrudeVerts[0]), shape3.getIndexOf(extrudeVerts[1])));

        }

        return shape3;
    }

    internal void swizzle()
    {
        //swaps the x,y dims of each vert
        int[] vertIDs = indicesVertexMap.Keys.ToArray();
        foreach (int vertId in vertIDs)
        {
            MyVector2 oldVert = (MyVector2)indicesVertexMap[vertId];
            MyVector2 swizzledVert = oldVert.swizzle();
            indicesVertexMap[vertId] = swizzledVert;
            verticesIndexMap.Remove(oldVert);
            if (verticesIndexMap.ContainsKey(swizzledVert)) continue;
            verticesIndexMap.Add(swizzledVert, vertId);
        }
    }

    private MyVector3 getCenterPoint()
    {
        MyVector2 sum = MyVector2.zero;
        for (int i = 0; i < numVertices; i++)
        {
            sum += getVertex2At(i);
        }
        MyVector3 average =  (sum * (1f / numVertices)).addDimWithVal(0);
        return average + extrusion * 0.5f;
    }

    public MyVector2 getVertex2At(int index)
    {
        return (MyVector2)getVertexAt(index);
    }
}
