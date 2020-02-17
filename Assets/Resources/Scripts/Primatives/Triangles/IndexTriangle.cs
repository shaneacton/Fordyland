using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexTriangle
{
    public int[] vertexIndices; // 3 vertex indices connected in order 0-1-2-0

    public IndexTriangle(int v1, int v2, int v3)
    {
        this.vertexIndices = new int[] { v1, v2, v3 };
    }

    public IndexTriangle(int[] vertexIndices)
    {
        this.vertexIndices = new int[] { vertexIndices[0], vertexIndices[1], vertexIndices[2] };
    }

    public int this[int index]
    {
        get
        {
            return vertexIndices[index];

        }

        set
        {
            vertexIndices[index] = value;
        }
    }

    public override string ToString()
    {        
        return "Triangle ("+vertexIndices[0] +"," + vertexIndices[1] + "," + vertexIndices[2] + ")";
    }

    public Triangle4 getTriangle4(Shape4D shape) {
        return new Triangle4(shape.getVertex4At(vertexIndices[0]), shape.getVertex4At(vertexIndices[1]), shape.getVertex4At(vertexIndices[2]));
    }

    public Triangle3 getTriangle3(Shape3D shape)
    {
        return new Triangle3(shape.getVertex3At(vertexIndices[0]), shape.getVertex3At(vertexIndices[1]), shape.getVertex3At(vertexIndices[2]));
    }
}
