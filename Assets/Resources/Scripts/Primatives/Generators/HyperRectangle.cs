using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperRectangle : HyperCube
{
    public float xWid, yWid, zWid, qWid;

    override protected void initShape() {
        base.h = 1;
        base.initShape();

        float[] dims = new float[] { xWid, yWid, zWid, qWid };
        for (int i = 0; i < generatedShape.numVertices; i++)
        {
            for (int dim = 0; dim < 4; dim++)
            {
                generatedShape.getVertex4At(i)[dim] *= dims[dim];
            }
        }
    }

    public override float getMass()
    {
        return Mathf.Pow(xWid * yWid * zWid * qWid, 1f / 4f);
    }
}
