using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSimplifier : ShapeOperator
{
    internal static void removeRedundantLinesFromShape(Shape shape, bool debug = false)
    {
        ShapeSimplifier simplifier = new ShapeSimplifier(shape);
        simplifier.findLineIntersections(debug: debug);
        simplifier.removeRedundantVertices(debug: debug);
        simplifier.removeRedundantVertices(debug: debug);
    }

    public ShapeSimplifier(Shape shape)
    {
        this.shape = shape;
    }


    public void removeRedundantVertices(bool debug = false, int remainingRecurses = 5)
    {
        bool removedLine = false;
        if (remainingRecurses == 0)
        {
            throw new Exception("");
        }
        //Debug.Log("number of lines on shape " + shape+ " before removal: " + shape.lines.Count);
        foreach (int intersection in lineIntersections.Keys)
        {
            if (lineIntersections[intersection].Count == 2)
            {
                //if the two lines coming in and out of this vert are parallel - fuse them
                IndexLine[] oldLines = new IndexLine[2];
                int[] externalPoints = new int[2];
                RealLine[] oldRealLines = new RealLine[2];
                Vector sharedPoint;
                Vector[] externalRealPoints = new Vector[2];
                int i = 0;
                foreach (LinePoint line in lineIntersections[intersection])
                {
                    oldLines[i] = line.line;
                    externalPoints[i] = line.getOtherPoint();
                    oldRealLines[i] = line.line.getRealLine(shape);
                    sharedPoint = oldRealLines[i][line.index];
                    externalRealPoints[i] = oldRealLines[i][1 - line.index];

                    i++;
                }
                if (oldLines[0].Equals(oldLines[1]))
                {
                    throw new Exception("identical lines " + oldLines[0] + " and " + oldLines[1] + " intersecting at vert: " + intersection);
                }
                if (!shape.containsLine(oldLines[0]))
                {
                    continue;
                    //throw new Exception("lines in intersections " + intersection + " not present in shape - " + oldLines[0]);
                }
                if (!shape.containsLine(oldLines[1]))
                {
                    continue;
                    //throw new Exception("lines in intersections " + intersection + " not present in shape - " + oldLines[1]);
                }
                //Vector line1 = oldRealLines[0].getDiff().getRounded(Vector.normalEqualityTolerance);
                //Vector line2 = oldRealLines[1].getDiff().getRounded(Vector.normalEqualityTolerance);

                bool isPar = oldRealLines[0].isParallelTo(oldRealLines[1], tolerance: 0.9f);
                //bool isPar = line1.isParallelTo(line2);//todo test rounded v non
                if (!isPar) continue;
                if (debug)
                {
                    Debug.Log("found redundant lines: " + oldLines[0] + " and " + oldLines[1]);
                }
                //either a connects to b or a is contained in b 
                IndexLine newLine;
                if (oldRealLines[0].doesLineContainPoint(externalRealPoints[1]) && false)
                {//line 0 contains line 1
                    Debug.LogWarning("found line containing other");
                    newLine = oldLines[0];
                    removedLine = removedLine || shape.removeLine(oldLines[1]);
                }
                else if (oldRealLines[1].doesLineContainPoint(externalRealPoints[0]) && false)
                {//line 1 contains line 0
                    Debug.LogWarning("found line containing other");
                    newLine = oldLines[1];
                    removedLine = removedLine || shape.removeLine(oldLines[0]);
                }
                else
                {
                    newLine = new IndexLine(externalPoints[0], externalPoints[1]);
                    shape.removeLine(oldLines[0]);
                    shape.removeLine(oldLines[1]);
                    if (externalPoints[0] == externalPoints[1])
                    {
                        if (debug)
                        {
                            Debug.LogWarning("line starting and ending at " + externalPoints[0] + " found");
                        }
                        continue;
                    }

                    shape.addLine(newLine);
                }


            }
            else if (lineIntersections[intersection].Count == 1)
            {
                throw new Exception();
            }
            //else if (lineIntersections[intersection].Count == 4) {
            //    throw new Exception();
            //}
        }
        //Debug.Log("number of lines on shape " + shape + "  after removal: " + shape.lines.Count);
        findLineIntersections();

        if (removedLine)
        {
            //removeRedundantVertices(debug: false, remainingRecurses: remainingRecurses -1);
        }
    }



}
