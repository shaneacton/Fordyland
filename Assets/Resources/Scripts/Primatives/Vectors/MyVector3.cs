using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyVector3 : Vector
{
    public Vector3 vector;
    internal static MyVector3 zero { get => new MyVector3(0,0,0); }

    public MyVector3(float x, float y, float z) {
        vector = new Vector3(x, y, z);
    }

    public MyVector3(float [] vals)
    {
        vector = new Vector3(vals[0], vals[1], vals[2]);
    }

    public MyVector3(Vector3 vec) {
        vector = vec * 1 ;
    }


    public MyVector3 normalised { get => magnitude ==0? zero:  this * (1f / magnitude) ; }
    public float magnitude { get => vector.magnitude; }
    public float x {get => vector.x; set { vector[0] = value; } }
    public float y { get => vector.y; set { vector[1] = value; } }
    public float z { get => vector.z; set { vector[2] = value; } }


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
            if (index == 2)
            {
                return z;
            }
            throw new Exception("");
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
            if (index == 2)
            {
                z = value;
                return;
            }
            throw new Exception("index must be {0,1,2} not: " + index);
        }
    }

    internal MyVector3 cross(MyVector3 other)
    {
        return new MyVector3(Vector3.Cross(other.vector, vector));
    }

    internal override float dot(Vector other)
    {
        if (other is MyVector3 other3)
        {
            return Vector3.Dot(vector, other3.vector);
        }
        throw new Exception();
    }

    public static MyVector3 operator - (MyVector3 a, Vector b)
    {
        if (b is MyVector3)
        {
            MyVector3 b3 = (MyVector3)b;
            return new MyVector3(a.x - b3.x, a.y - b3.y, a.z - b3.z);
        }
        throw new Exception();
    }
    public static MyVector3 operator + (MyVector3 a, Vector b)
    {
        if (b is MyVector3)
        {
            MyVector3 b3 = (MyVector3)b;
            return new MyVector3(a.x + b3.x, a.y + b3.y, a.z + b3.z);
        }
        throw new Exception();
    }
    public static MyVector3 operator *(MyVector3 a, float b)
    {
        return new MyVector3(a.x * b, a.y * b, a.z * b);
    }
    public override string ToString()
    {
        return "MyVector3(" + x + ", " + y + ", " + z + ")";
    }

    public override int GetHashCode()
    {
        var hashCode = 1474027755;
        MyVector3 rounded = (MyVector3)getRounded(equalityTolerance);
        for (int i = 0; i < 3; i++)
        {
            hashCode = hashCode * -1521134295 + EqualityComparer<float>.Default.GetHashCode(rounded[i]);
        }
        return hashCode;
    }

    public override bool Equals(object obj)
    {
        return obj is MyVector3 vector &&
               ((MyVector3)(getRounded(equalityTolerance) - vector.getRounded(equalityTolerance))).magnitude < equalityTolerance;
    }

    public override bool isParallelTo(Vector other, float tollerance = -1)
    {
        if (tollerance == -1) {
            tollerance = parallelTolerance;
        }
        if (other is MyVector3 other3)
        {
            float dot = other3.normalised.dot(this.normalised);
            return Mathf.Abs(dot) > tollerance;
        }
        throw new Exception();
    }

    public override float mag()
    {
        return this.magnitude; 
    }

    public override Vector minus(Vector b)
    {
        return this - b;
    }

    public override Vector plus(Vector b)
    {
        return this + b;
    }

    public override Vector times(float val)
    {
        return this * val;
    }

    public override Vector getRounded(float quanta)
    {
        return new MyVector3(quantiseValue(x, quanta), quantiseValue(y, quanta), quantiseValue(z, quanta));
    }

    public override int getNumDims()
    {
        return 3;
    }

    public override void setValueOfdimension(int dim, float value)
    {
        this[dim] = value;
    }
    public override float getValueOfDimension(int dim)
    {
        return this[dim];
    }

    public override Vector getNormalised()
    {
        return normalised;
    }

    internal MyVector2 dropDim(int dim)
    {
        float[] vals = new float[getNumDims() - 1];
        int i = 0;
        for (int d = 0; d < getNumDims(); d++)
        {
            if (d == dim) continue;
            vals[i] = this[d];
            i++;
        }
        return new MyVector2(vals);
    }

    public Vector4 addQDim(float qVal) {
        return new Vector4(x, y, z, qVal);
    }
}
