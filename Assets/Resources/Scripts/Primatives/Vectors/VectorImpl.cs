using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface VectorImpl
{
    float distance(Vector b);
    float mag();
    int getNumDims();
    Vector minus(Vector b);
    Vector plus(Vector b);
    Vector times(float val);
    Vector getRounded(float quanta);
}
