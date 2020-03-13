using System.Collections.Generic;
using GeoStreamer;
using SocketStreamer;

//##################################################################
// Author Leon Brohmann
// E-Mail: Leonbrohmann@gmx.de | leon.brohmann@tu-braunschweig.de
//##################################################################

/// <summary>
/// The UnityClient connects to the Server
/// NO DEBUGLOG HERE! UnityClient is running in a different thread - Unity doesn't like that!
/// </summary>
class UnityClient : GeoClient<UnityClient>
{
    /// <summary>
    /// The Client adds here in this list the geometryData, which it  recieves from the Server
    /// </summary>
    private readonly Queue<ISerializableData> geometryChanged = new Queue<ISerializableData>();

    /// <summary>
    /// The Client adds here in this list the gathered informatioens, like geometrycount BEFORE it recieves the changedGeometry
    /// </summary>
    private readonly Queue<ISerializableData> informationsChanged = new Queue<ISerializableData>();

    private readonly Queue<ISerializableData> indexChanged = new Queue<ISerializableData>();

    /// <summary>
    /// UpdateCurves is overwritten from its derived class: GeoClient, 
    /// everytime gemetry data is send, it is stacked in a special "list", 
    /// which is later processed in the unity update circle 
    /// </summary>
    /// <param name="data">The network class for Curves</param>
    protected override void UpdateCurves(BroadCastCurve data)
    {
        lock (geometryChanged)
            geometryChanged.Enqueue(data);
    }

    /// <summary>
    /// UpdateMesh is overwritten from its derived class: GeoClient, 
    /// everytime geometry data is send, it is stacked in a special "list", 
    /// which is later processed in the unity update circle 
    /// </summary>
    /// <param name="data">The network class for Meshes</param>
    protected override void UpdateMesh(BroadCastMesh data)
    {
        lock (geometryChanged)
            geometryChanged.Enqueue(data);
    }

    /// <summary>
    /// UpdateText is overwritten from its derived class: GeoClient, 
    /// everytime geometry data is send, it is stacked in a special "list", 
    /// which is later processed in the unity update circle 
    /// </summary>
    /// <param name="data">The network class for Texts</param>
    protected override void UpdateText(BroadCastText data)
    {
        lock (geometryChanged)
            geometryChanged.Enqueue(data);
    }

    /// <summary>
    /// Updates the GeometryInfo - Unity has to know how many geometry is send, so it can create or delete GameObjects
    /// </summary>
    /// <param name="geoinfo">The network class for GeometryInfo</param>
    protected override void UpdateGeometry(BroadCastGeometryInfo geoinfo)
    {
        lock (informationsChanged)
        {
            informationsChanged.Enqueue(geoinfo);

            //Clears old Geometry
            geometryChanged.Clear();
        }
    }

    protected override void UpdateIndex(BroadCastIndex updateIdex)
    {
        lock (indexChanged)
        {
            indexChanged.Enqueue(updateIdex);
        }
    }

    /// <summary>
    /// This Method is called from the Unity Update Routine. It is processing all the gathered networkdata.
    /// <see cref="GeometryStreamer.Update()"/>
    /// </summary>
    public void ProcessMessages()
    {
        //Update Index
        ISerializableData updateIndex = null;

        lock (indexChanged)
            if (indexChanged.Count > 0)
            {
                updateIndex = indexChanged.Dequeue();
                indexChanged.Clear();
            }

        if (updateIndex != null && updateIndex is BroadCastIndex)
            InputControl.Instance.UpdadeStreamingIndex(((BroadCastIndex)updateIndex).index);


        ISerializableData updateGeometry = null;

        // First it looks, if we have some genereal informations about the meshes. Rhino as example sends first a 
        // BroadCastGeometryInfo Instance over the network, to tell Unity how many Gemoetry got sent.
        lock (informationsChanged)
            if (informationsChanged.Count > 0)
                updateGeometry = informationsChanged.Dequeue();

        //UpdateGeometry() will create or delete geometry, if the geometry count changed.
        if (updateGeometry != null)
            Factory.Instance.UpdateIndex((BroadCastGeometryInfo)updateGeometry);


        ISerializableData broadcast = null;


        // It looks, if geometry got sent.
        lock (geometryChanged)
            if (geometryChanged.Count > 0)
                broadcast = geometryChanged.Dequeue();

        if (broadcast == null)
            return;

        // Is the geometry a mesh, curve or text ?
        // Update old geometry

        if (broadcast is BroadCastMesh)
            Factory.Instance.UpdateMesh((BroadCastMesh)broadcast);
        else if (broadcast is BroadCastCurve)
            Factory.Instance.UpdateCurve((BroadCastCurve)broadcast);
        else if (broadcast is BroadCastText)
            Factory.Instance.UpdateText((BroadCastText)broadcast);
    }

    /// <summary>
    /// Send the actual GeometryIndex to the Server - Rhino reads it, actualise its StreamingGate
    /// - Geometry will change afterwards
    /// </summary>
    /// <param name="idx"></param>
    public void SendIndex(int idx)
    {
        BroadCastIndex idxMsg = new BroadCastIndex
        {
            gateId = 0,
            index = idx
        };

        Send(idxMsg);
    }
}