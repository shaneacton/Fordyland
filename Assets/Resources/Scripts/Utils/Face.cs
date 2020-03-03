using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Face
{
    public Vector normal;
    public RealLine otherRealLine;

    public HashSet<IndexLine> lines;

    public Walk walk;


    public Face(Vector normal, RealLine otherLine)
    {
        if (normal.mag() == 0) throw new Exception("cannot start face with a zero normal");
        lines = new HashSet<IndexLine>();
        this.normal = getAlignedNormal(normal).getNormalised().getRounded(Vector.normalEqualityTolerance);
        this.otherRealLine = otherLine;
    }

    private Vector getAlignedNormal(Vector normal)
    {
        if (normal[0] == 0 && normal[1] < 0)
        {
            normal *= -1;
        }
        if (normal[0] < 0) { normal *= -1; }//aligns normals so two parallel but opposite normals will be equal
        for (int dim = 0; dim < normal.getNumDims(); dim++)
        {
            if (normal[dim] < 0) { return normal * -1; }
            if (normal[dim] > 0) { return normal; }
            //val at dim must have been zero, try align via the next dim
        }
        throw new Exception("cannot align zero vec: " + normal);

    }

    internal void add(IndexLine line)
    {
        if (lines.Contains(line))
        {
            //Debug.LogError("trying to add duplicate point to face: " + nextPoint);
            throw new Exception("trying to add duplicate line to face: " + line);
        }
        lines.Add(line);
    }

    public override bool Equals(object obj)
    {
        return obj is Face otherFace && lines.SetEquals(otherFace.lines);
    }

    internal bool containsLine(IndexLine line)
    {
        return lines.Contains(line);
    }

    public override string ToString()
    {
        return "Face {norm = " + normal + ", " + walk + "}";
    }

    private int getAnIndex()
    {
        if (lines == null) {
            return walk.vertices[0];
        }
        foreach (IndexLine line in lines)
        {
            return line[0];
        }
        throw new Exception();
    }

    public int getASafeIndex(Dictionary<int, HashSet<LinePoint>> lineIntersections)
    {//returns an arbitrary vert which has exactly 2 lines intersecting it
        foreach (IndexLine line in lines)
        {
            for (int i = 0; i < 2; i++)
            {
                int vertId = line[i];
                int count = 0;
                foreach (LinePoint intersection in lineIntersections[vertId])
                {
                    if (lines.Contains(intersection.line)) {
                        count++;
                    }
                }
                if (count == 2)
                {
                    return vertId;
                }
                else {
                    Debug.LogWarning("intersections at vert " + vertId + " = " + lineIntersections[vertId].Count + " line: "  + line);
                    foreach (LinePoint intersection in lineIntersections[vertId]) {
                        if (!lines.Contains(intersection.line)) { continue; }
                        //Debug.LogWarning("intersection: " + intersection);
                    }
                }
            }
        }
        throw new Exception();
    }

    internal void findLines(IndexLine nextLine, Dictionary<int, HashSet<LinePoint>> lineIntersections, Shape shape, int remainingRecurses = 20, bool debug = false)
    {
        if (remainingRecurses == 0)
        {
            throw new Exception("run out of recurses gathering a face");
        }
        //recursively move around the face verts until no more connected lines found

        if (containsLine(nextLine))
        {
            throw new Exception(this + " already contains the line " + nextLine);
        }
        add(nextLine);

        for (int side = 0; side < 2; side++)
        {//try propagate from both sides of the line
            LinePoint linePoint = new LinePoint(nextLine, side);
            int vertId = linePoint.getPoint();
            if (!lineIntersections.ContainsKey(vertId))
            {
                throw new Exception("no line intersections for vert: " + vertId + " pointed to by line: " + nextLine);
            }

            foreach (LinePoint candidateLine in lineIntersections[vertId])
            {//for each line coming off nextLine
                //Debug.Log("considering " + otherLine + " coming off point: " + face.getLastIndex());
                if (containsLine(candidateLine.line)) continue; // line is returning back to acounted for verts
                if (isLineInFace(candidateLine, shape))
                {
                    findLines(candidateLine.line, lineIntersections, shape, remainingRecurses - 1, debug: debug);
                }
            }
        }
    }

    internal void fillLines()
    {
        //if this face is open (not a closed cycle) - fill it in by connecting the dead ends
        Dictionary<int, int> vertCount = new Dictionary<int, int>(); // maps each vert to the number of lines it is a part of
        foreach (IndexLine line in lines)
        {
            for (int i = 0; i < 2; i++)
            {
                int vert = line[i];
                if (!vertCount.ContainsKey(vert))
                {
                    vertCount.Add(vert, 1);
                }
                else {
                    vertCount[vert]++;
                }
            }
        }

        HashSet<int> deadEndVerts = new HashSet<int>();
        foreach (int vert in vertCount.Keys)
        {
            if (vertCount[vert] < 2)
            {
                deadEndVerts.Add(vert);
            }
        }
        if (deadEndVerts.Count % 2 != 0) {
            throw new Exception();
        }
        if (deadEndVerts.Count > 2) {
            throw new Exception();
        }
        if (deadEndVerts.Count == 2) {
            int[] deadEnds = deadEndVerts.ToArray();
            add(new IndexLine(deadEnds[0], deadEnds[1]));
        }

    }

    bool isLineInFace(LinePoint candidateLine, Shape shape)
    {
        RealLine candidateRealLine = candidateLine.line.getRealLine(shape);
        Vector newNormal = candidateRealLine.cross(otherRealLine);
        return newNormal.isParallelTo(normal) || otherRealLine.isParallelTo(candidateRealLine);
    }

    internal bool contains(IndexLine line1, IndexLine line2)
    {
        return lines.Contains(line1) && lines.Contains(line2);
    }

    public Vector getU()
    {//returns 1 of the two perp axis which define the face plane
        return otherRealLine.getDiff().getNormalised();
    }

    public Vector getV(Shape shape)
    {//returns the other of the two perp axis which define the face plane
        if (shape.getVertexAt(0).getNumDims() == 2)
        {
            //the shape this face is on is 2d
            MyVector2 u = (MyVector2)otherRealLine.getDiff();
            return new MyVector2(-u.y, u.x).getNormalised();//prep to u
        }
        else if (shape.getVertexAt(0).getNumDims() == 3)
        {
            //3d shape
            MyVector3 u = (MyVector3)otherRealLine.getDiff();
            return u.cross((MyVector3)normal).getNormalised();
        }
        throw new Exception();
    }
}
