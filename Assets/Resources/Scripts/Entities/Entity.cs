using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public enum PositionType {
        PLAYER_RELATIVE, ABSOLUTE 
    }

    public PositionType positionType = PositionType.ABSOLUTE;
    public bool useGravity;
    public bool isKinematic;

    private Vector4 position = Vector4.zero;
    [NonSerialized] public Shape4DRenderer shape4;
    [NonSerialized] public Shape3DRenderer shape3;
    [NonSerialized] public Shape3DRenderer shape2;

    public Shape4DGenerator shapeGenerator;
    public Material material;

    HashSet<Entity> subEntities;

    private void Start()
    {
        start();
    }

    protected void start()
    {
        subEntities = new HashSet<Entity>();

        generateShape();
        setPosition(shapeGenerator.getPosition(), true);//move objects away before turing on colliders
        configurePhysics();
    }

    private void Update()
    {
        if (shape4 == null) { return; }
        shape4.checkIfMovedPhysically();
    }

    public Vector4 getPosition() {
        return position;
    }

    public void movePosition(Vector4 movement) {
        //Debug.Log("moving position by: " + movement);
        position = position +  movement;
        updateSubshapePhysicalPositions();
    }

    public void setPosition(Vector4 newPos, bool force = false) {
        position = newPos * 1;
        updateSubshapePhysicalPositions(force);
    }

    public void generateShape()
    {
        Shape4D shape4 = shapeGenerator.getShape();
        //Debug.Log("creating shape renderer for entity " + gameObject);
        GameObject rendererObject = Instantiate<GameObject>(SceneRenderer.object4DRendererPrefab);
        rendererObject.transform.position = Vector3.zero;
        Shape4DRenderer renderer = rendererObject.GetComponent<Shape4DRenderer>();
        renderer.setShape(shape4, positionType, this);
        shape3 = renderer.shape3Rend;
        shape2 = renderer.shape2Rend;
        this.shape4 = renderer;
        SceneRenderer.registerEntity(this);


        shape3.meshRenderer.material = material;
        shape2.meshRenderer.material = material;


        for (int i = 0; i < shapeGenerator.transform.childCount; i++)
        {
            Transform child = shapeGenerator.transform.GetChild(i);
            Vector3 localPos = child.localPosition;
            Quaternion localRot = child.localRotation;

            child.parent = shape3.transform;
            child.localPosition = localPos;
            child.localRotation = localRot;
        }
    }

    private void configurePhysics()
    {
        shape3.meshCollider.enabled = true;
        shape2.meshCollider.enabled = true;

        shape2.body.isKinematic = isKinematic;
        shape3.body.isKinematic = isKinematic;

        shape2.body.mass = shapeGenerator.getMass();
        shape3.body.mass = shapeGenerator.getMass();

        shape2.body.useGravity = useGravity;
        shape3.body.useGravity = useGravity;
    }

    void updateSubshapePhysicalPositions(bool force = false) {
        //Debug.Log(gameObject + " setting 3d pos of " + shape3.gameObject + " to: " + position.dropQ().vector + " old pos: " + shape3.lastPhysicalPosition + " rep pos: " + position);
        //if (positionType == PositionType.PLAYER_RELATIVE) {
        shape2.moveToPysicalPosition(new Vector3(position.q, position.y, 0), force: force, debug: false);
        shape3.moveToPysicalPosition(position.dropQ().vector, force: force, debug: false);
        //}

    }
}
