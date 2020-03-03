using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape4DGenerator : MonoBehaviour
{

    public float x;
    public float y;
    public float z;
    public float q;

    protected Shape4D generatedShape;
    public Dictionary<Vector4, int> indexMap;
    int nextIndex;

    public Shape4D getShape()
    {
        generatedShape = new Shape4D();
        initShape();
        return generatedShape;
    }

    public Vector4 getPosition()
    {
        return new Vector4(x, y, z, q);
    }


    protected virtual void initShape() {
        indexMap = new Dictionary<Vector4, int>();
        nextIndex = 0;
    }

    public void addVert(Vector4 vert) {
        if (!indexMap.ContainsKey(vert)) {
            //Debug.Log("registering vert " + vert + " with " + nextIndex);
            indexMap.Add(vert, nextIndex);
            nextIndex++;
        }
    }

    protected void commitVerts() {
        foreach (Vector4 vert in indexMap.Keys)
        {
            generatedShape.addVertex(vert);
        }
    }

    public virtual float getMass() {
        throw new Exception();
    }

    internal void addTriangles(IEnumerable<IndexTriangle> triangles)
    {
        foreach (IndexTriangle tri in triangles)
        {
            generatedShape.triangles.Add(tri);
        }
    }

    internal void addTriangle(IndexTriangle indexTriangle)
    {
        generatedShape.triangles.Add(indexTriangle);
    }
}
