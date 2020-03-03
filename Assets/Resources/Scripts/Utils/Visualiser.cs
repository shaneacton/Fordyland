using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualiser : MonoBehaviour
{
    Shape4D shape;
    Shape3D shape3;
    Shape2D shape2;

    float qRadius =10;
    MyVector3 qDimProjection;

    float qVal = 0.5f;
    float xVal = 0.5f;
    float zVal = 0.5f;

    public MeshFilter meshFilter2;
    public MeshFilter meshFilter3;


    // Start is called before the first frame update
    void Start()
    {
        //shape = new OneWayShrinkingHyperRectangle(1, 2, 3, 1.5f, 0, 0, 0.5f, 0).generatedShape;
        //shape = new HyperRectangle(1, 2, 3, 4);
        //shape = new HyperCube(3f);
        shape3 = shape.getShapeAtQ(qVal);
        Triangulator.triangulateShape(shape3);
        shape2 = shape.getShapeAtXZ(xVal, zVal);
        visualiseMesh2();
        visualiseMesh3();

        qDimProjection = new MyVector3(2, 3, 4).normalised * qRadius;
        //visualiseProgressiveCrossSection();
    }

    

    // Update is called once per frame
    void Update()
    {
        //visualiseProjection();
        //visualiseProgressiveCrossSection();
        //visualiseCrossSection();

        //visualiseLines3D();
        //visualiseLines2D();

        //visualiseTriangles(qVal);

        if (Input.GetKey(KeyCode.Z))
        {
            qVal -= Time.deltaTime;
            shape3 = shape.getShapeAtQ(qVal);
            Triangulator.triangulateShape(shape3);

            visualiseMesh3();
        }
        if (Input.GetKey(KeyCode.X))
        {
            qVal += Time.deltaTime;
            shape3 = shape.getShapeAtQ(qVal);
            Triangulator.triangulateShape(shape3);
            visualiseMesh3();
        }

        if (Input.GetKey(KeyCode.A)) {
            xVal -= Time.deltaTime;
            shape2 = shape.getShapeAtXZ(xVal, zVal);
            visualiseMesh2();
        }

        if (Input.GetKey(KeyCode.D))
        {
            xVal += Time.deltaTime;
            shape2 = shape.getShapeAtXZ(xVal, zVal);
            visualiseMesh2();
        }

        if (Input.GetKey(KeyCode.W))
        {
            zVal -= Time.deltaTime;
            shape2 = shape.getShapeAtXZ(xVal, zVal);
            visualiseMesh2();
        }

        if (Input.GetKey(KeyCode.S))
        {
            zVal += Time.deltaTime;
            shape2 = shape.getShapeAtXZ(xVal, zVal);
            visualiseMesh2();
        }

        if (Input.GetKey(KeyCode.R))
        {
            xVal = 0.5f;
            zVal = 0.5f;
            qVal = 0.5f;
            shape2 = shape.getShapeAtXZ(xVal, zVal);
            visualiseMesh2();
            shape3 = shape.getShapeAtQ(qVal);
            Triangulator.triangulateShape(shape3);
            visualiseMesh3();
        }

    }

    private void visualiseLines2D()
    {
        //Debug.Log("x: " + xVal + " z: " + zVal + " num lines: " + shape2.lines.Count);
        foreach (IndexLine line in shape2.getLines())
        {
            Line2 line2 = line.getLine2(shape2);
            Line3 line3 = line2.addDimWithVal(0);
            Debug.DrawLine(line3.getStart().vector, line3.getEnd().vector);

            MyVector3 cam = new MyVector3(Camera.main.transform.forward);
            Line3 perpLine1 = line3.getPerpendicularThrough(line3.getStart(), cam, 0.2f);
            Line3 perpLine2 = line3.getPerpendicularThrough(line3.getEnd(), cam, 0.2f);
            Debug.DrawLine(perpLine1.getStart().vector, perpLine1.getEnd().vector);
            Debug.DrawLine(perpLine2.getStart().vector, perpLine2.getEnd().vector);
        }
    }

    void visualiseMesh2() {
        meshFilter2.mesh = shape2.getMesh();
        meshFilter2.mesh.RecalculateBounds();
        meshFilter2.mesh.RecalculateNormals();
        meshFilter2.mesh.RecalculateTangents();

        //Debug.Log("verts: " + meshFilter.mesh.vertices.Length);
    }

    private void visualiseMesh3()
    {
        meshFilter3.mesh = shape3.getMesh();
        meshFilter3.mesh.RecalculateBounds();
        meshFilter3.mesh.RecalculateNormals();
        meshFilter3.mesh.RecalculateTangents();
    }

    public static void visualiseTriangles(Shape shape) {
        if (shape is Shape3D shape3) {
            visualiseTriangles3(shape3);
        }
    }

    static void visualiseTriangles3(Shape3D shape3)
    {
        foreach (IndexTriangle triangle in shape3.triangles)
        {
            for (int i = 0; i < 3; i++)
            {
                int nextId = (i + 1) % 3;
                Debug.DrawLine(shape3.getVertex3At(triangle[i]).vector, shape3.getVertex3At(triangle[nextId]).vector);
            }
        }
    }

    public static void visualiseLines(Shape shape)
    {
        if (shape is Shape3D shape3) {
            visualiseLines3(shape3);
        }
        if (shape is Shape2D shape2)
        {
            visualiseLines2(shape2);
        }
    }

    static void visualiseLines3(Shape3D shape3) {
        foreach (IndexLine line in shape3.getLines())
        {
            Line3 line3 = line.getLine3(shape3);
            Vector3 start = shape3.getVertex3At(line.start).vector;
            Vector3 end = shape3.getVertex3At(line.end).vector;
            Debug.DrawLine(start, end);
            MyVector3 cam = new MyVector3(SceneRenderer.camera3.transform.forward);
            //Line3 perpLine1 = line3.getPerpendicularThrough(line3.getStart(), cam, 0.2f);
            //Line3 perpLine2 = line3.getPerpendicularThrough(line3.getEnd(), cam, 0.2f);
            //Debug.DrawLine(perpLine1.getStart().vector, perpLine1.getEnd().vector);
            //Debug.DrawLine(perpLine2.getStart().vector, perpLine2.getEnd().vector);

        }
    }

    static void visualiseLines2(Shape2D shape2)
    {
        foreach (IndexLine line in shape2.getLines())
        {
            Line3 line3 = line.getLine2(shape2).addDimWithVal(0);
            Vector3 start = shape2.getVertex2At(line.start).addDimWithVal(0).vector;
            Vector3 end = shape2.getVertex2At(line.end).addDimWithVal(0).vector;
            Debug.DrawLine(start, end);
            MyVector3 cam = new MyVector3(SceneRenderer.camera3.transform.forward);
            Line3 perpLine1 = line3.getPerpendicularThrough(line3.getStart(), cam, 0.2f);
            Line3 perpLine2 = line3.getPerpendicularThrough(line3.getEnd(), cam, 0.2f);
            Debug.DrawLine(perpLine1.getStart().vector, perpLine1.getEnd().vector);
            Debug.DrawLine(perpLine2.getStart().vector, perpLine2.getEnd().vector);

        }
    }

    private void updateProjection()
    {
        qDimProjection += new MyVector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        qDimProjection = qDimProjection.normalised * qRadius;
    }

    private void visualiseProjection()
    {
        foreach (IndexTriangle triangle in shape.triangles)
        {
            drawTriangle(triangle);
        }
    }

    void visualiseLines3D() {
        //Debug.Log("num lines: " + shape3.lines.Count + ", num verts :" + shape3.numVertices);

        foreach (IndexLine line in shape3.getLines()) {
            Line3 line3 = line.getLine3(shape3);
            Debug.DrawLine(line3.getStart().vector, line3.getEnd().vector);

            MyVector3 cam = new MyVector3(Camera.main.transform.forward);
            Line3 perpLine1 = line3.getPerpendicularThrough(line3.getStart(), cam, 0.2f);
            Line3 perpLine2 = line3.getPerpendicularThrough(line3.getEnd(), cam, 0.2f);
            Debug.DrawLine(perpLine1.getStart().vector, perpLine1.getEnd().vector);
            Debug.DrawLine(perpLine2.getStart().vector, perpLine2.getEnd().vector);
        }
    }

    private void drawTriangle(IndexTriangle triangle)
    {
        for (int i = 0; i < 2; i++)
        {
            Triangle4 tri4 = triangle.getTriangle4(shape);
            int nextId = (i + 1) % 3;
            Vector3 a = getProjection(tri4.points[i]).vector;
            Vector3 b = getProjection(tri4.points[nextId]).vector;
            //Debug.Log("drawing line from " + a + " to: " + b);
            Debug.DrawLine(a, b);
        }
    }

    private MyVector3 getProjection(Vector4 vert)
    {
        MyVector3 baseVert = vert.dropQ();
        return baseVert +  qDimProjection * vert.q;
    }

    internal static void visualiseFaces(HashSet<Face> faces, Shape shape)
    {
        foreach (Face face in faces) {
            visualiseFace(face, shape);
        }
    }

    private static void visualiseFace(Face face, Shape shape)
    {
        if (shape is Shape3D shape3) {
            visualiseFace3(face, shape3);
        }

    }

    private static void visualiseFace3(Face face, Shape3D shape3)
    {
        Walk faceWalk = face.walk;
        for (int i = 0; i < faceWalk.vertCount; i++)
        {
            int vert = faceWalk.vertices[i];
            int nextVert = faceWalk.vertices[(i + 1) % faceWalk.vertCount];
            IndexLine iLine = new IndexLine(vert, nextVert);
            Line3 rLine = iLine.getLine3(shape3);
            Vector3 a = rLine[0].vector;
            Vector3 b = rLine[1].vector;
            Vector3 norm = ((MyVector3)face.normal).vector;

            //Debug.DrawLine(a,b);
            Debug.DrawLine(a, a + norm);
            Debug.DrawLine(b, b + norm);
        }
    }
}
