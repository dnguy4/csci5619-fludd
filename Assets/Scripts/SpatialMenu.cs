using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialMenu : MonoBehaviour
{
    public LineRenderer leftHand;
    public Transform[] points;

    public GameObject LinePrefab;
    public Canvas rightHand;
    private List<LineRenderer> lRenderers;
    public bool showing { get; private set; }
    void Start()
    {
        lRenderers = new List<LineRenderer>();
        int numSegments = points.Length / 2;
        showing = true;

        for (int i=0; i<numSegments; i++)
        {
            GameObject go = Instantiate(this.LinePrefab, transform);
            LineRenderer lr = go.GetComponent<LineRenderer>();
            lRenderers.Add(lr);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showing)
        {
            int j = 0;
            for (int i = 0; i < points.Length; i += 2)
            {
                lRenderers[j].SetPosition(0, points[i].position);
                lRenderers[j].SetPosition(1, points[i + 1].position);
                j += 1;
            }
        }
    }

    public void HideMenu()
    {
        showing = false;
        for (int i = 0; i < points.Length / 2; i += 1)
        {
            lRenderers[i].enabled = false;
        }
        rightHand.enabled = false;
    }

    public void ShowMenu()
    {
        showing = true;
        for (int i = 0; i < points.Length / 2; i += 1)
        {
            lRenderers[i].enabled = true;
        }
        rightHand.enabled = true;
    }
}
