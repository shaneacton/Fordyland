using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Entity;

public class Shape4DRenderer : MonoBehaviour
{
    public static Vector3 offsetFor2dObjects = new Vector3(0, 0, 0);

    Shape4D shape4;
    public GameObject objectRendererPrefab;

    public Shape3DRenderer shape3Rend;
    public Shape3DRenderer shape2Rend;
    public Entity ownerEntity;

    public void setShape(Shape4D shape4, PositionType posType, Entity owner) {
        this.shape4 = shape4;
        this.ownerEntity = owner;

        GameObject object3D = Instantiate<GameObject>(objectRendererPrefab);
        GameObject object2D = Instantiate<GameObject>(objectRendererPrefab);
        object3D.transform.parent = transform;
        object2D.transform.parent = transform;
        object2D.transform.position += offsetFor2dObjects;

        shape3Rend = object3D.GetComponent<Shape3DRenderer>();
        shape2Rend = object2D.GetComponent<Shape3DRenderer>();

        object2D.layer = LayerMask.NameToLayer("2D");
        object3D.layer = LayerMask.NameToLayer("3D");

        shape2Rend.start();
        shape3Rend.start();

        update3DMesh(posType);
        update2DMesh(posType);
    }

    public void checkIfMovedPhysically() {
        // if the physics engine has moved either the 2d, or 3d shape - refresh the pos4 of the entity to reflect
        bool movedPhysically3D = shape3Rend.checkMovedPhysicallyAndUpdate();
        bool movedPhysically2D = shape2Rend.checkMovedPhysicallyAndUpdate();

        //Debug.Log("moved 3d: " + shape3Rend.checkMovedPhysically() + " 2d: " + shape2Rend.checkMovedPhysically());

        if (movedPhysically3D || movedPhysically2D) {
            if (shape3Rend.transform.position.y != shape2Rend.transform.position.y) {
                //Debug.LogError("4d objects 2d and 3d variants have differing physical y vals 3d: " + shape3Rend.transform.position + " , 2d: " + shape2Rend.transform.position);
            }
            Vector4 movedToPosition = new Vector4(shape3Rend.transform.position.x, shape3Rend.transform.position.y, shape3Rend.transform.position.z, shape2Rend.transform.position.x);
            //Debug.Log("physics moved a body to: " + movedToPosition + " updating rep pos");

            ownerEntity.setPosition(movedToPosition);
        }
    }

    public void update3DMesh(PositionType posType) {
        Shape3D shape3 = null;
        if (posType == PositionType.ABSOLUTE)
        {
            shape3 = shape4.getShapeAtQ(SceneRenderer.q - ownerEntity.getPosition().q);
        }
        else if (posType == PositionType.PLAYER_RELATIVE) {
            shape3 = shape4.getShapeAtQ(0);
        }
        Triangulator.triangulateShape(shape3, debug: false);
        shape3Rend.updateMesh(shape3.getVertexList(), shape3.getMeshTriangles());
    }

    public void update2DMesh(PositionType posType)
    {
        //Debug.Log("rerendering shape 2");
        Shape3D shape3 = null;
        if (posType == PositionType.ABSOLUTE)
        {
            shape3 = shape4.getShape3AtXZ(SceneRenderer.x - ownerEntity.getPosition().x, SceneRenderer.z - ownerEntity.getPosition().z);
        }
        else if (posType == PositionType.PLAYER_RELATIVE)
        {
            shape3 = shape4.getShape3AtXZ(0, 0);
        }
        //Visualiser.visualiseLines(shape3);
        Triangulator.triangulateShape(shape3, debug: false);
        //Visualiser.visualiseLines(shape3);

        shape2Rend.updateMesh(shape3.getVertexList(), shape3.getMeshTriangles());
    }
}
