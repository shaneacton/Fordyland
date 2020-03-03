using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape3DRenderer : MonoBehaviour
{
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public MeshRenderer meshRenderer;

    Mesh mesh;
    public Rigidbody body;

    public Vector3 lastPhysicalPosition;

    public void start()
    {
        body = GetComponent<Rigidbody>();
    }

    public bool checkMovedPhysically() {

        if (transform.position != lastPhysicalPosition)
        {
            return true;
        }
        return false;
    }

    public bool checkMovedPhysicallyAndUpdate() {
        if (transform.position != lastPhysicalPosition)
        {
            //Debug.Log(gameObject + " has moved from " + lastPhysicalPosition + " to " + transform.position);
            lastPhysicalPosition = transform.position * 1f;
            return true;
        }
        return false;
    }

    public void resetLastPhysicalposition(Vector3 newPhysicalPosition) {
        lastPhysicalPosition = newPhysicalPosition * 1f;
    }

    public void moveToPysicalPosition(Vector3 newPhysicalPosition, bool force = false, bool debug = false) {
        if (debug)
        {
            Debug.Log("moving to physical position: " + newPhysicalPosition + " last pos: " + lastPhysicalPosition);
        }
        newPhysicalPosition = newPhysicalPosition * 1;//deep copy
        resetLastPhysicalposition(newPhysicalPosition);
        if (force) {
            transform.position = newPhysicalPosition;
        }
        else
        {
            //if (debug)
            //{
            //    Debug.Log("using rb.move to go to " + newPhysicalPosition + " updated old pos to :" + lastPhysicalPosition);
            //}
            body.MovePosition(newPhysicalPosition);
        }
    }

    public void updateMesh(Vector3[] vertices, int[] triangles) {
        if (mesh == null) {
            mesh = new Mesh();
            mesh.MarkDynamic();
            meshFilter.mesh = mesh;
        }
        //Debug.Log("updating mesh with " + vertices.Length + " verts and " + triangles.Length + " triangles");
        if (vertices.Length == 0)
        {
            //throw new Exception("cannot update mesh with zero vertices");
            gameObject.SetActive(false);
        }
        else {
            gameObject.SetActive(true);
        }
        mesh.triangles = new int[0];
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        meshCollider.sharedMesh = mesh;

    }
}
