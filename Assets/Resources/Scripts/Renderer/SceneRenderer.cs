using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneRenderer : MonoBehaviour
{
    public static SceneRenderer singleton;

    public GameObject object4DRendererPrefabLink;
    public Player cameraEntity; // position of player
    HashSet<Entity> entities;

    public Camera camera2D;
    public static Vector3 camera2DOffset = new Vector3(0,0,-20);
    public Camera camera3D;

    public static Camera camera2 { get => singleton.camera2D; }
    public static Camera camera3 { get => singleton.camera3D; }
    private static Player player { get => singleton.cameraEntity; }


    internal static GameObject object4DRendererPrefab { get => singleton.object4DRendererPrefabLink; }

    internal static float q { get => singleton.cameraEntity.getPosition().q; }
    internal static float x { get => singleton.cameraEntity.getPosition().x; }
    internal static float y { get => singleton.cameraEntity.getPosition().y; }
    internal static float z { get => singleton.cameraEntity.getPosition().z; }



    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("starting scene renderer");
        entities = new HashSet<Entity>();
        singleton = this;
    }

    internal static void registerEntity(Entity entity)
    {
        singleton.entities.Add(entity);
    }

    public static void update3DMeshes()
    {
        //Debug.Log("updating q pos: " + player.position.q + " y pos: " + player.position.y);
        foreach (Entity entity in singleton.entities)
        {
            entity.shape4.update3DMesh(entity.positionType);
        }
    }

    public static void update2DMeshes()
    {
        foreach (Entity entity in singleton.entities)
        {
            entity.shape4.update2DMesh(entity.positionType);
        }
    }

    public static void updateCameras() {
        camera2.transform.position = new Vector3(player.getPosition().q, player.getPosition().y, 0) + camera2DOffset; ;
        //Debug.Log("setting cam3 pos to: " + player.playerController.cameraAnchor.transform.position);
        camera3.transform.position = player.playerController.cameraAnchor.transform.position;
        camera3.transform.rotation = player.playerController.cameraAnchor.transform.rotation;
    }
}
