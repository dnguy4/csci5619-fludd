using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadMenu : MonoBehaviour
{
    GameObject[] slots = new GameObject[8];
    Vector3[] initalPos = new Vector3[8];
    void Start()
    {
        //Just have predefined positions in each quad. We can use 4 positions per quad for now
        // Point to object inside menu to select immediately? or just refine until 1 per quad?
        // XR socket interactor might be good fit
        // https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@0.0/api/UnityEngine.XR.Interaction.Toolkit.XRSocketInteractor.html


    }

    void PopulateQuads(List<GameObject> objs)
    {
        for (int i=0; i< objs.Count; i++)
        {

        }
    }

    public void OnButtonClick(int i)
    {
        Debug.Log("Clicked " + i );
    }
}
