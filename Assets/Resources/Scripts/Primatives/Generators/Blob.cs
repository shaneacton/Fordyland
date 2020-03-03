using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob
{
    /*
     * draws rings around an axis. the rings change width according to a function, and the rings are linked to form a valid 3d mesh
     */
    int holdoutDim, verticalDim, numRings;
    float height;
    Func<float, float> ringWidthFunction; 
    Ring[] rings;
    Shape4DGenerator generator;
    Vector4 origin;

    public Blob(Shape4DGenerator generator, Vector4 origin , int holdoutDim, int verticalDim, float height, int numRings, Func<float, float> ringWidthFunction)
    {
        this.generator = generator;
        this.origin = origin;
        this.holdoutDim = holdoutDim;
        this.verticalDim = verticalDim;
        this.height = height;
        this.numRings = numRings;
        this.rings = new Ring[numRings];
        this.ringWidthFunction = ringWidthFunction;

        createRings();
        linkRings();
    }

    internal void connectTo(Blob blob2)
    {
        //draws triangles between this and blob2
        for (int i = 0; i < numRings; i++)
        {
            int nextRingId = (i + 1) % numRings;
            Ring l1 = rings[i];
            Ring l2 = rings[nextRingId];
            Ring r1 = blob2.rings[i];
            Ring r2 = blob2.rings[nextRingId];
            connectRings(l1, l2, r1);
            connectRings(r1, r2, l2);
        }
        blob2.rings[0].connectTo(rings[0]);
        blob2.rings[numRings - 1].connectTo(rings[numRings - 1]);
    }

    private void connectRings(Ring l1, Ring l2, Ring r1)
    {
        for (int i = 0; i < Ring.numVertsOnRing; i++)
        {
            int l1Vert, l2Vert, r1Vert;
            l1Vert = generator.indexMap[l1.verts[i]];
            l2Vert = generator.indexMap[l2.verts[i]];
            r1Vert = generator.indexMap[r1.verts[i]];
            generator.addTriangle(new IndexTriangle(l1Vert,l2Vert,r1Vert));
        }
    }

    private void linkRings()
    {
        //link the top and bottom rings like faces. and then link the rings with eachother
        addRingVerts();
        linkTopAndBottomRingFaces();
        linkRingsSides();
    }

    private void linkRingsSides()
    {
        for (int i = 0; i < numRings - 1; i++)
        {
            Ring bottomRing = rings[i];
            Ring topRing = rings[i + 1];
            for (int vertNo = 0; vertNo < Ring.numVertsOnRing; vertNo++)
            {
                int nextVertNo = (vertNo + 1) % Ring.numVertsOnRing;
                int b1, b2, t1, t2;
                b1 = generator.indexMap[bottomRing.verts[vertNo]];
                b2 = generator.indexMap[bottomRing.verts[nextVertNo]];
                t1 = generator.indexMap[topRing.verts[vertNo]];
                t2 = generator.indexMap[topRing.verts[nextVertNo]];
                IndexTriangle[] linkingTriangles = new IndexTriangle[] { new IndexTriangle(b1, b2, t1), new IndexTriangle(t1, t2, b2) };
                generator.addTriangles(linkingTriangles);
            }
        }
    }

    private void addRingVerts()
    {
        foreach (Ring ring in rings)
        {
            foreach (Vector4 vert in ring.verts)
            {
                generator.addVert(vert);
            }
        }
    }

    private void linkTopAndBottomRingFaces()
    {
        linkRingAsface(rings[0]);
        linkRingAsface(rings[rings.Length - 1]);
    }

    private void linkRingAsface(Ring ring)
    {
        List<int> orderedVerts = new List<int>();
        for (int i = 0; i < ring.verts.Length; i++)
        {
            Vector4 vert = ring.verts[i];
            int vertID = generator.indexMap[vert];
            orderedVerts.Add(vertID);
        }
        Walk walk = new Walk(orderedVerts);
        HashSet<IndexTriangle> triangles = walk.getTriangles();
        generator.addTriangles(triangles);
    }

    private void createRings()
    {
        for (int ringNo = 0; ringNo < numRings; ringNo++)
        {
            float heightProgress = (float)ringNo / numRings;
            float ringHeight = heightProgress * height;
            float ringRadius = ringWidthFunction(heightProgress);
            Ring ring = new Ring(generator,origin, holdoutDim, verticalDim, ringHeight, ringRadius);
            rings[ringNo] = ring;
        }
    }
}

public class Ring {
    public static int numVertsOnRing = 12;

    int holdoutDim, verticalDim;
    float radius, height;
    public Vector4[] verts;
    Vector4 origin;
    Shape4DGenerator generator;

    public Ring(Shape4DGenerator generator, Vector4 origin, int holdoutDim, int verticalDim, float height, float radius)
    {
        this.generator = generator;
        this.origin = origin;
        this.holdoutDim = holdoutDim;
        this.verticalDim = verticalDim;
        this.height = height;
        this.radius = radius;
        this.verts = new Vector4[numVertsOnRing];
        createVerts();
        //Debug.Log("creating ring at h= " + height);
    }

    private void createVerts()
    {
        int[] radialDims = getRadialDims();
        for (int i = 0; i < numVertsOnRing; i++)
        {
            float angleUnit = 2f * Mathf.PI / numVertsOnRing;
            float angleOffset = (i % 2) * (angleUnit / 2f);//every second ring is offset from the last by half an angle unit
            float angle = i * angleUnit + angleOffset;
            float dim1Val = Mathf.Cos(angle) * radius;
            float dim2Val = Mathf.Sin(angle) * radius;
            Vector4 vert = getVertFromRadialDimValues(dim1Val, dim2Val) + origin;
            verts[i] = vert;
        }
    }

    private Vector4 getVertFromRadialDimValues(float dim1Val, float dim2Val)
    {
        float[] dimVals = new float[] { dim1Val, dim2Val };
        Vector4 vert = new Vector4();
        int i = 0;
        for (int dim = 0; dim < 4; dim++)
        {
            if (dim == holdoutDim) { vert[dim] = 0;  continue;  }
            if (dim == verticalDim) { vert[dim] = height; continue; }
            vert[dim] = dimVals[i];
            i++;
        }
        return vert;
    }

    private int[] getRadialDims()
    {
        int[] radialDims = new int[2];
        int i = 0;
        for (int dim = 0; dim < 4; dim++)
        {
            if (dim == holdoutDim) { continue; }
            if (dim == verticalDim) { continue; }
            //Debug.Log("dim: " + dim + " i: " + i + " hol: " + holdoutDim + " vert: " + verticalDim);
            radialDims[i] = dim;
            i++;
        }
        return radialDims;
    }

    internal void connectTo(Ring ring2)
    {
        for (int i = 0; i < numVertsOnRing; i++)
        {
            int nextIndex = (i + 1) % numVertsOnRing;
            int b1, b2, t1, t2;
            b1 = generator.indexMap[verts[i]];
            b2 = generator.indexMap[verts[nextIndex]];
            t1 = generator.indexMap[ring2.verts[i]];
            t2 = generator.indexMap[ring2.verts[nextIndex]];
            IndexTriangle[] linkingTriangles = new IndexTriangle[] { new IndexTriangle(b1, b2, t1), new IndexTriangle(t1, t2, b2) };
            generator.addTriangles(linkingTriangles);
        }
    }
}
