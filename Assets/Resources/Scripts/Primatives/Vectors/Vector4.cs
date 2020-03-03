using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Vector4 : Vector
{
    public float x, y, z, q;
    internal static Vector4 zero { get => new Vector4(0,0,0,0); }

    public Vector4 normalised { get => this * (magnitude > 0 ? (1f / magnitude) : 0); }
    public float magnitude {
        get =>(float) Math.Pow(Math.Pow(x,2) + Math.Pow(y, 2) + Math.Pow(z, 2) + Math.Pow(q, 2), 0.5f);
    }

    public Vector4(float x, float y, float z, float q) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.q = q;
    }

    public Vector4() {
    }

    public float this[int index]
    {
        get
        {
            if (index == 0) {
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
            if (index == 3)
            {
                return q;
            }
            throw new Exception("");

        }

        set
        {
            if (index == 0)
            {
                x = value;
            }
            if (index == 1)
            {
                y = value;
            }
            if (index == 2)
            {
                z = value;
            }
            if (index == 3)
            {
                q = value;
            }
        }
    }

    internal MyVector3 dropDim(int dim)
    {
        float[] vals = new float[getNumDims() -1];
        int i = 0;
        for (int d = 0; d < getNumDims(); d++)
        {
            if (d == dim) continue;
            vals[i] = this[d];
            i++;
        }
        return new MyVector3(vals);
    }

    internal MyVector3 dropQ()
    {
        return dropDim(3);
    }

    public static Vector4 operator +(Vector4 a, Vector b) {
        if (b is Vector4)
        {
            Vector4 b4 = (Vector4)b;
            return new Vector4(a.x + b4.x, a.y + b4.y, a.z + b4.z, a.q + b4.q);
        }
        throw new Exception("can only perform ops on same types, expected vec4 - not: " + b);
    }
    public static Vector4 operator -(Vector4 a) {
        return new Vector4(-a.x, -a.y, -a.z, -a.q);
    }
    public static Vector4 operator -(Vector4 a, Vector b) {
        if (b is Vector4)
        {
            Vector4 b4 = (Vector4)b;
            return new Vector4(a.x - b4.x, a.y - b4.y, a.z - b4.z, a.q - b4.q);
        }
        throw new Exception();
    }
    public static Vector4 operator *(Vector4 a, float b)
    {
        return new Vector4(a.x *b, a.y * b, a.z * b, a.q * b);
    }
    public override string ToString()
    {
        return "Vector4("+x+","+y + "," + z + "," + q +")";
    }

    public override int GetHashCode()
    {
        var hashCode = 1474027755;
        for (int i = 0; i < 4; i++)
        {
            hashCode = hashCode * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this[i]);
        }
        return hashCode;
    }

    public override bool Equals(object obj)
    {
        return obj is Vector4 vector &&
               x == vector.x &&
               y == vector.y &&
               z == vector.z &&
               q == vector.q;
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

    public override float mag()
    {
        return this.magnitude;
    }

    public override Vector getRounded(float quanta)
    {
        return new Vector4(quantiseValue(x, quanta), quantiseValue(y, quanta), quantiseValue(z, quanta), quantiseValue(q, quanta));
    }

    public override int getNumDims()
    {
        return 4;
    }


    public override float getValueOfDimension(int index)
    {
        return this[index];
    }

    public override void setValueOfdimension(int dim, float value)
    {
        this[dim] = value;
    }

    public override Vector getNormalised()
    {
        throw new NotImplementedException();
    }

    public override bool isParallelTo(Vector normal, float tollerance = -1)
    {
        throw new NotImplementedException();
    }

    internal override float dot(Vector u)
    {
        throw new NotImplementedException();
    }
}
