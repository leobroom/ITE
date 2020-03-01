using System.Collections.Generic;
using UnityEngine;

//##################################################################
// Author Leon Brohmann
// E-Mail: Leonbrohmann@gmx.de | leon.brohmann@tu-braunschweig.de
//##################################################################

/// <summary>
/// A internal Geometry "Database", which stores al the Geometry
/// </summary>
class GeometryStorage
{
    private readonly List<GameObject> meshGOStorage = new List<GameObject>();
    private readonly List<GameObject> curveGOStorage = new List<GameObject>();
    private readonly List<GameObject> txtGOStorage = new List<GameObject>();

    private delegate GameObject GetGameObject();

    public enum GeoType
    {
        Mesh,
        Curve,
        Txt
    }

    private static GeometryStorage instance;

    private GeometryStorage() { }

    public static GeometryStorage Instance
    {
        get
        {
            if (instance == null)
                instance = new GeometryStorage();

            return instance;
        }
    }


    /// <summary>
    /// Gets the Geometry from the Database, if there is no any, it is getting created
    /// </summary>
    public GameObject GetGeometry(int objNr, GeoType type)
    {
        try
        {
            switch (type)
            {
                default:
                case GeoType.Mesh:
                    return GetGeometry(objNr, meshGOStorage, Factory.Instance.CreateMeshObject);
                case GeoType.Curve:
                    return GetGeometry(objNr, curveGOStorage, Factory.Instance.CreateCurveObject);
                case GeoType.Txt:
                    return GetGeometry(objNr, txtGOStorage, Factory.Instance.CreateTextObject);
            }
        }
        catch (System.Exception e)
        {
            string error = $"ID: {objNr}, Typ: {type}, curveGOStorage {curveGOStorage.Count}, meshGOStorage {meshGOStorage.Count}, textGOStorage {txtGOStorage.Count} " + e.Message;
            throw new System.Exception(error);
        }
    }

    /// <summary>
    /// Gets the right Object- if it is not there, it is getting created
    /// </summary>
    private GameObject GetGeometry(int objNr, List<GameObject> storage, GetGameObject CreateObj)
    {
        GameObject stored;

        if (storage.Count - 1 < objNr)
        {
            stored = CreateObj();
            storage.Add(stored);
        }
        else
            stored = storage[objNr];

        return stored;
    }

    /// <summary>
    /// Updates the Geometry/ Deletes it
    /// </summary>
    /// <param name="count"></param>
    /// <param name="goTable"></param>
    private void UpdateGeo(int count, List<GameObject> goTable)
    {
        //Debug.Log($"UpdateGeo----------: " +  count +  "    "+goTable.Count);

        int tableCount = goTable.Count;
        if (count < tableCount)
        {
            int toDelete = tableCount - count;

            // Debug.Log($"Destroy: " + toDelete);

            List<GameObject> geos = new List<GameObject>();

            for (int i = 0; i < toDelete; i++)
                geos.Add(goTable[i]);

            for (int i = 0; i < geos.Count; i++)
            {
                goTable.RemoveAt(0);
                GameObject.Destroy(geos[i], i * 0.01f);
            }
        }
    }

    /// <summary>
    /// If the new GeometryCound is lower than before, Gemoetry gets deleted
    /// </summary>
    public void DeleteIfNeccesaryGeometry(int curveCount, int meshCount, int txtCount)
    {
        //Debug.Log($"GeoUpdate CRV/MSH/TXT:  {curveCount},  {meshCount},  {txtCount}");
        UpdateGeo(curveCount, curveGOStorage);
        UpdateGeo(meshCount, meshGOStorage);
        UpdateGeo(txtCount, txtGOStorage);
    }
}