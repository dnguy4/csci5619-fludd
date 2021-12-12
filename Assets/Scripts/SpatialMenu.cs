using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialMenu : MonoBehaviour
{
    public LineRenderer leftHand;
    public Transform[] points;

    public GameObject LinePrefab;
    private List<LineRenderer> lRenderers;
    void Start()
    {
        lRenderers = new List<LineRenderer>();
        int numSegments = points.Length / 2;

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
        int j = 0;
        for (int i=0; i < points.Length; i+= 2)
        {
            lRenderers[j].SetPosition(0, points[i].position);
            lRenderers[j].SetPosition(1, points[i+1].position);
            j += 1;
        }
    }

}
