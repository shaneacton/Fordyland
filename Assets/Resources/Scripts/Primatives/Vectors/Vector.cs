using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vector : VectorImpl
{
    public static float equalityTolerance = 0.0001f;//0.001
    public static float normalEqualityTolerance = 0.1f;
    public static float parallelTolerance = 0.992f;

    public abstract Vector getRounded(float quanta);
    public abstract float mag();
    public abstract Vector minus(Vector b);
    public abstract Vector plus(Vector b);
    public abstract Vector times(float val);

    public abstract int getNumDims();

    public abstract void setValueOfdimension(int dim, float value);
    public abstract float getValueOfDimension(int dim);

    public abstract Vector getNormalised();

    public float distance(Vector b)
    {
        return b.minus(this).mag();
    }

    public static Vector operator +(Vector a, Vector b)
    {
        return a.plus(b);
    }
    public static Vector operator -(Vector a, Vector b)
    {
        return a.minus(b);
    }
    public static Vector operator *(Vector a, float b)
    {
        return a.times(b);
    }

    public static float quantiseValue(float val, float quanta) {
        float lower = val - (val % quanta);
        float upper = lower + quanta;
        if (Mathf.Abs(val - lower) < Mathf.Abs(val - upper)) {
            return lower;
        }
        return upper;
    }

    public float this[int index]
    {
        get
        {
            return this.getValueOfDimension(index);
        }

        set
        {
            setValueOfdimension(index, value);
        }
    }

    public abstract bool isParallelTo(Vector normal, float tollerance = -1);

 
}
