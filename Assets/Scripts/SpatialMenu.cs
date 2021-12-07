using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialMenu : MonoBehaviour
{
    public LineRenderer leftHand;
    public Transform[] points;
    public GameObject handMenu;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        for (int i=0; i < points.Length; i++)
        {
            leftHand.SetPosition(i, points[i].position);
        }
    }
    public void ButtonClicked()
    {
        handMenu.SetActive(false);
    }
}
