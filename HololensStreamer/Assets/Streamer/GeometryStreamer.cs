using UnityEngine;
using GeoStreamer;
using SocketStreamer;
using System.Collections.Generic;

//##################################################################
// Author Leon Brohmann
// E-Mail: Leonbrohmann@gmx.de | leon.brohmann@tu-braunschweig.de
//##################################################################

/// <summary>
/// The Main Class in Unity to create a unity (GeometryStreamer)Client and add a Debugger to it
/// </summary>
public class GeometryStreamer : MonoBehaviour
{
    /// <summary>
    /// Server client to connect with the 
    /// </summary>
    private UnityClient client;

    /// <summary>
    /// The surface material of the streamed Objects 
    /// </summary>
    [SerializeField]
    private Material surfaceMat;

    /// <summary>
    /// The curve material of the streamed Objects 
    /// </summary>
    [SerializeField]
    private Material curveMat;
    /// <summary>
    /// The Parent of all streamed Objects - this is the Gameobject, which will change the position, after Orientation
    /// </summary>
    [SerializeField]
    private GameObject worldCoordinates;
    public GameObject WorldCoordinates => worldCoordinates;

    [SerializeField]
    private GameObject txtPrefab;
    public GameObject TxtPrefab => txtPrefab;

    /// <summary>
    /// The IPAdress to the SErver
    /// </summary>
    [SerializeField]
    private string ipAddress = "127.0.0.1";

    public bool connect = true;

    /// <summary>
    /// GeometryStreamer Singleton 
    /// </summary>
    private static GeometryStreamer instance;

    /// <summary>
    /// GeometryStreamer Singleton 
    /// </summary>
    public static GeometryStreamer Instance => instance;

    /// <summary>
    /// The Curve material of the streamed Objects 
    /// </summary>
    public Material CurveMat => curveMat;

    /// <summary>
    /// The Surface material of the streamed Objects 
    /// </summary>
    public Material SurfaceMat => surfaceMat;

    /// <summary>
    /// DebugList
    /// </summary>
    private static Queue<string> debugLog = new Queue<string>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Factory.Instance.CreateParent(worldCoordinates);

        client = UnityClient.Initialize
            (ipAddress, Utils.GetTestPort(), "Hololens", ThreadingType.Thread);
        client.Message += OnMessage;
        if (connect)
            client.Connect();
    
        SendWelcomeMsgToServer();
    }

    private void Update()
    {
        client.ProcessMessages();
        ProcessDebugLogs();
    }

    private void SendWelcomeMsgToServer()
    {
        BroadCastMsg bc = new BroadCastMsg() { broadcastMsg = " Hey hier ist Unity Projekt!" };
        client.Send(bc);
    }

    /// <summary>
    /// The client gets disconnected 
    /// </summary>
    private void OnDisable() => client?.Disconnect();

    /// <summary>
    /// Send the DebugLog to the Console
    /// </summary>
    private void ProcessDebugLogs()
    {
        lock (debugLog)
        {
            if (debugLog.Count > 0)
                Debug.Log(debugLog.Dequeue());
        }
    }

    /// <summary>
    /// is a unityClient message created, it will be sown in the debug Console
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnMessage(object sender, MessageArgs e)
    {
        lock (debugLog)
            debugLog.Enqueue(e.Message);
    }
}