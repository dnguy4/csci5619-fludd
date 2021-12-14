using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadMenu : MonoBehaviour
{
    public struct Selectable
    {
        public GameObject obj;
        public Vector3 initialPos;
        public Quaternion initialRot;
        public Vector3 initialSca;
        public Transform initialParent;

        public Selectable(GameObject o, Vector3 p, Quaternion r, Vector3 s, Transform par)
        {
            this.obj = o;
            this.initialPos = p;
            this.initialParent = par;
            this.initialRot = r;
            this.initialSca = s;
        }
    }

    public Transform[] buttons;
    public GraspGrabberRight rhand;
    List<Selectable>[] quads;
    Vector3 refSize = new Vector3(0.12f, 0.12f, 0.12f);

    void Start()
    {
        quads = new List<Selectable>[4];
        for (int i=0; i<4; i++)
        {
            quads[i] = new List<Selectable>();
        }
        gameObject.SetActive(false); //Hide menu
    }

    public void PopulateQuads(List<GameObject> objs)
    {
        for (int i=0; i< objs.Count; i++)
        {
            int index = i % 4;
            Selectable s = new Selectable(objs[i], objs[i].transform.position, 
                objs[i].transform.rotation, objs[i].transform.localScale, objs[i].transform.parent);
            quads[index].Add(s);

            ResizeGameobject(objs[i], refSize);
            Vector3 newScale = 100 * objs[i].transform.localScale;
            newScale.z = objs[i].transform.localScale.z;
            objs[i].transform.parent = buttons[index];

            //calculate offsets
            int horizontalOffset = (i / 4) % 4;
            int verticalOffset = (i / 4) / 4;
            objs[i].transform.localPosition = new Vector3(-30 + 15*horizontalOffset, 7 - verticalOffset*14, 0);

            // Set scale and rotation for proper display
            objs[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            objs[i].transform.localScale = newScale;

            Rigidbody rb = objs[i].GetComponent<Rigidbody>();
            if (rb)
                rb.useGravity = false;
            s.obj.GetComponent<Collider>().enabled = false;
        }
    }

    public void ResizeGameobject(GameObject go, Vector3 refSize)
    {
        //refSize should be defined in the coordinate space go is in
        Vector3 oldSize = go.GetComponent<Collider>().bounds.size;
        if (oldSize.sqrMagnitude < refSize.sqrMagnitude) return;

        float maxExtent = Mathf.Max(oldSize.x, oldSize.y, oldSize.z);
        float resizeX = refSize.x / maxExtent;
        float resizeY = refSize.y / maxExtent;
        float resizeZ = refSize.z / maxExtent;

        resizeX *= go.transform.localScale.x;
        resizeY *= go.transform.localScale.y;
        resizeZ *= go.transform.localScale.z;

        go.transform.localScale = new Vector3(resizeX, resizeY, resizeZ);
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

    public void ClearQuads(int i)
    {
        // Clear quads not equal to i
        for (int j = 0; j < 4; j++)
        {
            if (j != i)
            {
                foreach (Selectable s in quads[j])
                {
                    s.obj.transform.parent = s.initialParent;
                    s.obj.transform.rotation = s.initialRot;
                    s.obj.transform.localScale = s.initialSca;
                    s.obj.transform.position = s.initialPos;

                    Rigidbody rb = s.obj.GetComponent<Rigidbody>();
                    if (rb)
                        rb.useGravity = true;
                    s.obj.GetComponent<Collider>().enabled = true;
                }
                quads[j].Clear();
            }
        }
    }

    //Add onHover event to do the selection instead of clicking?
    public void OnButtonClick(int i)
    {
        ClearQuads(i);
        if (quads[i].Count == 1)
        {
            //Finish selection
            Selectable s = quads[i][0];
            rhand.GrabSelectableFromMenu(s);
            quads[i].Clear();
            gameObject.SetActive(false);
        }
        else
        {
            RedistributeQuads(quads[i]);
        }
    }
}
