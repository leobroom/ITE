using UnityEngine;

public class AxisTrigger : MonoBehaviour, GeoTrigger
{
    LineRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.GetComponent<LineRenderer>();
    }

    void Update() { }

    private void SetWidth(float width)
    {
        renderer.startWidth = width;
        renderer.endWidth = width;
    }

    public void SetTriggerActive(bool active)
    {
        float width = (active) ? 0.015f : 0.005f;
        SetWidth(width);
    }
}
