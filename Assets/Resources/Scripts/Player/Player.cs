using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("starting player");
        positionType = PositionType.PLAYER_RELATIVE;
        base.start();
        SceneRenderer.update2DMeshes();
        SceneRenderer.update3DMeshes();
    }

    private void Update()
    {
        //override
    }

}
