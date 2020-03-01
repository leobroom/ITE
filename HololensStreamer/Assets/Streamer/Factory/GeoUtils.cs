using System.Collections.Generic;
using UnityEngine;

//##################################################################
// Author Leon Brohmann
// E-Mail: Leonbrohmann@gmx.de | leon.brohmann@tu-braunschweig.de
//##################################################################

/// <summary>
/// HelperClass to Convert Data from Networkclass to UnityFormat
/// </summary>
public static class GeoUtils
{
    /// <summary>
    /// Fixed Scaling Issue
    /// </summary>
    private const int SCALE = 1000;

    public static Vector3 GetVector(float[] floats) => new Vector3(floats[0] / SCALE, floats[1] / SCALE, floats[2] / SCALE);

    public static Vector3[] GetVector3Array(float[] floats)
    {
        int length = floats.Length / 3;
        Vector3[] vecs = new Vector3[length];

        for (int i = 0; i < length; i++)
        {
            int a = i * 3;
            Vector3 pos = new Vector3(floats[a] / SCALE, floats[a + 1] / SCALE, floats[a + 2] / SCALE);

            vecs[i] = pos;
        }

        return vecs;
    }

    public static List<Vector3> GetVector3List(float[] floats)
    {
        int length = floats.Length / 3;
        List<Vector3> vecs = new List<Vector3>(length);

        for (int i = 0; i < length; i++)
        {
            int a = i * 3;
            Vector3 pos = new Vector3(floats[a] / SCALE, floats[a + 1] / SCALE, floats[a + 2] / SCALE);

            vecs.Add(pos);
        }

        return vecs;
    }

    public static Color GetUColor(byte[] colors)
    {
        float r = GetColorValue(colors[0]);
        float g = GetColorValue(colors[1]);
        float b = GetColorValue(colors[2]);
        float a = GetColorValue(colors[3]);

        return new Color(r, g, b, a);
    }

    private static float GetColorValue(byte v)
        => ((int)v) / 255f;
}