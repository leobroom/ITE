using Microsoft.MixedReality.Toolkit.Input;
using System;
using UnityEngine;

//##################################################################
// Author Leon Brohmann
// E-Mail: Leonbrohmann@gmx.de | leon.brohmann@tu-braunschweig.de
//##################################################################

/// <summary>
/// This class controlls, what happens, if you use the select gesture etc.
/// </summary>
public class InputControl : MonoBehaviour
{
    /// <summary>
    /// The custom cursor  - indicates which modus is set
    /// </summary>
    [SerializeField]
    private GameObject cursor;

    /// <summary>
    /// HelperObject to get the GazeProvider provider
    /// </summary>
    [SerializeField]
    private GameObject providerObj;

    private static InputControl instance;

    public static InputControl Instance => instance;

    /// <summary>
    /// There can be just one modus active, these are the different modi
    /// </summary>
    private enum Modus
    {
        /// <summary>
        /// Default Mode - the initial state, you can do nothing here
        /// </summary>
        Reset = 0,
        /// <summary>
        /// Orient the World Coordinates
        /// </summary>
        Orient = 1,
        /// <summary>
        /// Set the index + and send it to the server, which sends it to rhino
        /// </summary>
        Next = 2,
        /// <summary>
        /// Set the index - and send it to the server, which sends it to rhino
        /// </summary>
        Previous = 3
    }

    /// <summary>
    /// The actual Modus of the App
    /// </summary>
    private Modus actualModus = Modus.Reset;

    /// <summary>
    /// The actual Index of the Rhino Geometry/Fiberwinding Component
    /// </summary>
    private int actualIndex = 0;

    /// <summary>
    /// MRTK - GazeProvider - The floating point you see, when you have the hololens on your head
    /// </summary>
    private GazeProvider provider;

    private void Awake() => instance = this;

    /// <summary>
    /// Set Up, when the Programm Starts
    /// </summary>
    void Start()
    {
        provider = providerObj.GetComponentInChildren<GazeProvider>();

        Cursor.Instantiate(cursor, provider);
        SetMode((int)Modus.Reset);
    }

    /// <summary>
    /// When the Select Action gets triggerd, this method is called - 
    /// see in Unity in the inputManager / Input Action scripts
    /// </summary>
    public void OnSelectStart()
    {
        Debug.Log("OnSelectStart");
    }

    public void OnSelectEnd()
    {
        Debug.Log("OnSelectEnd");

        switch (actualModus)
        {
            default:
            case Modus.Reset:
                // Do nothing
                break;
            case Modus.Orient:
                bool reset;
                Orientation.Instance.SetOrient(provider.HitPosition, out reset);

                if (reset)
                    SetMode(Modus.Reset);
                break;
            case Modus.Next:
                NextStep();
                break;
            case Modus.Previous:
                PreviousStep();
                break;
        }
    }

    /// <summary>
    /// This Method is called from the Unity Inspector in the InputAction Script
    /// </summary>
    /// <param name="modus"></param>
    public void SetMode(int modus) => SetMode((Modus)modus);

    /// <summary>
    /// Set the actual mode
    /// </summary>
    /// <param name="modus"></param>
    private void SetMode(Modus modus)
    {
        actualModus = modus;
        Debug.Log(actualModus);

        switch (actualModus)
        {
            case Modus.Orient:
                Cursor.Instance.SetMode(Cursor.Mode.Orient);
                Orientation.Instance.Reset();
                break;
            default:
            case Modus.Reset:
                Cursor.Instance.SetMode(Cursor.Mode.None);
                Orientation.Instance.Reset();
                break;
            case Modus.Next:
                Cursor.Instance.SetMode(Cursor.Mode.Next);
                Orientation.Instance.Reset();
                break;
            case Modus.Previous:
                Cursor.Instance.SetMode(Cursor.Mode.Previous);
                Orientation.Instance.Reset();
                break;
        }
    }

    internal void UpdadeStreamingIndex(int index)
    {
        Debug.Log("UpdadeStreamingIndex");
        actualIndex = index;
    }

    void Update()
    {
        Cursor.Instance.SetPosition();
        AskForHit();

        // Just for Debug - you can use the Enter Key for simulating the select finger pose
        if (Input.GetKeyDown(KeyCode.Q))
            OnSelectStart();

        //if (Input.GetKeyUp(KeyCode.KeypadEnter))
        //    OnSelectEnd();

        if (Input.GetKeyUp(KeyCode.O))
            SetMode(Modus.Orient);

        if (Input.GetKeyUp(KeyCode.N))
            SetMode(Modus.Next);

        if (Input.GetKeyUp(KeyCode.P))
            SetMode(Modus.Previous);
    }

    /// <summary>
    /// Sets the next Index and send it to Rhino
    /// </summary>
    public void NextStep()
    {
        actualIndex++;
        UnityClient.Instance.SendIndex(actualIndex);
    }

    /// <summary>
    /// Sets the previous Index and send it to Rhino
    /// </summary>
    public void PreviousStep()
    {
        actualIndex--;
        UnityClient.Instance.SendIndex(actualIndex);
    }

    public void AskForHit()
    {
        GameObject go;

        Cursor.ActiveObject actualObj = Cursor.Instance.AskForHit(out go);
    }

    public Cursor.ActiveObject activeObject;
    private GameObject activeGameObject;
}