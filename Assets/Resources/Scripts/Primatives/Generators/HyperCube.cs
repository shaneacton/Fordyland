using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperCube : Shape4DGenerator
{
    public float h;

    HashSet<Vector4> visitedVerts;

    override protected void initShape()
    {
        base.initShape();
        visitedVerts = new HashSet<Vector4>();
        addVert(new Vector4(0, 0, 0, 0));
        propagate(new Vector4(0, 0, 0, 0));
        commitVerts();
    }

    void propagate(Vector4 origin) {
        for (int dim = 0; dim < 4; dim++)
        {
            if (origin[dim] == 0) {
                Vector4 newOrigin = origin * 1;
                newOrigin[dim] = h;
                addVert(newOrigin);

                for (int dim2 = 0; dim2 < 4; dim2++)//draw triangle
                {
                    if (dim != dim2)
                    { // move along each other axis from origin and draw triangle
                        Vector4 thirdPoint = origin * 1;
                        thirdPoint[dim2] = h - thirdPoint[dim2];
                        addVert(thirdPoint);

                        Triangle4 tri = new Triangle4(origin, newOrigin, thirdPoint);
                        generatedShape.triangles.Add(tri.getTriangle(indexMap));
                    }
                }

                if (!visitedVerts.Contains(newOrigin)) {
                    //new point
                    visitedVerts.Add(newOrigin);
                    propagate(newOrigin);
                }
            }
        }

        foreach (Vector4 vert in generatedShape.verticesIndexMap.Keys) {
            for (int i = 0; i < 4; i++)
            {
                vert[i] -= h / 2.0f;
            }
        }
    }

    public override float getMass()
    {
        return h;
    }

}
