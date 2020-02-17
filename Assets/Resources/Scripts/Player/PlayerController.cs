using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Player player;
    public Transform cameraAnchor;
    private float movementSpeed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector4 keysMovement = Vector4.zero;
        if (Input.GetKey(KeyCode.Z))
        {
            keysMovement[3] -= 1;
        }
        if (Input.GetKey(KeyCode.X))
        {
            keysMovement[3] += 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            keysMovement[0] -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            keysMovement[0] += 1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            keysMovement[2] += 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            keysMovement[2] -= 1;
        }



        keysMovement = keysMovement.normalised * Time.deltaTime * movementSpeed;
        Vector3[] localAxis = new Vector3[] { player.shape3.transform.right, player.shape3.transform.up, player.shape3.transform.forward };
        Vector4 realMovement = Vector4.zero;
        for (int i = 0; i < 3; i+=2)
        {//x and z
            realMovement += (new MyVector3(localAxis[i]) * keysMovement[i]).addQDim(0);
            //Debug.Log("adding movement: " + (new MyVector3(localAxis[i]) * keysMovement[i]).addQDim(0));
        }
        realMovement[3]= keysMovement[3];//q   

        if (player.shape3.checkMovedPhysically() || false)
        {
            try
            {
                SceneRenderer.update2DMeshes();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }
        }
        if (player.shape2.checkMovedPhysically())
        {
            try
            {
                SceneRenderer.update3DMeshes();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
            }
        }

        player.shape4.checkIfMovedPhysically();
        if (realMovement.magnitude > 0)
        {
            player.movePosition(realMovement);
        }

        Vector2 cameraMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        player.shape3.transform.RotateAroundLocal(player.shape3.transform.up, cameraMovement.x);
        cameraAnchor.transform.RotateAround(player.shape3.transform.right, -cameraMovement.y * Time.deltaTime);
        SceneRenderer.updateCameras();
    }
}
