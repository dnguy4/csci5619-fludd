using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cylinder : MonoBehaviour
{
	public Action<Collider, Collider> onTriggerEntered;
	public HashSet<GameObject> currentCollisions;
	public float radius;
	// Start is called before the first frame update
	void Start()
	{
		currentCollisions = new HashSet<GameObject>();
		radius = transform.localScale.x;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void ModifyRadius(float newRadius)
    {
		transform.localScale = new Vector3(newRadius, transform.localScale.y, newRadius);
		radius = newRadius;
    }

	public static void ModifyOpacity(GameObject obj, float newOpacity)
    {
		Renderer r = obj.GetComponent<Renderer>();
		Color oldColor = r.material.color;
		Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newOpacity);
		r.material.SetColor("_Color", newColor);
	}

	private void OnTriggerEnter(Collider other)
	{
		currentCollisions.Add(other.gameObject);
		ModifyOpacity(other.gameObject, 0.5f);

		// If there are any listeners...
		if (onTriggerEntered != null)
		{
			// ...broadcast the callback.
			onTriggerEntered(GetComponent<Collider>(), other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		currentCollisions.Remove(other.gameObject);

		ModifyOpacity(other.gameObject, 1f);
		foreach (GameObject gObject in currentCollisions)
		{
			print(gObject.name);
		}
	}

    private void OnDisable()
    {
		foreach (GameObject g in currentCollisions)
        {
			ModifyOpacity(g, 1f);
        }
		currentCollisions.Clear();
    }
}
