using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Walks 
{

    public static Walk getEnclosingwalk(Shape shape, Face face, Dictionary<int, HashSet<LinePoint>> lineIntersections)
    {
        int start = face.getASafeIndex(lineIntersections);
        Walk startingCycle;
        try
        {
            startingCycle = getAWalkFrom(start, new HashSet<IndexLine>(face.lines), lineIntersections);
        }
        catch (Exception e)
        {
            Texture2D lines =DebugImage.drawFaceLines(face, shape);
            if (e is DeadEndException deadEnd)
            {
                DebugImage.drawWalk(deadEnd.walk,face, shape, lines);
            }
            
            throw e;
        }

        /*
         * from the starting walk, go through each new line on the face, check if it is in the current biggest cycle
         * if it is, find its pair, remove the point they cut out, and add the pair
         */

        Walk enclosingWalk = expandWalkCycle(startingCycle, new HashSet<IndexLine>(face.lines), lineIntersections, face, shape);

        return enclosingWalk;
    }

    private static Walk expandWalkCycle(Walk currentBiggestCycle, HashSet<IndexLine> remainingLines, Dictionary<int, HashSet<LinePoint>> lineIntersections, Face face, Shape shape)
    {
        /*
         * Start from any point on the cycle which branches
         * for each branch that is external to the cycle - follow it until it rejoins the cycle
         * once it rejoins the cycle, find which old cycle points are enclosed in the new larger cycle
         */
        HashSet<IndexLine> cycleLines = currentBiggestCycle.getLines();
        remainingLines.ExceptWith(cycleLines);

        if (remainingLines.Count == 0) {
            return currentBiggestCycle;
        }
        if (remainingLines.Count == 1) {
            throw new Exception("stray line");
        }
        int branchSeed = -1;
        LinePoint branch = null;
        foreach (int vert in lineIntersections.Keys)
        {
            HashSet<LinePoint> intersections = lineIntersections[vert];
            if (intersections.Count == 1) {
                throw new Exception();
            }
            if (intersections.Count == 2) { continue; }//not a branch
            // is  a branch

            foreach (LinePoint candidate in intersections)
            {
                if (isLineExternal(candidate, currentBiggestCycle, remainingLines, shape, face)) {
                    //this branch is external to the current biggest cycle
                    branchSeed = vert;
                    branch = candidate;
                    break;
                }
            }

            if (branch != null) { break; }// has found a valid  branch already
        }
        if (branch == null) {// none of the branches were external, the current cycle is fully enclosing
            return currentBiggestCycle;
        }

        // here there is at least one branch coming off the current cycle which is external

        int nextPoint = branch.getOtherPoint();
        List<int> orderedExternalPoints = new List<int>();
        HashSet<int> externalPoints = new HashSet<int>();
        externalPoints.Add(nextPoint);
        orderedExternalPoints.Add(nextPoint);

        while (!currentBiggestCycle.contains(nextPoint)) {
            nextPoint = getNextValidExternalPointFromBranch(currentBiggestCycle, nextPoint, externalPoints, remainingLines, lineIntersections ,shape, face);
            externalPoints.Add(nextPoint);
            orderedExternalPoints.Add(nextPoint);
        }

        /*
         * here external points is a chain which connects to the current cycle at branchSeed and nextPoint
         * there are two possible cycles which can be made - the larger of the two (area) should be chosen
         * ie: the one which contains the other
         */

        Walk expandedCycle = mergeCurrentCycleWithExternalChain(currentBiggestCycle, orderedExternalPoints, branchSeed, nextPoint, shape, face);

        return expandWalkCycle(expandedCycle, remainingLines, lineIntersections, face, shape);//recursively continue to add chains until there are no more candidates
    }

    private static Walk mergeCurrentCycleWithExternalChain(Walk currentBiggestCycle, List<int> orderedExternalPoints, int connectA, int connectB, Shape shape, Face face)
    {
        /*
         * create both the walk from (A->B->chain->A) and (B->A->chain->B)
         * decide which walk contains the other, return the container
         * the chain should start just after connectA and end just before connectB
         */

        int connectionIndexA = currentBiggestCycle.vertices.IndexOf(connectA);
        int connectionIndexB = currentBiggestCycle.vertices.IndexOf(connectB);

        Walk walkA = new Walk();
        Walk walkB = new Walk();

        bool aDone = false;
        bool bDone = false;

        for (int i = 0; i < currentBiggestCycle.vertCount; i++)
        {//go clockwise around cycle from both connection points
            int aIndex = (i + connectionIndexA) % currentBiggestCycle.vertCount;
            int bIndex = (i + connectionIndexB) % currentBiggestCycle.vertCount;

            if (!aDone)
            {
                walkA.addVert(aIndex);
            }
            if (!bDone)
            {
                walkB.addVert(bIndex);
            }

            if (!aDone && aIndex == connectionIndexB) {
                //has reached other connection point - cut out slack and add in chain
                walkA.addVerts(orderedExternalPoints, reverse: true);
                aDone = true;
            }
            if (!bDone && bIndex == connectionIndexA)
            {
                //has reached other connection point - cut out slack and add in chain
                walkA.addVerts(orderedExternalPoints);
                aDone = true;
            }
        }

        if (walkA.doesFaceWalkContainPoint(shape, face, shape.getVertexAt(walkB[1]) )){
            return walkA;
        }
        return walkB;
    }

    private static int getNextValidExternalPointFromBranch(Walk currentBiggestCycle, int currentPoint, HashSet<int> externalPoints, HashSet<IndexLine> remainingLines, Dictionary<int, HashSet<LinePoint>> lineIntersections, Shape shape, Face face)
    {
        /*
         * trying to return to the cycle. will only move along lines which are external
         * mustn't move back onto any point already collected in external points
         */

        HashSet<LinePoint> intersections = lineIntersections[currentPoint];
        if (intersections.Count == 1)
        {
            throw new Exception();
        }

        foreach (LinePoint candidate in intersections) {
            if (currentBiggestCycle.contains(currentPoint) || isLineExternal(candidate, currentBiggestCycle, remainingLines, shape, face)) {
                int candidatePoint = candidate.getOtherPoint();
                if (externalPoints.Contains(candidatePoint)) { continue; }//this point is already included
                return candidatePoint;
            }
        }
        throw new Exception("failed to find next external point");
    }


    private static bool isLineExternal(LinePoint candidate, Walk currentBiggestCycle, HashSet<IndexLine> remainingLines, Shape shape, Face face)
    {
        if (!remainingLines.Contains(candidate.line)) { return false; }//not a candidate
        //is a candidate
        RealLine realCandidate = candidate.line.getRealLine(shape);
        if (!currentBiggestCycle.doesFaceWalkContainPoint(shape, face, realCandidate.getCenterPoint()))
        {
            //this branch is external to the current biggest cycle
            return true;
        }
        else
        {
            remainingLines.Remove(candidate.line);//this branch is internal - throw it away
            return false;
        }
    }

    public static Walk getAWalkFrom(int currentVert, HashSet<IndexLine> lineCandidates, Dictionary<int, HashSet<LinePoint>> lineIntersections, Walk walk = null) {
        if (walk == null)
        {
            walk = new Walk();
        }

        if (walk.contains(currentVert))
        {
            //have returned to starting point. work is done
            return walk.intersect(currentVert);
        }

        walk.addVert(currentVert);
        Debug.Log("traversing from vert: " + currentVert);

        HashSet<LinePoint> linePointsIntersectionsOnFace = new HashSet<LinePoint>();
        foreach (LinePoint linePoint in lineIntersections[currentVert])
        {//collects the lines connected to nextVert, which are also candidates for this walk eg: on the relevant face
            if (lineCandidates.Contains(linePoint.line))
            {
                linePointsIntersectionsOnFace.Add(linePoint);
                Debug.Log("including line(" + linePoint.line + ")");
            }
            else {
                Debug.Log("line ("+ linePoint.line + ") at current vert, not a candidate of: {" + String.Join(",", lineCandidates) + "}");
            }
        }
        int numPaths = linePointsIntersectionsOnFace.Count;
        if (numPaths < 2)
        {
            throw new DeadEndException("dead end at "+currentVert+" - num paths: " + numPaths + " line candidates: " + lineCandidates.Count + " walk length: " + walk.vertCount, walk);
        }

        int previousVertId = -1;

        foreach (LinePoint linePoint in linePointsIntersectionsOnFace)
        {
            if (walk.vertCount == 1)
            {
                //has just begun, chose starting direction arbitrarily
                int nextVertInWalk = linePoint.getOtherPoint();
                return getAWalkFrom(nextVertInWalk, lineCandidates, lineIntersections, walk: walk);
            }
            //has picked a direction. ensure that the next point is in that dir

            previousVertId = walk.getsecondLastVert();
            int nextVertcandidaate = linePoint.getOtherPoint();
            //Debug.Log("coming from " + currentVert + " candidate: " + nextVertcandidaate + " previous: " + previousVertId + " walk: " + walk);
            if (nextVertcandidaate != previousVertId)
            {
                //not going backwards   
                return getAWalkFrom(nextVertcandidaate,lineCandidates ,lineIntersections, walk:walk);
            }
        }

        throw new Exception("failed to find next point on walk");
    }

    
}

[Serializable]
internal class DeadEndException : Exception
{
    public Walk walk;

    public DeadEndException(string message, Walk walk) : base(message)
    {
        this.walk = walk;
    }

}