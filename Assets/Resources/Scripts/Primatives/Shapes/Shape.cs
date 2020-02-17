using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shape
{
    public HashSet<IndexTriangle> triangles;
    private HashSet<IndexLine> lines;
    public Dictionary<Vector, int> verticesIndexMap;
    protected Dictionary<int, Vector> indicesVertexMap;
    public int numVertices;


    public Shape()
    {
        triangles = new HashSet<IndexTriangle>();
        lines = new HashSet<IndexLine>();
        verticesIndexMap = new Dictionary<Vector, int>();
        indicesVertexMap = new Dictionary<int, Vector>();
        numVertices = 0;
    }

    public void addVertex(Vector vertex) {
        if (verticesIndexMap.ContainsKey(vertex)) return;
        verticesIndexMap.Add(vertex, numVertices);
        indicesVertexMap.Add(numVertices, vertex);
        numVertices++;
    }

    internal bool containsVertex(Vector vertex)
    {
        return verticesIndexMap.ContainsKey(vertex);
    }

    public Vector getVertexAt(int index) {
        if (!indicesVertexMap.ContainsKey(index)) {
            throw new Exception("no vertex for index: " + index);
        }
        return indicesVertexMap[index];
    }

    public int getIndexOf(Vector vertex) {
        return verticesIndexMap[vertex];
    }

    public HashSet<IndexLine> getLines() {
        return lines;
    }

    public void addLine(IndexLine line) {
        if (containsLine(line)) {
            //throw new Exception("adding duplicate line to shape");
            //Debug.LogWarning("adding duplicate line to shape");
        }
        lines.Add(line);
    }

    internal bool removeLine(IndexLine line)
    {
        return lines.Remove(line);
    }

    internal bool containsLine(IndexLine indexLine)
    {
        return lines.Contains(indexLine);
    }
}