using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayShrinkingHyperRectangle : HyperRectangle
{
    public float xShrink, yShrink, zShrink, qShrink;

    override protected void initShape()
    {
        base.initShape();

        float[] shrinks = new float[] { xShrink, yShrink, zShrink, qShrink };

        Vector4 bounds = generatedShape.calculateBoundingBoxSize();

        for (int i = 0; i < generatedShape.numVertices; i++)
        {
            for (int dim = 0; dim < 4; dim++)
            {//shrinking all other dims by how far in this dim relatively to bounds
                float vertVal = generatedShape.getVertex4At(i)[dim];
                float p = (Mathf.Abs(vertVal) + vertVal);
                float progress = (vertVal / bounds[dim]) + 0.5f;
                for (int otherdim = 0; otherdim < 4; otherdim++)
                {
                    if (otherdim == dim) { continue; }
                    generatedShape.getVertex4At(i)[otherdim] *= 1f - (progress * shrinks[dim]);
                }
            }
        }
    }
}
