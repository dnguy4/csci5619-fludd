using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadMenu : MonoBehaviour
{
    struct Selectable
    {
        public GameObject obj;
        public Vector3 initialPos;
        public Transform initialParent;

        public Selectable(GameObject o, Vector3 p, Transform par)
        {
            this.obj = o;
            this.initialPos = p;
            this.initialParent = par;
        }
    }

    public Transform[] buttons;
    List<Selectable>[] quads;

    void Start()
    {
        //Just have predefined positions in each quad. We can use 4 positions per quad for now
        // Point to object inside menu to select immediately? or just refine until 1 per quad?
        // XR socket interactor might be good fit
        // https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@0.0/api/UnityEngine.XR.Interaction.Toolkit.XRSocketInteractor.html

        quads = new List<Selectable>[4];
        for (int i=0; i<4; i++)
        {
            quads[i] = new List<Selectable>();
        }

    }

    public void PopulateQuads(List<GameObject> objs)
    {
        for (int i=0; i< objs.Count; i++)
        {
            int index = i % 4;
            Selectable s = new Selectable(objs[i], objs[i].transform.position, objs[i].transform.parent);
            quads[index].Add(s);

            objs[i].transform.parent = buttons[index];
            //calculate offsets
            int horizontalOffset = (i / 4) % 4;
            int verticalOffset = (i / 4) / 4;
            objs[i].transform.localPosition = new Vector3(-30 + 15*horizontalOffset, 7 - verticalOffset*14, 0);
        }
    }


    void RedistributeQuads(List<Selectable> list)
    {
        int parts = Mathf.Min(list.Count, 4);
        var newQuads = list.Split(parts).ToList();

        for (int i = 0; i< parts; i++)
        {
            quads[i] = newQuads[i].ToList();
            for (int j = 0; j < quads[i].Count; j++)
            {
                GameObject GO = quads[i][j].obj;
                GO.transform.parent = buttons[i];
                int horizontalOffset = j % 4;
                int verticalOffset = j / 4;
                GO.transform.localPosition = new Vector3(-30 + 15 * horizontalOffset, 7 - verticalOffset * 14, 0);
            }
        }
    }

    public void OnButtonClick(int i)
    {
        for (int j = 0; j < 4; j++)
        {
            if (j != i)
            {
                foreach (Selectable s in quads[j])
                {
                    s.obj.transform.position = s.initialPos;
                    s.obj.transform.parent = s.initialParent;
                }
                quads[j].Clear();
            }
        }

        if (quads[i].Count == 1)
        {
            //Finish selection
        }
        else
        {
            RedistributeQuads(quads[i]);
        }
    }
}