using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulator: ShapeOperator
{
    Dictionary<Vector, int> clumps; // maps the clump point to its index
    Dictionary<int, int> unclumpedToClumpedIds;
    int nextClumpIndex;

    public Triangulator(Shape shape) {
        this.shape = shape;
    }

    public void triangulateShape(bool debug = false) {
        ShapeSimplifier.removeRedundantLinesFromShape(shape);
        //removeRedundantVertices(debug: debug);
        findLineIntersections(debug: debug);
        HashSet<Face> faces = findFaces(debug: debug);
        if (debug) {
            //Visualiser.visualiseFaces(faces, shape);
        }
        if (faces == null) {
            return;
        }
        foreach (Face face in faces)
        {
            triangulateFace(face);
        }
    }

    private void triangulateFace(Face face)
    {
        HashSet<IndexTriangle> newTriangles = face.getTriangles();
        foreach (IndexTriangle triangle in newTriangles)
        {
            if (shape.triangles.Contains(triangle)) {
                Debug.LogError("shape already contains triangle : " + triangle + " duplicate originating from: " + face);
                continue;
            }
            shape.triangles.Add(triangle);
        }
    }

    private HashSet<Face> findFaces(bool debug = false)
    {
        Dictionary<IndexLine, int> facesAccounted = new Dictionary<IndexLine, int>();// maps each line to the number of faces found - a line is done when it has 2 faces
        Dictionary<Vector, HashSet<Face>> normals = new Dictionary<Vector, HashSet<Face>>();// maps each existing aligned normal to all the unique faces which have that normal
        HashSet<Face> faces = new HashSet<Face>();


        foreach (int vertId in lineIntersections.Keys)
        {
            foreach (LinePoint line in lineIntersections[vertId])
            {
                RealLine realLine = line.line.getRealLine(shape);
                foreach (LinePoint otherLine in lineIntersections[vertId])
                {
                    if (otherLine.Equals(line)) continue;
                    //Debug.Log("found pair of lines hitting the same point: " + line + ", " + otherLine);
                    RealLine otherRealLine = otherLine.line.getRealLine(shape);
                    Vector faceNormal = realLine.cross(otherRealLine);//don't use this normal, it is not yet aligned
                    if (faceNormal.mag() == 0) {
                        continue;//don't start a face with two parallel lines
                    }
                    Face face = new Face(faceNormal, otherRealLine);

                    bool faceAccountedFor = false;
                    if (normals.ContainsKey(face.normal))
                    {
                        foreach (Face otherface in normals[face.normal])
                        {
                            //here face and otherFace share a normal, if they also share lines - they are the same face
                            if (otherface.contains(line.line, otherLine.line))
                            {
                                faceAccountedFor = true;
                                break;
                            }
                        }
                    }
                    if (faceAccountedFor) continue;

                    face.findLines(otherLine.line, lineIntersections, shape, debug: debug);
                    face.calculateOrderedVerts(shape, lineIntersections ,debug: debug);

                    if (!normals.ContainsKey(face.normal))
                    {
                        //Debug.Log("found new face normal: " + face.normal);
                        normals.Add(face.normal, new HashSet<Face>());
                    }
                    else {
                        //Debug.Log("found duplicate face normal" + face.normal);
                    }
                    
                    normals[face.normal].Add(face);
                    faces.Add(face);
                    //Debug.Log("found face: " + face);
                }
            }
        }

        //Visualiser.visualiseFaces(faces, shape);

        //Debug.Log("found " + faces.Count + " faces");

        foreach (Vector normal in normals.Keys)
        {
            foreach (Vector otherNormal in normals.Keys)
            {
                if (normal == otherNormal) { continue; }
                //for each pair of normals - check that no faces are common
                foreach (Face face in normals[otherNormal])
                {
                    if (normals[normal].Contains(face)) {
                        throw new Exception();
                    }
                }
            }

        }

        return faces;
    }

    internal static void triangulateShape(Shape shape, bool debug = false)
    {
        Triangulator trior = new Triangulator(shape);
        //trior.clumpPoints();
        trior.triangulateShape(debug: debug);
    }


    //public void naiveTriangulateShape(int remainingRecurses = 1)
    //{
    //    /*
    //     * recursively triangulates the shapes vertices by adding to to lines and triangles
    //     */

    //    findLineIntersections();
    //    bool addedNewTriangles = naiveTriangulateIntersections();
    //    lineIntersections = null;
    //    if (addedNewTriangles)
    //    {
    //        //Debug.Log("added new triangles");

    //        if (remainingRecurses > 0)
    //        {
    //            naiveTriangulateShape(remainingRecurses - 1);
    //        }
    //        else {
    //            Debug.LogWarning("run out of recurses");
    //        }
    //    }
    //}


    //private bool naiveTriangulateIntersections()
    //{
    //    bool addedNewTriangles = false;

    //    int clumpPoint;
    //    int secondPoint;
    //    int thirdPoint;

    //    for (int i = 0; i < lineIntersections.Count; i++)
    //    {
    //        foreach (LinePoint lineP1 in lineIntersections[i])
    //        {
    //            clumpPoint = lineP1.getPoint();
    //            secondPoint = lineP1.getOtherPoint();
    //            foreach (LinePoint lineP2 in lineIntersections[i])
    //            {
    //                if (lineP1.line.Equals(lineP2.line)) { continue; }
    //                thirdPoint = lineP2.getOtherPoint();
    //                if (secondPoint == thirdPoint) {
    //                    throw new Exception("linePoints: " + lineP1 + " and " + lineP2 + " have equal other points: " + secondPoint + ", " + thirdPoint +  " lines eq: " + (lineP1.line.start == lineP2.line.start) + "," + (lineP1.line.end == lineP2.line.end) + ", " + (lineP1.line == lineP2.line) + " .eq: " + lineP1.line.Equals(lineP2.line));
    //                }
    //                IndexTriangle newTriangle = new IndexTriangle(clumpPoint, secondPoint, thirdPoint);
    //                if (!shape.triangles.Contains(newTriangle)) {
    //                    //Debug.Log("added tringle: " + newTriangle);
    //                    shape.triangles.Add(newTriangle);
    //                    addedNewTriangles = true;
    //                }
    //                shape.lines.Add(new IndexLine(secondPoint, thirdPoint));
    //            }
    //        }
    //    }
    //    return addedNewTriangles;
    //}
}
