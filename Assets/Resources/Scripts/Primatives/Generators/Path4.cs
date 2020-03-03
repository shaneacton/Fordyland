using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path4 : Shape4DGenerator
{
    public static float unitsPerBlob = 1;//the distance the drag can move without laying down a new blob
    public int holdoutDim;//the dim which the path blobs will be shifted along. no two consequtive nodes can have the same value in this dim
    public int verticalDim;//the dim which the path blobs will be shifted along. no two consequtive nodes can have the same value in this dim

    Vector4 start = new Vector4(0, 0, 0, -5);
    Vector4 end = new Vector4(25, 0, 5, 10);

    override protected void initShape()
    {
        base.initShape();
        createPath();
        commitVerts();
    }

    private void createPath()
    {
        Blob blob1 = new Blob(this, start, 3, 1, 10, 2, (x => x* 5 + 5));
        Blob blob2 = new Blob(this, end, 3, 1, 10, 2, (x => x * 5 + 5));
        blob1.connectTo(blob2);

    }

    public override float getMass()
    {
        return 50;
    }
}
