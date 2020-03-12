using GeoStreamer;
using TMPro;
using UnityEngine;

//##################################################################
// Author Leon Brohmann
// E-Mail: Leonbrohmann@gmx.de | leon.brohmann@tu-braunschweig.de
//##################################################################

/// <summary>
/// This Singleton creates and updates Unity Geometry through the Networkpackages, 
/// which were send from the Server
/// </summary>
public class Factory
{
    /// <summary>
    /// <see cref="https://de.wikipedia.org/wiki/Singleton_(Entwurfsmuster)"/>
    /// </summary>
    private static Factory instance;

    private Factory() { }

    /// <summary>
    /// <see cref="https://de.wikipedia.org/wiki/Singleton_(Entwurfsmuster)"/>
    /// creates, when it is called first the Unity ParentObject
    /// </summary>
    public static Factory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Factory();
                instance.CreateWorldCoordinateSystem();
            }

            return instance;
        }
    }

    /// <summary>
    /// The WorldCoordinate parent
    /// </summary>
    private GameObject worldParent;

    /// <summary>
    /// is needed to update colors
    /// <seealso cref="https://thomasmountainborn.com/2016/05/25/materialpropertyblocks/"/>
    /// </summary>
    private readonly MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

    //##################### WORLD COORDINATES #######################################################

    /// <summary>
    /// CreateWorldCoordinateSystem and CreateParent doing similar stuff - need to clean this stuff - LB
    /// </summary>
    private void CreateWorldCoordinateSystem()
    {
        worldParent = new GameObject("worldParent");
        worldParent.transform.localPosition = new Vector3(-0.5f,0,-0.5f);
        worldParent.transform.rotation = Quaternion.Euler(-90, -90, 0);
        worldParent.transform.localScale = new Vector3(-1, 1, 1);
    }

    /// <summary>
    /// CreateWorldCoordinateSystem and CreateParent doing similar stuff - need to clean this stuff - LB
    /// </summary>
    public void CreateParent(GameObject grandparent)
    {
        worldParent.transform.SetParent(grandparent.transform, false);
        //worldParent.transform.rotation = Quaternion.Euler(-90, 0, 0);
        //worldParent.transform.localScale = new Vector3(-1, 1, 1);
    }

    //################################ MESHES ################################

    /// <summary>
    /// Creates an empty MeshObject in Unity
    /// </summary>
    public GameObject CreateMeshObject()
    {
        GameObject meshObject = new GameObject("MeshObject");
        meshObject.transform.SetParent(worldParent.transform, false);

        MeshFilter filter = meshObject.AddComponent<MeshFilter>();
        filter.mesh = new Mesh();

        var renderer = meshObject.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = GeometryStreamer.Instance.SurfaceMat;

        return meshObject;
    }

    /// <summary>
    /// Updates a Mesh and overwrites its informations
    /// </summary>
    public void UpdateMesh(BroadCastMesh broadcast)
    {
        GameObject go = GeometryStorage.Instance.GetGeometry(broadcast.meshNr, GeometryStorage.GeoType.Mesh);
        MeshFilter filter = go.GetComponent<MeshFilter>();

        Mesh mesh = filter.mesh;
        mesh.Clear();
        mesh.SetVertices(GeoUtils.GetVector3List(broadcast.vertices));
        mesh.SetTriangles(broadcast.triangles, 0);
        mesh.SetNormals(GeoUtils.GetVector3List(broadcast.normals));

        Color c = GeoUtils.GetUColor(broadcast.color);
        var renderer = go.GetComponent<Renderer>();

        renderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", c);
        renderer.SetPropertyBlock(propBlock);
    }

    //################################ CURVES ################################

    /// <summary>
    /// Creates an empty CurveObject in Unity
    /// </summary>
    public GameObject CreateCurveObject()
    {
        GameObject go = new GameObject("CurveObject");
        go.transform.SetParent(worldParent.transform, false);

        LineRenderer linerenderer = go.AddComponent<LineRenderer>();
        linerenderer.useWorldSpace = false;

        linerenderer.sharedMaterial = GeometryStreamer.Instance.CurveMat;
        float width = 0.004f;
        linerenderer.startWidth = width;
        linerenderer.endWidth = width;
        linerenderer.receiveShadows = false;
        linerenderer.allowOcclusionWhenDynamic = false;
        linerenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        linerenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        linerenderer.positionCount = 0;

        return go;
    }

    /// <summary>
    /// Updates a CurveObject and overwrites its informations
    /// </summary>
    public void UpdateCurve(BroadCastCurve broadcast)
    {
        var allPoints = GeoUtils.GetVector3Array(broadcast.positions);

        int length = allPoints.Length;

        GameObject go = GeometryStorage.Instance.GetGeometry(broadcast.curveNr, GeometryStorage.GeoType.Curve);
        LineRenderer renderer = go.GetComponent<LineRenderer>();
        renderer.positionCount = length;
        renderer.SetPositions(allPoints);

        renderer.GetPropertyBlock(propBlock);
        Color c = GeoUtils.GetUColor(broadcast.colors);
        propBlock.SetColor("_Color", c);
        renderer.SetPropertyBlock(propBlock);

        float width = broadcast.width * 2;

        if (renderer.startWidth == width)
            return;

        renderer.startWidth = width;
        renderer.endWidth = width;
    }

    //################################ TEXTS ################################

    /// <summary>
    /// Creates an empty TextObject in Unity
    /// </summary>
    public GameObject CreateTextObject()
    {
        GameObject txtPrefab = GeometryStreamer.Instance.TxtPrefab;
        GameObject go = GameObject.Instantiate(txtPrefab);

        go.name = "DEFAULT";
        go.transform.SetParent(worldParent.transform, false);

        return go;
    }

    /// <summary>
    /// Updates a TextObject and overwrites its informations
    /// </summary>
    internal void UpdateText(BroadCastText broadcast)
    {
        GameObject go = GeometryStorage.Instance.GetGeometry(broadcast.textNr, GeometryStorage.GeoType.Txt);
        TextMeshPro textMesh = go.GetComponent<TextMeshPro>();

        textMesh.color = GeoUtils.GetUColor(broadcast.color);
        go.transform.localPosition = GeoUtils.GetVector(broadcast.position);
        go.transform.localEulerAngles = GeoUtils.GetVector(broadcast.rotation);
        textMesh.text = broadcast.text;
        textMesh.fontSize = broadcast.textSize / 2;
    }

    //################################ GEOMETRY INFORMATIONS ################################

    /// <summary>
    /// Deletes Geometry, if the GeometryCount changed
    /// </summary>
    public void UpdateGeometry(BroadCastGeometryInfo broadcast)
        => GeometryStorage.Instance.DeleteIfNeccesaryGeometry(broadcast.curvesCount, broadcast.meshesCount, broadcast.textCount);
}