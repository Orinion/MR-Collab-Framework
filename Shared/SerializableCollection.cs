using System;
using UnityEngine;

/// <summary>
/// The list of used DataChannels
/// </summary>
public enum DataChannels
{
    Reserved_Audio,
    Reserved_Video,
    LeftHand,
    RightHand,
    Head,
    Marker,
    SpatialMesh,
    TaskData
}


/// <summary>
/// Serializable implementation of the Vector3
/// </summary>
[Serializable]
public class Vector3S
{
    /// <summary>
    /// x component
    /// </summary>
    public float x;

    /// <summary>
    /// y component
    /// </summary>
    public float y;

    /// <summary>
    /// z component
    /// </summary>
    public float z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public Vector3S(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3(Vector3S rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3S(Vector3 rValue)
    {
        return new Vector3S(rValue.x, rValue.y, rValue.z);
    }

    public bool Equals(Vector3 v) { return v != null && x == v.x && y == v.y && z == v.z; }
}


/// <summary>
/// Serializable implementation of the Vector3
/// </summary>
[Serializable]
public class Vector4S
{
    public float w;
    /// <summary>
    /// x component
    /// </summary>
    public float x;

    /// <summary>
    /// y component
    /// </summary>
    public float y;

    /// <summary>
    /// z component
    /// </summary>
    public float z;

    /// <summary>
    /// Constructor
    /// </summary>
    public Vector4S(float rW,float rX, float rY, float rZ)
    {
        w = rW;
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}, {3}]", w, x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector4(Vector4S rValue)
    {
        return new Vector4(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector4S(Vector4 rValue)
    {
        return new Vector4S(rValue.w,rValue.x, rValue.y, rValue.z);
    }

    public bool Equals(Vector4 v) { return v != null && w == v.w && x == v.x && y == v.y && z == v.z; }
}

/// <summary>
/// Serializable implementation of the Quaternion
/// </summary>
[Serializable]
public class QuaternionS
{
    /// <summary>
    /// x component
    /// </summary>
    public float x;

    /// <summary>
    /// y component
    /// </summary>
    public float y;

    /// <summary>
    /// z component
    /// </summary>
    public float z;

    /// <summary>
    /// w component
    /// </summary>
    public float w;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public QuaternionS(float rX, float rY, float rZ, float rW)
    {
        x = rX;
        y = rY;
        z = rZ;
        w = rW;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}, {3}]", x, y, z, w);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Quaternion(QuaternionS rValue)
    {
        return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator QuaternionS(Quaternion rValue)
    {
        return new QuaternionS(rValue.x, rValue.y, rValue.z, rValue.w);
    }

    public static QuaternionS operator +(QuaternionS a, QuaternionS b)
    {
        return new QuaternionS(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
    }

    public bool Equals(Quaternion q) { return q != null && x == q.x && y == q.y && z == q.z && w == q.w; }
}

/// <summary>
/// Serializable version of a Transform  (Vector + Quaternion)
/// </summary>
[Serializable]
public class TransformS
{
    public Vector3S pos;
    public QuaternionS rot;
    public Vector3S scale;

    public TransformS(Transform t) { if (t == null) return;
        pos = t.position; rot = t.rotation; scale = t.localScale; }
    public TransformS(Vector3 v, Quaternion q) { pos = v; rot = q; scale = Vector3.one; }

    public bool Equals(TransformS t) { return t != null && this.pos.Equals(t.pos) && this.rot.Equals(t.rot); }

    public bool Equals(Transform t) { return t != null && this.pos.Equals(t.position) && this.rot.Equals(t.rotation) && scale == t.localScale; }

    public override string ToString() { return pos.ToString() + ", " + rot.ToString(); }
}

/// <summary>
/// Serializable version of a Marker  (Vector + Quaternion)
/// </summary>
[Serializable]
public class Marker
{
    public Marker(int id, int model, TransformS t) { this.id = id; this.model = model; this.transform = t; }
    public Marker(int id, int model) { this.id = id; this.model = model; this.transform = null; }

    public int id;
    public int model;
    public TransformS transform;

    public bool Equals(Transform t) { return t==null && transform ==null || t != null && transform != null && transform.Equals(t); }
}


/// <summary>
/// Serializable version of a Matrix
/// </summary>
[Serializable]
public class MatrixS
{
    Vector4S c0,c1,c2,c3;

    public MatrixS(Vector4S c0, Vector4S c1, Vector4S c2, Vector4S c3)
    {
        this.c0 = c0;
        this.c1 = c1;
        this.c2 = c2;
        this.c3 = c3;
    }

    public static implicit operator MatrixS(Matrix4x4 r)
    {
        return new MatrixS(r.GetColumn(0), r.GetColumn(1), r.GetColumn(2), r.GetColumn(3));
    }

    public static implicit operator Matrix4x4(MatrixS r)
    {
        return new Matrix4x4(r.c0, r.c1,r.c2,r.c3);
    }
}

/// <summary>
/// Serializable Object for the progress indicator
/// </summary>
[Serializable]
public class TaskData
{
    public float progress;
    public string description;

    public TaskData(float progress, string description)
    {
        this.progress = progress;
        this.description = description;
    }
}