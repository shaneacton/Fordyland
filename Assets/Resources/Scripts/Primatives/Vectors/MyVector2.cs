using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyVector2 : Vector
{
    internal static MyVector2 zero = new MyVector2(0, 0);

    public Vector2 vector;

    public MyVector2(float x, float y)
    {
        vector = new Vector2(x, y);
    }

    public MyVector2(float[] vals)
    {
        vector = new Vector2(vals[0], vals[1]);
    }

    public MyVector2(Vector2 vec)
    {
        vector = vec * 1;
    }

    public float this[int index]
    {
        get
        {
            if (index == 0)
            {
                return x;
            }
            if (index == 1)
            {
                return y;
            }
            throw new Exception("index must be 0,1 - not: " + index);
        }

        set
        {
            if (index == 0)
            {
                x = value;
                return;
            }
            if (index == 1)
            {
                y = value;
                return;
            }
            throw new Exception("index must be {0,1,2} not: " + index);
        }
    }

    public float x { get => vector.x; set { vector[0] = value; } }
    public float y { get => vector.y; set { vector[1] = value; } }

    public MyVector2 normalised { get => this * (1f / magnitude); }

    public float magnitude { get => vector.magnitude; }


    public override int getNumDims()
    {
        return 2;
    }

    public override Vector getRounded(float quanta)
    {
        return new MyVector2(quantiseValue(x, quanta), quantiseValue(y, quanta));
    }

    public override float getValueOfDimension(int dim)
    {
        return this[dim];
    }
    public override bool isParallelTo(Vector other, float tollerance = -1)
    {
        if (tollerance == -1) {
            tollerance = parallelTolerance;
        }
        if (other is MyVector2 other2) {
            float dot = other2.normalised.dot(this.normalised);
            return Mathf.Abs(dot) > tollerance;
        }
        throw new Exception();
    }
    internal override float dot(Vector other)
    {
        if (other is MyVector2 other2)
        {
            return Vector2.Dot(vector, other2.vector);
        }
        throw new Exception();
    }

    public override float mag()
    {
        return this.magnitude;
    }

    public static MyVector2 operator -(MyVector2 a, Vector b)
    {
        if (b is MyVector2)
        {
            MyVector2 b3 = (MyVector2)b;
            return new MyVector2(a.x - b3.x, a.y - b3.y);
        }
        throw new Exception();
    }
    public static MyVector2 operator +(MyVector2 a, Vector b)
    {
        if (b is MyVector2)
        {
            MyVector2 b3 = (MyVector2)b;
            return new MyVector2(a.x + b3.x, a.y + b3.y);
        }
        throw new Exception();
    }
    public static MyVector2 operator *(MyVector2 a, float b)
    {
        return new MyVector2(a.x * b, a.y * b);
    }

    public override Vector minus(Vector b)
    {
        return this - b;
    }

    public override Vector plus(Vector b)
    {
        return this + b;
    }

    public override void setValueOfdimension(int dim, float value)
    {
        this[dim] = value;
    }

    public override Vector times(float val)
    {
        return this * val;
    }

    public override Vector getNormalised()
    {
        return normalised;
    }

    public MyVector3 addDimWithVal(int val)
    {
        return new MyVector3(x, y, val);
    }

    public override int GetHashCode()
    {
        var hashCode = 1474027755;
        MyVector2 rounded = (MyVector2)getRounded(equalityTolerance);
        for (int i = 0; i < 2; i++)
        {
            hashCode = hashCode * -1521134295 + EqualityComparer<float>.Default.GetHashCode(rounded[i]);
        }
        return hashCode;
    }

    public override bool Equals(object obj)
    {
        return obj is MyVector2 vector &&
               ((MyVector2)(getRounded(equalityTolerance) - vector.getRounded(equalityTolerance))).magnitude < equalityTolerance;
    }

    internal MyVector2 swizzle()
    {
        return new MyVector2(y, x);
    }
}
