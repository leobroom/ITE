using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

//##################################################################
// Author Leon Brohmann
// E-Mail: Leonbrohmann@gmx.de | leon.brohmann@tu-braunschweig.de
//##################################################################

/// <summary>
/// Simple Cursor Script to Create a cheap Cursor, which can change the color
/// </summary>
public class Cursor
{
    /// <summary>
    /// <see cref="https://de.wikipedia.org/wiki/Singleton_(Entwurfsmuster)"/>
    /// </summary>
    private static Cursor instance;

    private GameObject cursorObj;

    /// <summary>
    /// MRTK - GazeProvider - The floating point you see, when you have the hololens on your head
    /// </summary>
    private GazeProvider provider;

    /// <summary>
    /// The Cursor Mode
    /// </summary>
    public enum Mode
    {
        None,
        Orient,
        Next,
        Previous
    }

    /// <summary>
    /// The actual set CursorMode
    /// </summary>
    private Mode actualCursor;

    /// <summary>
    /// is needed to update colors
    /// <seealso cref="https://thomasmountainborn.com/2016/05/25/materialpropertyblocks/"/>
    /// </summary>
    private readonly MaterialPropertyBlock propBlock = new MaterialPropertyBlock();

    private Cursor(GameObject prefab, GazeProvider provider)
    {
        cursorObj = GameObject.Instantiate(prefab);
        this.provider = provider;
    }

    public static Cursor Instance => instance;

    /// <summary>
    /// Creates a Cursor Singleton
    /// </summary>
    /// <param name="cursorPrefab"></param>
    /// <param name="provider"></param>
    public static void Instantiate(GameObject cursorPrefab, GazeProvider provider)
        => instance = new Cursor(cursorPrefab, provider);

    /// <summary>
    /// Sets the Mode of the Cursor (changes the color)
    /// </summary>
    /// <param name="actualCurs"></param>
    public void SetMode(Mode actualCurs)
    {
        actualCursor = actualCurs;

        switch (actualCursor)
        {
            default:
            case Mode.None:
                SetCursorStatus(false);
                break;
            case Mode.Orient:
                SetCursorStatus(true);
                SetCursorColor(Color.grey);
                break;
            case Mode.Next:
                SetCursorStatus(true);
                SetCursorColor(Color.white);
                break;
            case Mode.Previous:
                SetCursorStatus(true);
                SetCursorColor(Color.red);
                break;
        }
    }

    /// <summary>
    /// Is the Cursor visible?
    /// </summary>
    /// <param name="isActive"></param>
    private void SetCursorStatus(bool isActive) => cursorObj.SetActive(isActive);

    private void SetCursorColor(Color c)
    {
        LineRenderer[] renderers = cursorObj.GetComponentsInChildren<LineRenderer>();

        foreach (var r in renderers)
        {
            r.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", c);
            r.SetPropertyBlock(propBlock);
        }
    }

    /// <summary>
    /// Sets the position of the Cursor
    /// </summary>
    public void SetPosition()
    {
        Vector3 hitposition = provider.HitPosition;

        cursorObj.gameObject.transform.position = hitposition;
        cursorObj.gameObject.transform.localRotation = Quaternion.LookRotation(provider.transform.forward, provider.transform.up);
    }

    public enum ActiveObject
    {
        None,
        Z,
        X,
        Y
    }

    public Cursor.ActiveObject activeObject;
    private GameObject activeGameObject;

    public ActiveObject AskForHit(out GameObject gameObj)
    {
        gameObj = null;


        Cursor.ActiveObject actual = ActiveObject.None;

            Collider col = provider.HitInfo.collider;
        if (col == null) { }
        else  if (col.tag == "Z")
        {
            gameObj = provider.HitInfo.collider.gameObject;
            actual = ActiveObject.Z;
        }
        else if (col.tag == "X")
        {
            gameObj = provider.HitInfo.collider.gameObject;
            actual = ActiveObject.X;
        }
        else if (col.tag == "Y")
        {
            gameObj = provider.HitInfo.collider.gameObject;
            actual = ActiveObject.Y;
        }

        if (actual != activeObject)
        {
            activeGameObject?.GetComponent<GeoTrigger>()?.SetTriggerActive(false);
            gameObj?.GetComponent<GeoTrigger>()?.SetTriggerActive(true);

            activeGameObject = gameObj;
        }

        activeObject = actual;
        return activeObject;
    }
}