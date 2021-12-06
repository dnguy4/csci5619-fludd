using UnityEngine;
using UnityEngine.InputSystem;

public class PointerSelect : MonoBehaviour
{
    public LineRenderer laserPointer;
    public Material grabbablePointerMaterial;
    public Material grabTargetMaterial;

    public InputActionProperty touchAction;
    public InputActionProperty grabAction;

    public Transform rightHand;
    public Cylinder selectionPlane;
    public LayerMask layerMask;

    Material lineRendererMaterial;
    Transform grabPoint;
    GameObject grabbedObject;
    Transform initialParent;

    // Start is called before the first frame update
    void Start()
    {
        laserPointer.enabled = true;
        lineRendererMaterial = laserPointer.material;

        grabPoint = new GameObject().transform;
        grabPoint.name = "Grab Point";
        grabPoint.parent = this.transform;
        grabbedObject = null;
        initialParent = null;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;

        touchAction.action.performed += TouchDown;
        touchAction.action.canceled += TouchUp;

    }

    private void OnDestroy()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;

        touchAction.action.performed -= TouchDown;
        touchAction.action.canceled -= TouchUp;

    }

    // Update is called once per frame
    void Update()
    {
        if (laserPointer.enabled)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            {
                laserPointer.SetPosition(1, new Vector3(0, 0, hit.distance));

                if (hit.collider.GetComponent<Outline>())
                {
                    if (selectionPlane.currentCollisions.Contains(hit.collider.gameObject))
                    {
                        laserPointer.material = grabTargetMaterial;
                    } 
                    else
                    {
                        laserPointer.material = grabbablePointerMaterial;
                    }
                    
                }
                else
                {
                    laserPointer.material = lineRendererMaterial;
                }
            }
            else
            {
                laserPointer.SetPosition(1, new Vector3(0, 0, 100));
                laserPointer.material = lineRendererMaterial;
            }
        }
    }

    public void Grab(InputAction.CallbackContext context)
    {
        Debug.Log("Grabbing");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.GetComponent<Outline>() && selectionPlane.currentCollisions.Contains(hit.collider.gameObject))
            {
                grabPoint.localPosition = new Vector3(0, 0, hit.distance);
                grabbedObject = hit.collider.gameObject;
                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                initialParent = grabbedObject.transform.parent;
                grabbedObject.transform.parent = transform;
                grabbedObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void Release(InputAction.CallbackContext context)
    {
        if (grabbedObject)
        {
            Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            grabbedObject.transform.parent = initialParent;
            grabbedObject = null;
        }
    }

    void TouchDown(InputAction.CallbackContext context)
    {
        laserPointer.enabled = true;
    }

    void TouchUp(InputAction.CallbackContext context)
    {
        laserPointer.enabled = false;
    }
}