using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Walk 
{
    //a walk is a cycle with no branches

    private List<int> orderedIndicies;
    private HashSet<int> allVertices;

    public int this[int index]
    {
        get
        {
            if (orderedIndicies.Count > index)
            {
                return orderedIndicies[index];
            }
            
            throw new Exception("");
        }
    }

    public List<int> vertices
    {
        get => orderedIndicies;
    }

    public HashSet<int> vertexSet
    {
        get => allVertices;
    }

    public int vertCount { get => orderedIndicies.Count; }


    public Walk()
    {
        orderedIndicies = new List<int>();
        allVertices = new HashSet<int>();
    }

    public Walk(List<int> orderedIndicies)
    {
        this.orderedIndicies = orderedIndicies;
        allVertices = new HashSet<int>();
        allVertices.UnionWith(orderedIndicies);
    }

    internal HashSet<IndexLine> getLines()
    {
        HashSet<IndexLine> lines = new HashSet<IndexLine>();
        for (int i = 0; i < vertCount; i++)
        {
            int nextID = (i + 1) % vertCount;
            lines.Add(new IndexLine(vertices[i], vertices[nextID]));
        }
        return lines;
    }

    internal HashSet<IndexTriangle> getTriangles()
    {
        int anchor = orderedIndicies[0];
        HashSet<IndexTriangle> triangles = new HashSet<IndexTriangle>();

        for (int i = 1; i < orderedIndicies.Count - 1; i++)
        {
            int second = orderedIndicies[i];
            int third = orderedIndicies[i + 1];
            IndexTriangle tri = new IndexTriangle(anchor, second, third);
            triangles.Add(tri);
        }
        return triangles;
    }

    internal void addVert(int nextVert)
    {
        orderedIndicies.Add(nextVert);
        allVertices.Add(nextVert);
    }

    internal Walk intersect(int ending)
    {
        //returns a cyclic walk which begins at vert[0], and ends at the given ending
        int endIndex = orderedIndicies.IndexOf(ending);
        if (endIndex == 0) {
            return this;
        }
        if (endIndex < 2) {
            throw new Exception("cannot intersect at given point ("+ending+"). a walk should be at least 3 indicies. " + this);
        }
        Walk a = new Walk(orderedIndicies.GetRange(0,endIndex + 1));
        Walk b = new Walk(orderedIndicies.GetRange(endIndex, orderedIndicies.Count - endIndex -1));
        if (a.vertCount > b.vertCount) {
            return a;
        }
        return b;
    }

    internal bool contains(int vert)
    {
        return allVertices.Contains(vert);
    }

    public override string ToString()
    {
        string ret = "walk (";
        for (int i = 0; i < orderedIndicies.Count; i++)
        {
            ret += orderedIndicies[i] + ",";
        }
        return ret + ") ";
    }

    internal int getsecondLastVert()
    {
        return orderedIndicies[orderedIndicies.Count - 2];
    }

    public bool doesFaceWalkContainPoint(Shape shape, Face face, Vector candidatePosition)
    {
        /*
         * find an arbitrary perpendicular axis coplanar to the face. this axis, u is used to project from the candidate position
         * if the number of lines the projection crosses is even, then vert is not enclosed by the walk, if odd - it is enclosed
         */

        Vector u = face.otherRealLine.getDiff();
        int intersectionCount = 0;
        for (int i = 0; i < vertCount; i++)
        {
            int vertA = vertices[i];
            int vertB = vertices[(i + 1) % vertCount];

            IndexLine indexLine = new IndexLine(vertA, vertB);
            RealLine realLine = indexLine.getRealLine(shape);

            if (realLine.doesVectorIntersectLine(candidatePosition, u))
            {
                intersectionCount++;
            }
        }
        return intersectionCount % 2 == 1;
    }

    internal void addVerts(List<int> orderedVerts, bool reverse = false)
    {
        if (!reverse)
        {
            foreach (int vert in orderedVerts)
            {
                addVert(vert);
            }
        }
        else {
            List<int> reverseList = orderedVerts.ToList();
            reverseList.Reverse();
            addVerts(reverseList);
        }
    }
}
