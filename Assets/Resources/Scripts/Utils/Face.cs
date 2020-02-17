using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Face
{
    public List<int> orderedIndicies;
    public Vector normal;
    public RealLine otherRealLine;

    public HashSet<IndexLine> lines;

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
        String ret = "Face (norm = " + normal + " indicies: (";
        for (int i = 0; i < orderedIndicies.Count; i++)
        {
            ret += orderedIndicies[i] + ",";
        }
        return ret + ")) ";
    }

    internal HashSet<IndexTriangle> getTriangles()
    {
        int anchor = getAnIndex();


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

    private int getAnIndex()
    {
        foreach (IndexLine line in lines)
        {
            return line[0];
        }
        throw new Exception();
    }

    private int getASafeIndex(Dictionary<int, HashSet<LinePoint>> lineIntersections)
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
                        Debug.LogWarning("intersection: " + intersection);
                    }
                }
            }
        }
        throw new Exception();
    }

    internal void calculateOrderedVerts(Shape shape, Dictionary<int, HashSet<LinePoint>> lineIntersections, bool debug = false)
    {//perform convex 

        //if (debug)
        //{
        //    foreach (IndexLine line in lines)
        //    {
        //        RealLine realLine = line.getRealLine(shape);
        //        Vector3 a = ((MyVector3)realLine[0]).vector;
        //        Vector3 b = ((MyVector3)realLine[1]).vector;

        //        Debug.DrawLine(a, b);
        //        Vector3 mid = (a + b) * 0.5f;
        //        //Debug.DrawLine(mid, mid + ((MyVector3)normal).vector);

        //        for (int side = 0; side < 2; side++)
        //        {
        //            HashSet<IndexLine> connectedLines = new HashSet<IndexLine>();
        //            int vertId = line[side];
        //            //Debug.Log("number of lines at " + vertId + " = " + lineIntersections[vertId].Count);
        //            foreach (LinePoint lineP in lineIntersections[vertId]) {
        //                if (lines.Contains(lineP.line)) {
        //                    connectedLines.Add(lineP.line);
        //                }
        //            }
        //            if (connectedLines.Count != 2) {
        //                Debug.LogError("strage number of face lines connected at vert " + vertId + " num: " + connectedLines.Count + " total lines at intersect: " + lineIntersections[vertId].Count);
        //                foreach (LinePoint linePoint in lineIntersections[vertId])
        //                {
        //                    Debug.Log("line at strange intersect: " + linePoint);
        //                }
        //                Vector3 troublePoint = ((MyVector3)realLine[0]).vector;
        //                Debug.DrawLine(troublePoint, troublePoint + ((MyVector3)normal).vector);
        //            }
        //        }
        //    }
        //}
        orderedIndicies = new List<int>();
        int start = getASafeIndex(lineIntersections);
        if (debug)
        {
            Debug.Log(shape + "," + this + " starting on " + start);
        }
        traverseLines(start, lineIntersections, shape, debug: debug);
        if (orderedIndicies.Count != lines.Count) {
            throw new Exception();
        }
    }

    public int getSecondLastPoint() {
        return orderedIndicies[orderedIndicies.Count - 2];
    }

    private void traverseLines(int vertId, Dictionary<int, HashSet<LinePoint>> lineIntersections, Shape shape, bool debug = false)
    {
        if (orderedIndicies.Count > 0 && orderedIndicies[0] == vertId)
        {
            //have returned to starting point. work is done
            if (debug)
            {
                Debug.Log(this + " finished trversing");
            }
            return;
        }
        if (orderedIndicies.Contains(vertId))
        {
            if (debug)
            {
                foreach (LinePoint linePoint in lineIntersections[vertId])
                {
                    Debug.LogError(this + " line point connected to error: " + linePoint);
                }
            }
            throw new Exception(this + " trying to add duplicte vert " + vertId + " to " + this + " debug: " + debug);
        }

        orderedIndicies.Add(vertId);
        if (debug)
        {
            Debug.Log(this + " traversing from: " + vertId);
        }
        HashSet<LinePoint> linePointsIntersectionsOnFace = new HashSet<LinePoint>();
        foreach (LinePoint linePoint in lineIntersections[vertId])
        {
            if (lines.Contains(linePoint.line))
            {
                linePointsIntersectionsOnFace.Add(linePoint);
            }
        }
        if (linePointsIntersectionsOnFace.Count == 3) {
            //check if both are valid
            foreach (LinePoint linePoint in lineIntersections[vertId])
            {
                int outGoingVert = linePoint.getOtherPoint();
                if (outGoingVert == getSecondLastPoint()) { continue; }//returning line
                //the next two lines are the fork
                //if(lineIntersections[outGoingVert].Contains(lin))
                Debug.LogError("num intersections at end of fork " + linePoint + " =" + linePoint.getNumIntersectionsAtOtherPoint(lineIntersections));
            }
        }
        if (linePointsIntersectionsOnFace.Count != 2)
        {
            foreach (LinePoint linePoint in lineIntersections[vertId])
            {
                RealLine rl = linePoint.line.getRealLine(shape);
                Vector newNormal = rl.cross(otherRealLine).getNormalised();

                
                if (debug)
                {
                    Debug.LogError(linePoint + " is on face: " + isLineInFace(linePoint, shape) + "\nreal: " + rl + "\toriginal: " + otherRealLine +
                   "\nline norm: " + newNormal + "face norm: " + normal + "\nnorms par: " + newNormal.isParallelTo(normal) + " line par with orig " + otherRealLine.isParallelTo(rl));

                    RealLine realLine = linePoint.line.getRealLine(shape);
                    Vector3 a = ((MyVector3)realLine[0]).vector;
                    Vector3 b = ((MyVector3)realLine[1]).vector;
                    //Debug.DrawLine(a, b);
                    //Visualiser.visualiseLines(shape);
                }
            }
            throw new Exception("strange number of linePoints on the face at vert " + vertId + " = " + linePointsIntersectionsOnFace.Count + " total lines at intersection: " + lineIntersections[vertId].Count);
        }
        int movedToVert = -1;
        int previousVertId = -1;

        foreach (LinePoint linePoint in linePointsIntersectionsOnFace)
        {
            if (orderedIndicies.Count == 1)
            {
                //has just begun, chose starting direction arbitrarily
                int nextVert = linePoint.getOtherPoint();
                if (debug)
                {
                    Debug.Log(this + " setting trv dir with " + linePoint + " moving to vert " + nextVert);
                }
                traverseLines(nextVert, lineIntersections, shape, debug: debug);
                return;
            }
            //haas picked a direction. ensure that the next point is in tht dir
            if (movedToVert == -1)
            {
                previousVertId = orderedIndicies[orderedIndicies.Count - 2];
            }
            int nextVertcandidaate = linePoint.getOtherPoint();
            if (nextVertcandidaate != previousVertId)
            {
                //not going backwards   
                if (movedToVert != -1) { throw new Exception("already moved from " + vertId + " to " + movedToVert + " now trying to move to" + nextVertcandidaate + " previous: " + previousVertId); }
                movedToVert = nextVertcandidaate;
                if (debug)
                {
                    Debug.Log(this + " traversing on " + linePoint + " moving to vert " + nextVertcandidaate);
                }
                traverseLines(nextVertcandidaate, lineIntersections, shape, debug: debug);
                return;
            }
        }
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
}
