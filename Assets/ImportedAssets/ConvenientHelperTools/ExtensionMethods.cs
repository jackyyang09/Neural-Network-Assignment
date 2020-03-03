using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Returns a Vector3 with the same value in x, y and z
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public static Vector3 UniformVector(this Vector3 vec, float val)
    {
        return new Vector3(val, val, val);
    }

    /// <summary>
    /// Returns a Vector3 with the same value in x, y and z
    /// </summary>
    /// <param name="vec"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    public static Vector3 UniformVector(this Vector3 vec, int val)
    {
        return new Vector3(val, val, val);
    }
}
