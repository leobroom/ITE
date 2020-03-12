using System.Collections.Generic;
using UnityEngine;

//##################################################################
// Author Leon Brohmann
// E-Mail: Leonbrohmann@gmx.de | leon.brohmann@tu-braunschweig.de
//##################################################################

/// <summary>
/// An ugly helper Script to Orient the woorldcoordiantes - will be replaced later by a vuforia solution
/// </summary>
public class Orientation
{
    private static Orientation instance;

    /// <summary>
    /// List of alle the created spheres in orient mode
    /// </summary>
    private readonly List<GameObject> orientSpheres = new List<GameObject>();

    private Orientation() { }

    public static Orientation Instance
    {
        get
        {
            if (instance == null)
                instance = new Orientation();

            return instance;
        }
    }

    /// <summary>
    /// When 2 Spheres are created, the worldCoordiates are changed 
    /// </summary>
    /// <param name="hitposition"></param>
    /// <param name="reset"></param>
    public void SetOrient(Vector3 hitposition, out bool reset)
    {
        Debug.Log("SetOrient");

        if (orientSpheres.Count == 2)
        {
            CreateOrientSphere(hitposition);
            SetWorldCoordinates();
            reset = true;
        }
        else
        {
            reset = false;
            CreateOrientSphere(hitposition);
        }
    }

    /// <summary>
    /// Creates a simple Sphere on the Floor
    /// </summary>
    /// <param name="pos"></param>
    private void CreateOrientSphere(Vector3 pos)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        go.GetComponent<Collider>().enabled = false;

        float s = 0.02f;
        go.transform.localScale = new Vector3(s, s, s);
        go.transform.position = pos;

        orientSpheres.Add(go);
    }

    /// <summary>
    /// Sets the SetWorldCoordinates to a new Rotation/ Position 
    /// </summary>
    private void SetWorldCoordinates()
    {
        Vector3 a = orientSpheres[0].transform.position;
        Vector3 b = orientSpheres[1].transform.position;
        Vector3 c = orientSpheres[2].transform.position;

        Vector3 forward = b - a;
        Vector3 right = c - a;
        Vector3 up = Vector3.Cross(forward, right);

        var worldCoord = GeometryStreamer.Instance.WorldCoordinates.transform;
        worldCoord.gameObject.SetActive(true);
        worldCoord.localPosition = a;
        worldCoord.localRotation = Quaternion.LookRotation(forward, up);

        Vector3 localRot = worldCoord.localRotation.eulerAngles;

        //Entfernt Kippwinkel
       worldCoord.localRotation = Quaternion.Euler(0, localRot.y, 0);

        Vector3 movingVec = (worldCoord.forward + worldCoord.right) * 0.5f;
        worldCoord.localPosition += movingVec;
    }

    /// <summary>
    /// Resets the Values
    /// </summary>
    public void Reset()
    {
        foreach (var go in orientSpheres)
            GameObject.Destroy(go);

        orientSpheres.Clear();
    }
}